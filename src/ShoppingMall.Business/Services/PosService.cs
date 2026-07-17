using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class PosService
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly IProductRepository _productRepo;
    private readonly ICurrentStockRepository _stockRepo;
    private readonly IRepository<Payment> _paymentRepo;
    private readonly IRepository<TaxBreakdown> _taxRepo;
    private readonly GstCalculator _gstCalculator;
    private readonly IRepository<Customer> _customerRepo;

    public PosService(
        ITransactionRepository transactionRepo,
        IProductRepository productRepo,
        ICurrentStockRepository stockRepo,
        IRepository<Payment> paymentRepo,
        IRepository<TaxBreakdown> taxRepo,
        GstCalculator gstCalculator,
        IRepository<Customer> customerRepo)
    {
        _transactionRepo = transactionRepo;
        _productRepo = productRepo;
        _stockRepo = stockRepo;
        _paymentRepo = paymentRepo;
        _taxRepo = taxRepo;
        _gstCalculator = gstCalculator;
        _customerRepo = customerRepo;
    }

    public async Task<Transaction> CreateTransactionAsync(Guid storeId, Guid terminalId, Guid? userId)
    {
        var receiptNo = await _transactionRepo.GenerateReceiptNumberAsync(storeId);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            TerminalId = terminalId,
            UserId = userId,
            ReceiptNumber = receiptNo,
            Status = Core.Enums.TransactionStatus.Active,
            CreatedAt = DateTime.UtcNow,
            IdempotencyKey = Guid.NewGuid()
        };

        return await _transactionRepo.AddAsync(transaction);
    }

    public async Task<TransactionLine> AddLineItemAsync(Guid transactionId, Guid productId, decimal quantity, decimal? overridePrice = null)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Product not found");

        var transaction = await _transactionRepo.GetByIdAsync(transactionId);
        if (transaction == null || transaction.Status != Core.Enums.TransactionStatus.Active)
            throw new InvalidOperationException("Transaction not found or not active");

        var unitPrice = overridePrice ?? product.SellingPrice ?? 0;
        var discountAmount = 0m;
        var taxRate = product.TaxRate;

        var gstResult = _gstCalculator.Calculate(quantity * unitPrice, taxRate, Core.Enums.SupplyType.IntraState);

        var lineNo = transaction.Lines.Count + 1;

        var line = new TransactionLine
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            ProductId = productId,
            LineNumber = lineNo,
            ProductName = product.Name,
            SKU = product.SKU,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Mrp = product.Mrp ?? 0,
            DiscountAmount = discountAmount,
            TaxRate = taxRate,
            TaxAmount = gstResult.TotalTaxAmount,
            NetAmount = gstResult.GrandTotal,
            IsWeighable = product.IsWeighable
        };

        transaction.Lines.Add(line);
        transaction.SubTotal += quantity * unitPrice;
        transaction.TaxTotal += gstResult.TotalTaxAmount;
        transaction.GrandTotal = transaction.SubTotal - transaction.DiscountTotal + transaction.TaxTotal;
        await _transactionRepo.UpdateAsync(transaction);

        return line;
    }

    public async Task<Payment> ProcessPaymentAsync(Guid transactionId, decimal amount, Core.Enums.PaymentMethod method, decimal? tenderedAmount = null)
    {
        var transaction = await _transactionRepo.GetByIdAsync(transactionId);
        if (transaction == null)
            throw new InvalidOperationException("Transaction not found");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Method = method,
            Amount = amount,
            TenderedAmount = tenderedAmount,
            ChangeAmount = tenderedAmount - amount,
            Status = Core.Enums.PaymentStatus.Captured,
            IdempotencyKey = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepo.AddAsync(payment);

        var totalPaid = transaction.Payments.Sum(p => p.Amount) + amount;
        if (totalPaid >= transaction.GrandTotal)
        {
            transaction.Status = Core.Enums.TransactionStatus.Completed;
            await _transactionRepo.UpdateAsync(transaction);
        }

        return payment;
    }

    public async Task<Transaction?> LookupByReceiptAsync(string receiptNumber)
        => await _transactionRepo.GetByReceiptNumberAsync(receiptNumber);

    public async Task<Transaction?> LookupByPhoneAsync(string phone)
    {
        var customers = await _customerRepo.FindAsync(c => c.Phone == phone);
        var customer = customers.FirstOrDefault();
        if (customer == null) return null;

        var transactions = await _transactionRepo.FindAsync(t => t.CustomerId == customer.Id);
        return transactions.OrderByDescending(t => t.CreatedAt).FirstOrDefault();
    }
}

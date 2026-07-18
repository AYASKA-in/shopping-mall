using Microsoft.EntityFrameworkCore;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;
using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Business.Services;

public class PosService
{
    private readonly ShoppingMallDbContext _db;
    private readonly ITransactionRepository _transactionRepo;
    private readonly IProductRepository _productRepo;
    private readonly ICurrentStockRepository _stockRepo;
    private readonly IRepository<Payment> _paymentRepo;
    private readonly IRepository<TaxBreakdown> _taxRepo;
    private readonly GstCalculator _gstCalculator;
    private readonly InventoryService _inventoryService;
    private readonly IRepository<Customer> _customerRepo;

    public PosService(
        ShoppingMallDbContext db,
        ITransactionRepository transactionRepo,
        IProductRepository productRepo,
        ICurrentStockRepository stockRepo,
        IRepository<Payment> paymentRepo,
        IRepository<TaxBreakdown> taxRepo,
        GstCalculator gstCalculator,
        InventoryService inventoryService,
        IRepository<Customer> customerRepo)
    {
        _db = db;
        _transactionRepo = transactionRepo;
        _productRepo = productRepo;
        _stockRepo = stockRepo;
        _paymentRepo = paymentRepo;
        _taxRepo = taxRepo;
        _gstCalculator = gstCalculator;
        _inventoryService = inventoryService;
        _customerRepo = customerRepo;
    }

    public async Task<Transaction> CreateTransactionAsync(Guid storeId, Guid? terminalId, Guid? userId)
    {
        var receiptNo = await _transactionRepo.GenerateReceiptNumberAsync(storeId);
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            TerminalId = terminalId ?? Guid.Empty,
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

        var newSubTotal = transaction.SubTotal + quantity * unitPrice;
        var newTaxTotal = transaction.TaxTotal + gstResult.TotalTaxAmount;
        var newGrandTotal = newSubTotal - transaction.DiscountTotal + newTaxTotal;

        _db.TransactionLines.Add(line);
        await _db.Transactions
            .Where(t => t.Id == transactionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.SubTotal, newSubTotal)
                .SetProperty(t => t.TaxTotal, newTaxTotal)
                .SetProperty(t => t.GrandTotal, newGrandTotal));
        await _db.SaveChangesAsync();

        return line;
    }

    public async Task<Payment> ProcessPaymentAsync(Guid transactionId, decimal amount, Core.Enums.PaymentMethod method, decimal? tenderedAmount = null)
    {
        var transaction = await _db.Transactions
            .Include(t => t.Lines)
            .Include(t => t.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == transactionId);
        if (transaction == null)
            throw new InvalidOperationException("Transaction not found");

        if (transaction.Status == Core.Enums.TransactionStatus.Completed)
            throw new InvalidOperationException("Transaction already completed");

        // Check idempotency — skip if payment with same details already exists
        var existingPayment = transaction.Payments.FirstOrDefault(p =>
            p.Amount == amount && p.Method == method);
        if (existingPayment != null)
            return existingPayment;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Method = method,
            Amount = amount,
            TenderedAmount = tenderedAmount,
            ChangeAmount = (tenderedAmount ?? 0) - amount,
            Status = Core.Enums.PaymentStatus.Captured,
            IdempotencyKey = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Payments.Add(payment);

        foreach (var line in transaction.Lines)
        {
            try
            {
                await _inventoryService.DeductStockAsync(transaction.StoreId, line.ProductId,
                    line.Quantity, "SALE", transaction.Id, line.UnitPrice);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Insufficient stock for '{line.ProductName}': {ex.Message}");
            }
        }

        await _db.Transactions
            .Where(t => t.Id == transactionId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Status, Core.Enums.TransactionStatus.Completed));
        await _db.SaveChangesAsync();

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

using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class PosServiceTests
{
    private readonly Mock<ITransactionRepository> _txnRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<ICurrentStockRepository> _stockRepo = new();
    private readonly Mock<IRepository<Payment>> _paymentRepo = new();
    private readonly Mock<IRepository<TaxBreakdown>> _taxRepo = new();
    private readonly Mock<IRepository<Customer>> _customerRepo = new();
    private readonly GstCalculator _gst = new();
    private readonly Mock<IRepository<StockLedger>> _ledgerRepo = new();
    private readonly InventoryService _inventory;
    private readonly PosService _sut;

    public PosServiceTests()
    {
        _inventory = new InventoryService(_stockRepo.Object, _ledgerRepo.Object, _productRepo.Object);
        _sut = new PosService(
            _txnRepo.Object, _productRepo.Object, _stockRepo.Object,
            _paymentRepo.Object, _taxRepo.Object, _gst, _inventory, _customerRepo.Object);
    }

    [Fact]
    public async Task CreateTransactionAsync_GeneratesReceiptAndIdempotencyKey()
    {
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _txnRepo.Setup(r => r.GenerateReceiptNumberAsync(storeId))
            .ReturnsAsync("RCP-0001");
        _txnRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => t);

        var result = await _sut.CreateTransactionAsync(storeId, terminalId, userId);

        result.StoreId.Should().Be(storeId);
        result.TerminalId.Should().Be(terminalId);
        result.UserId.Should().Be(userId);
        result.ReceiptNumber.Should().Be("RCP-0001");
        result.Status.Should().Be(TransactionStatus.Active);
        result.IdempotencyKey.Should().NotBeNull();
    }

    [Fact]
    public async Task AddLineItemAsync_AddsLineAndUpdatesTotals()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            SKU = "TST-001",
            SellingPrice = 100,
            TaxRate = 18,
            Mrp = 120
        };
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            TerminalId = Guid.NewGuid(),
            Status = TransactionStatus.Active,
            Lines = new List<TransactionLine>()
        };

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _txnRepo.Setup(r => r.GetByIdAsync(txn.Id)).ReturnsAsync(txn);

        var result = await _sut.AddLineItemAsync(txn.Id, productId, 2);

        result.ProductId.Should().Be(productId);
        result.ProductName.Should().Be("Test Product");
        result.Quantity.Should().Be(2);
        result.UnitPrice.Should().Be(100);
        txn.SubTotal.Should().Be(200);
        txn.TaxTotal.Should().Be(36);
    }

    [Fact]
    public async Task AddLineItemAsync_OverridePrice_UsesOverriddenPrice()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test",
            SellingPrice = 100,
            TaxRate = 0
        };
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            TerminalId = Guid.NewGuid(),
            Status = TransactionStatus.Active,
            Lines = new List<TransactionLine>()
        };

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _txnRepo.Setup(r => r.GetByIdAsync(txn.Id)).ReturnsAsync(txn);

        var result = await _sut.AddLineItemAsync(txn.Id, productId, 1, 150);

        result.UnitPrice.Should().Be(150);
        txn.SubTotal.Should().Be(150);
    }

    [Fact]
    public async Task ProcessPaymentAsync_CompletesTransaction()
    {
        var line = new TransactionLine
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ProductName = "Item",
            Quantity = 2,
            UnitPrice = 50,
            TaxRate = 0,
            NetAmount = 100
        };
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            TerminalId = Guid.NewGuid(),
            Status = TransactionStatus.Active,
            SubTotal = 100,
            GrandTotal = 100,
            Lines = new List<TransactionLine> { line },
            Payments = new List<Payment>()
        };

        _txnRepo.Setup(r => r.GetByIdAsync(txn.Id)).ReturnsAsync(txn);
        _txnRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
            .Returns(Task.CompletedTask);

        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = txn.StoreId,
            ProductId = line.ProductId,
            OnHand = 100,
            Reserved = 0,
            Available = 100
        };
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(txn.StoreId, line.ProductId))
            .ReturnsAsync(stock);

        var result = await _sut.ProcessPaymentAsync(txn.Id, 100, PaymentMethod.Cash, 100);

        result.Amount.Should().Be(100);
        result.Method.Should().Be(PaymentMethod.Cash);
        result.ChangeAmount.Should().Be(0);
        txn.Status.Should().Be(TransactionStatus.Completed);
    }

    [Fact]
    public async Task ProcessPaymentAsync_AlreadyCompleted_Throws()
    {
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Completed,
            Payments = new List<Payment>()
        };

        _txnRepo.Setup(r => r.GetByIdAsync(txn.Id)).ReturnsAsync(txn);

        await FluentActions
            .Invoking(() => _sut.ProcessPaymentAsync(txn.Id, 100, PaymentMethod.Cash))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Transaction already completed");
    }

    [Fact]
    public async Task ProcessPaymentAsync_InsufficientStock_Throws()
    {
        var line = new TransactionLine
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ProductName = "OutOfStock",
            Quantity = 50,
            UnitPrice = 10,
            TaxRate = 0,
            NetAmount = 500
        };
        var txn = new Transaction
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            TerminalId = Guid.NewGuid(),
            Status = TransactionStatus.Active,
            SubTotal = 500,
            GrandTotal = 500,
            Lines = new List<TransactionLine> { line },
            Payments = new List<Payment>()
        };

        _txnRepo.Setup(r => r.GetByIdAsync(txn.Id)).ReturnsAsync(txn);

        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = txn.StoreId,
            ProductId = line.ProductId,
            OnHand = 10,
            Reserved = 0,
            Available = 10
        };
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(txn.StoreId, line.ProductId))
            .ReturnsAsync(stock);

        await FluentActions
            .Invoking(() => _sut.ProcessPaymentAsync(txn.Id, 500, PaymentMethod.Cash))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }
}

using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;
using ShoppingMall.Data.Repositories;

namespace ShoppingMall.Tests;

public class PosServiceTests
{
    private readonly ShoppingMall.Data.DbContext.ShoppingMallDbContext _db;
    private readonly ITransactionRepository _txnRepo;
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
        _db = TestDatabaseFactory.CreateInMemoryDbContext();
        _txnRepo = new TransactionRepository(_db);
        _inventory = new InventoryService(_stockRepo.Object, _ledgerRepo.Object, _productRepo.Object);
        _sut = new PosService(
            _db, _txnRepo, _productRepo.Object, _stockRepo.Object,
            _paymentRepo.Object, _taxRepo.Object, _gst, _inventory, _customerRepo.Object);
    }

    [Fact]
    public async Task CreateTransactionAsync_GeneratesReceiptAndIdempotencyKey()
    {
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await _sut.CreateTransactionAsync(storeId, terminalId, userId);

        result.StoreId.Should().Be(storeId);
        result.TerminalId.Should().Be(terminalId);
        result.UserId.Should().Be(userId);
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

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var txn = await _sut.CreateTransactionAsync(Guid.NewGuid(), Guid.NewGuid(), null);
        var result = await _sut.AddLineItemAsync(txn.Id, productId, 2);

        result.ProductId.Should().Be(productId);
        result.ProductName.Should().Be("Test Product");
        result.Quantity.Should().Be(2);
        result.UnitPrice.Should().Be(100);

        await _db.Entry(txn).ReloadAsync();
        txn.SubTotal.Should().Be(200);
        txn.TaxTotal.Should().Be(36);
        txn.GrandTotal.Should().Be(236);
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

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var txn = await _sut.CreateTransactionAsync(Guid.NewGuid(), Guid.NewGuid(), null);
        var result = await _sut.AddLineItemAsync(txn.Id, productId, 1, 150);

        result.UnitPrice.Should().Be(150);

        await _db.Entry(txn).ReloadAsync();
        txn.SubTotal.Should().Be(150);
    }

    [Fact]
    public async Task ProcessPaymentAsync_CompletesTransaction()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Item",
            SKU = "TST-002",
            SellingPrice = 50,
            TaxRate = 0
        };

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            ProductId = productId,
            OnHand = 100,
            Reserved = 0,
            Available = 100
        };
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(It.IsAny<Guid>(), productId))
            .ReturnsAsync(stock);

        var txn = await _sut.CreateTransactionAsync(stock.StoreId, Guid.NewGuid(), null);
        await _sut.AddLineItemAsync(txn.Id, productId, 2);

        var result = await _sut.ProcessPaymentAsync(txn.Id, 100, PaymentMethod.Cash, 100);

        result.Amount.Should().Be(100);
        result.Method.Should().Be(PaymentMethod.Cash);
        result.ChangeAmount.Should().Be(0);

        await _db.Entry(txn).ReloadAsync();
        txn.Status.Should().Be(TransactionStatus.Completed);
    }

    [Fact]
    public async Task ProcessPaymentAsync_AlreadyCompleted_Throws()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Item",
            SKU = "TST-003",
            SellingPrice = 10,
            TaxRate = 0
        };

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            ProductId = productId,
            OnHand = 100,
            Reserved = 0,
            Available = 100
        };
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(It.IsAny<Guid>(), productId))
            .ReturnsAsync(stock);

        var txn = await _sut.CreateTransactionAsync(stock.StoreId, Guid.NewGuid(), null);
        await _sut.AddLineItemAsync(txn.Id, productId, 1);
        await _sut.ProcessPaymentAsync(txn.Id, 10, PaymentMethod.Cash);

        await FluentActions
            .Invoking(() => _sut.ProcessPaymentAsync(txn.Id, 10, PaymentMethod.Cash))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Transaction already completed");
    }

    [Fact]
    public async Task ProcessPaymentAsync_InsufficientStock_Throws()
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "OutOfStock",
            SKU = "TST-004",
            SellingPrice = 10,
            TaxRate = 0
        };

        _productRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            ProductId = productId,
            OnHand = 10,
            Reserved = 0,
            Available = 10
        };
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(It.IsAny<Guid>(), productId))
            .ReturnsAsync(stock);

        var txn = await _sut.CreateTransactionAsync(stock.StoreId, Guid.NewGuid(), null);
        await _sut.AddLineItemAsync(txn.Id, productId, 50);

        await FluentActions
            .Invoking(() => _sut.ProcessPaymentAsync(txn.Id, 500, PaymentMethod.Cash))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }
}

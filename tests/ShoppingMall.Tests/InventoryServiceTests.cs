using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class InventoryServiceTests
{
    private readonly Mock<ICurrentStockRepository> _stockRepo = new();
    private readonly Mock<IRepository<StockLedger>> _ledgerRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _sut = new InventoryService(_stockRepo.Object, _ledgerRepo.Object, _productRepo.Object);
    }

    [Fact]
    public async Task AddStockAsync_NewProduct_CreatesStockRecord()
    {
        var storeId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(storeId, productId))
            .ReturnsAsync((CurrentStock?)null);

        await _sut.AddStockAsync(storeId, productId, 100, "GRN", Guid.NewGuid(), 10);

        _stockRepo.Verify(r => r.AddAsync(It.Is<CurrentStock>(s =>
            s.OnHand == 100 && s.Available == 100)), Times.Once);
        _ledgerRepo.Verify(r => r.AddAsync(It.Is<StockLedger>(l =>
            l.Quantity == 100)), Times.Once);
    }

    [Fact]
    public async Task AddStockAsync_ExistingProduct_UpdatesOnHand()
    {
        var storeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var existing = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            ProductId = productId,
            OnHand = 50,
            Reserved = 10,
            Available = 40
        };

        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(storeId, productId))
            .ReturnsAsync(existing);

        await _sut.AddStockAsync(storeId, productId, 30, "GRN", Guid.NewGuid(), 10);

        existing.OnHand.Should().Be(80);
        existing.Available.Should().Be(70);
        _stockRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task DeductStockAsync_SufficientStock_Reduces()
    {
        var storeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            ProductId = productId,
            OnHand = 100,
            Reserved = 20,
            Available = 80
        };

        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(storeId, productId))
            .ReturnsAsync(stock);
        _ledgerRepo.Setup(r => r.AddAsync(It.IsAny<StockLedger>()))
            .ReturnsAsync((StockLedger l) => l);

        await _sut.DeductStockAsync(storeId, productId, 30, "SALE", Guid.NewGuid());

        stock.OnHand.Should().Be(70);
        stock.Available.Should().Be(80);
    }

    [Fact]
    public async Task DeductStockAsync_InsufficientStock_Throws()
    {
        var storeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            ProductId = productId,
            OnHand = 10,
            Reserved = 0,
            Available = 10
        };

        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(storeId, productId))
            .ReturnsAsync(stock);

        await FluentActions
            .Invoking(() => _sut.DeductStockAsync(storeId, productId, 20, "SALE", Guid.NewGuid()))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient stock");
    }

    [Fact]
    public async Task DeductStockAsync_NoStockRecord_Throws()
    {
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((CurrentStock?)null);

        await FluentActions
            .Invoking(() => _sut.DeductStockAsync(Guid.NewGuid(), Guid.NewGuid(), 1, "SALE", Guid.NewGuid()))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Stock record not found");
    }

    [Fact]
    public async Task GetStockAsync_DelegatesToRepo()
    {
        var storeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var expected = new CurrentStock { Id = Guid.NewGuid(), StoreId = storeId, ProductId = productId };

        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(storeId, productId))
            .ReturnsAsync(expected);

        var result = await _sut.GetStockAsync(storeId, productId);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetLowStockAsync_DelegatesToRepo()
    {
        var storeId = Guid.NewGuid();
        var expected = new[] { new CurrentStock { Id = Guid.NewGuid(), StoreId = storeId, Available = 5 } };

        _stockRepo.Setup(r => r.GetLowStockItemsAsync(storeId))
            .ReturnsAsync(expected);

        var result = await _sut.GetLowStockAsync(storeId);

        result.Should().BeEquivalentTo(expected);
    }
}

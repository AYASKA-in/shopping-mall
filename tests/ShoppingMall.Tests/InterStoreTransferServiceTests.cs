using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class InterStoreTransferServiceTests
{
    private readonly Mock<IRepository<InterStoreTransfer>> _transferRepo = new();
    private readonly Mock<IRepository<InterStoreTransferLine>> _lineRepo = new();
    private readonly Mock<ICurrentStockRepository> _stockRepo = new();
    private readonly Mock<IRepository<StockLedger>> _ledgerRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly InterStoreTransferService _sut;

    public InterStoreTransferServiceTests()
    {
        var inventory = new InventoryService(_stockRepo.Object, _ledgerRepo.Object, _productRepo.Object);
        _sut = new InterStoreTransferService(_transferRepo.Object, _lineRepo.Object, inventory);
    }

    [Fact]
    public async Task CreateAsync_CreatesDraftTransfer()
    {
        var fromStore = Guid.NewGuid();
        var toStore = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var lines = new List<CreateTransferLine> { new(productId, 10) };

        _transferRepo.Setup(r => r.AddAsync(It.IsAny<InterStoreTransfer>()))
            .ReturnsAsync((InterStoreTransfer t) => t);

        var result = await _sut.CreateAsync(fromStore, toStore, userId, lines, "Test transfer");

        result.TransferNumber.Should().StartWith("TFR-");
        result.FromStoreId.Should().Be(fromStore);
        result.ToStoreId.Should().Be(toStore);
        result.Status.Should().Be(TransferStatus.Draft);
        result.Notes.Should().Be("Test transfer");
        result.Lines.Should().HaveCount(1);
        result.Lines.First().ProductId.Should().Be(productId);
        result.Lines.First().RequestedQty.Should().Be(10);
    }

    [Fact]
    public async Task ShipAsync_DraftTransfer_MovesToShipped()
    {
        var transferId = Guid.NewGuid();
        var fromStore = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var line = new InterStoreTransferLine
        {
            Id = Guid.NewGuid(),
            TransferId = transferId,
            ProductId = productId,
            RequestedQty = 5,
            ShippedQty = 0
        };
        var transfer = new InterStoreTransfer
        {
            Id = transferId,
            FromStoreId = fromStore,
            Status = TransferStatus.Draft,
            Lines = new List<InterStoreTransferLine> { line }
        };

        var stock = new CurrentStock
        {
            Id = Guid.NewGuid(),
            StoreId = fromStore,
            ProductId = productId,
            OnHand = 100,
            Available = 100
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transferId)).ReturnsAsync(transfer);
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(fromStore, productId)).ReturnsAsync(stock);
        _ledgerRepo.Setup(r => r.AddAsync(It.IsAny<StockLedger>()))
            .ReturnsAsync((StockLedger l) => l);

        await _sut.ShipAsync(transferId);

        line.ShippedQty.Should().Be(5);
        transfer.Status.Should().Be(TransferStatus.Shipped);
        transfer.ShippedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShipAsync_AlreadyReceived_Throws()
    {
        var transfer = new InterStoreTransfer
        {
            Id = Guid.NewGuid(),
            Status = TransferStatus.Received
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transfer.Id)).ReturnsAsync(transfer);

        await FluentActions
            .Invoking(() => _sut.ShipAsync(transfer.Id))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot ship*");
    }

    [Fact]
    public async Task ReceiveAsync_ShippedTransfer_ReceivesStock()
    {
        var transferId = Guid.NewGuid();
        var toStore = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var line = new InterStoreTransferLine
        {
            Id = Guid.NewGuid(),
            TransferId = transferId,
            ProductId = productId,
            RequestedQty = 5,
            ShippedQty = 5,
            ReceivedQty = 0
        };
        var transfer = new InterStoreTransfer
        {
            Id = transferId,
            ToStoreId = toStore,
            Status = TransferStatus.Shipped,
            Lines = new List<InterStoreTransferLine> { line }
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transferId)).ReturnsAsync(transfer);

        await _sut.ReceiveAsync(transferId);

        line.ReceivedQty.Should().Be(5);
        transfer.Status.Should().Be(TransferStatus.Received);
        transfer.ReceivedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ReceiveAsync_DraftTransfer_Throws()
    {
        var transfer = new InterStoreTransfer
        {
            Id = Guid.NewGuid(),
            Status = TransferStatus.Draft
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transfer.Id)).ReturnsAsync(transfer);

        await FluentActions
            .Invoking(() => _sut.ReceiveAsync(transfer.Id))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot receive*");
    }

    [Fact]
    public async Task CancelAsync_DraftTransfer_CancelsWithoutStockRestore()
    {
        var transfer = new InterStoreTransfer
        {
            Id = Guid.NewGuid(),
            Status = TransferStatus.Draft,
            Lines = new List<InterStoreTransferLine>()
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transfer.Id)).ReturnsAsync(transfer);

        await _sut.CancelAsync(transfer.Id);

        transfer.Status.Should().Be(TransferStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_ShippedTransfer_RestoresStock()
    {
        var transferId = Guid.NewGuid();
        var fromStore = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var line = new InterStoreTransferLine
        {
            Id = Guid.NewGuid(),
            TransferId = transferId,
            ProductId = productId,
            RequestedQty = 5,
            ShippedQty = 5,
            ReceivedQty = 0
        };
        var transfer = new InterStoreTransfer
        {
            Id = transferId,
            FromStoreId = fromStore,
            Status = TransferStatus.Shipped,
            Lines = new List<InterStoreTransferLine> { line }
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transferId)).ReturnsAsync(transfer);

        await _sut.CancelAsync(transferId);

        transfer.Status.Should().Be(TransferStatus.Cancelled);
    }

    [Fact]
    public async Task CancelAsync_ReceivedTransfer_Throws()
    {
        var transfer = new InterStoreTransfer
        {
            Id = Guid.NewGuid(),
            Status = TransferStatus.Received
        };

        _transferRepo.Setup(r => r.GetByIdAsync(transfer.Id)).ReturnsAsync(transfer);

        await FluentActions
            .Invoking(() => _sut.CancelAsync(transfer.Id))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot cancel*");
    }

    [Fact]
    public async Task GetByStoreAsync_ReturnsMatchingTransfers()
    {
        var storeId = Guid.NewGuid();
        var transfers = new[]
        {
            new InterStoreTransfer { Id = Guid.NewGuid(), FromStoreId = storeId, Status = TransferStatus.Draft },
            new InterStoreTransfer { Id = Guid.NewGuid(), ToStoreId = storeId, Status = TransferStatus.Shipped }
        };

        _transferRepo.Setup(r => r.FindAsync(t => t.FromStoreId == storeId || t.ToStoreId == storeId))
            .ReturnsAsync(transfers);

        var result = await _sut.GetByStoreAsync(storeId);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShipAsync_PartialShipment_ShipsOnlySpecified()
    {
        var transferId = Guid.NewGuid();
        var fromStore = Guid.NewGuid();
        var productA = Guid.NewGuid();
        var productB = Guid.NewGuid();
        var lineA = new InterStoreTransferLine { Id = Guid.NewGuid(), TransferId = transferId, ProductId = productA, RequestedQty = 10 };
        var lineB = new InterStoreTransferLine { Id = Guid.NewGuid(), TransferId = transferId, ProductId = productB, RequestedQty = 20 };
        var transfer = new InterStoreTransfer
        {
            Id = transferId,
            FromStoreId = fromStore,
            Status = TransferStatus.Draft,
            Lines = new List<InterStoreTransferLine> { lineA, lineB }
        };

        var stockA = new CurrentStock { Id = Guid.NewGuid(), StoreId = fromStore, ProductId = productA, OnHand = 100, Available = 100 };
        var stockB = new CurrentStock { Id = Guid.NewGuid(), StoreId = fromStore, ProductId = productB, OnHand = 100, Available = 100 };

        _transferRepo.Setup(r => r.GetByIdAsync(transferId)).ReturnsAsync(transfer);
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(fromStore, productA)).ReturnsAsync(stockA);
        _stockRepo.Setup(r => r.GetByStoreAndProductAsync(fromStore, productB)).ReturnsAsync(stockB);
        _ledgerRepo.Setup(r => r.AddAsync(It.IsAny<StockLedger>()))
            .ReturnsAsync((StockLedger l) => l);

        var shipments = new List<ShipLineUpdate> { new(productA, 5) };
        await _sut.ShipAsync(transferId, shipments);

        lineA.ShippedQty.Should().Be(5);
        lineB.ShippedQty.Should().Be(20);
    }

    [Fact]
    public async Task ShipAsync_NotFound_ReturnsNull()
    {
        _transferRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((InterStoreTransfer?)null);

        var result = await _sut.ShipAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByStoreAsync_NoResults_ReturnsEmpty()
    {
        var storeId = Guid.NewGuid();

        _transferRepo.Setup(r => r.FindAsync(t => t.FromStoreId == storeId || t.ToStoreId == storeId))
            .ReturnsAsync(Enumerable.Empty<InterStoreTransfer>());

        var result = await _sut.GetByStoreAsync(storeId);

        result.Should().BeEmpty();
    }
}

using ShoppingMall.Client.Offline;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class OfflineCacheTests : IDisposable
{
    private readonly string _dbPath;
    private readonly OfflineCache _cache;

    public OfflineCacheTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"offline_test_{Guid.NewGuid()}.db");
        _cache = new OfflineCache(_dbPath);
    }

    [Fact]
    public async Task QueueTransaction_AddsToPending()
    {
        await _cache.QueueTransactionAsync(@"{""Type"":""AddLineItem"",""TransactionId"":""00000000-0000-0000-0000-000000000001""}");
        var count = await _cache.GetPendingCountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task QueueMultiple_ReturnsAll()
    {
        await _cache.QueueTransactionAsync(@"{""Type"":""AddLineItem"",""ProductId"":""a""}");
        await _cache.QueueTransactionAsync(@"{""Type"":""Payment"",""Amount"":100}");
        await _cache.QueueTransactionAsync(@"{""Type"":""Suspend"",""ItemCount"":5}");

        var pending = await _cache.GetPendingTransactionsAsync();
        Assert.Equal(3, pending.Count);
    }

    [Fact]
    public async Task MarkSynced_RemovesFromPending()
    {
        var payload = @"{""Type"":""Payment"",""Amount"":50}";
        await _cache.QueueTransactionAsync(payload);
        Assert.Equal(1, await _cache.GetPendingCountAsync());

        await _cache.MarkSyncedAsync(payload);
        Assert.Equal(0, await _cache.GetPendingCountAsync());
    }

    [Fact]
    public async Task CacheProduct_DoesNotAffectPendingCount()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            SKU = "TST-001",
            SellingPrice = 100
        };
        await _cache.CacheProductAsync(product);

        var pending = await _cache.GetPendingCountAsync();
        Assert.Equal(0, pending);
    }

    [Fact]
    public async Task QueueTransaction_WithStoreAndTerminal()
    {
        await _cache.QueueTransactionAsync(@"{""Type"":""AddLineItem""}", "store-1", "term-1");
        var pending = await _cache.GetPendingTransactionsAsync();
        Assert.Single(pending);
    }

    [Fact]
    public async Task MultipleMarkSynced_OnlyAffectsTarget()
    {
        var p1 = @"{""Type"":""Keep""}";
        var p2 = @"{""Type"":""Remove""}";
        await _cache.QueueTransactionAsync(p1);
        await _cache.QueueTransactionAsync(p2);
        Assert.Equal(2, await _cache.GetPendingCountAsync());

        await _cache.MarkSyncedAsync(p2);
        Assert.Equal(1, await _cache.GetPendingCountAsync());

        var pending = await _cache.GetPendingTransactionsAsync();
        Assert.Single(pending);
    }

    public void Dispose()
    {
        _cache.Dispose();
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }
}

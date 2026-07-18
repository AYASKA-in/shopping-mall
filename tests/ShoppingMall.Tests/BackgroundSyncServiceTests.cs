using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ShoppingMall.Client.Offline;
using ShoppingMall.Client.Services;

namespace ShoppingMall.Tests;

public class BackgroundSyncServiceTests : IDisposable
{
    private readonly string _dbPath;
    private readonly OfflineCache _offline;
    private readonly Client.Services.ApiClient _api;
    private readonly BackgroundSyncService _sync;
    private readonly HttpListener _listener;
    private readonly Thread _listenerThread;
    private readonly string _baseUrl = "http://localhost:19876";
    private readonly string _realConfigPath;
    private readonly string? _savedConfig;

    public BackgroundSyncServiceTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"sync_test_{Guid.NewGuid()}.db");
        _offline = new OfflineCache(_dbPath);

        _listener = new HttpListener();
        _listener.Prefixes.Add($"{_baseUrl}/");
        _listener.Start();
        _listenerThread = new Thread(HandleRequests);
        _listenerThread.Start();

        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ServerUrl"] = _baseUrl,
                ["TerminalId"] = Guid.NewGuid().ToString(),
                ["StoreId"] = Guid.NewGuid().ToString(),
                ["TerminalName"] = "Test-POS",
                ["AutoConnect"] = "true"
            }!)
            .Build();

        var httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        _api = new Client.Services.ApiClient(httpClient, configBuilder);

        // Save real config, write test config
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ShoppingMall");
        Directory.CreateDirectory(appData);
        _realConfigPath = Path.Combine(appData, "config.json");
        _savedConfig = File.Exists(_realConfigPath) ? File.ReadAllText(_realConfigPath) : null;
        File.WriteAllText(_realConfigPath, JsonSerializer.Serialize(new
        {
            ServerUrl = _baseUrl,
            TerminalId = Guid.NewGuid().ToString(),
            StoreId = Guid.NewGuid().ToString(),
            IsConfigured = true
        }));

        _sync = new BackgroundSyncService(_offline, _api, new AppConfiguration());
    }

    private void HandleRequests()
    {
        try
        {
            while (_listener.IsListening)
            {
                var ctx = _listener.GetContext();
                var body = new StreamReader(ctx.Request.InputStream).ReadToEnd();
                var resp = ctx.Response;
                resp.StatusCode = 200;
                var buffer = System.Text.Encoding.UTF8.GetBytes("{}");
                resp.OutputStream.Write(buffer, 0, buffer.Length);
                resp.Close();
            }
        }
        catch { }
    }

    [Fact]
    public async Task CheckConnectivityAsync_ServerResponds_IsOnlineTrue()
    {
        await _sync.CheckConnectivityAsync();
        Assert.True(_sync.IsOnline);
    }

    [Fact]
    public async Task CheckConnectivityAsync_ServerDown_IsOnlineFalse()
    {
        _listener.Stop();
        await Task.Delay(100);
        await _sync.CheckConnectivityAsync();
        Assert.False(_sync.IsOnline);
        _listener.Start();
    }

    [Fact]
    public async Task ConnectivityChanged_FiresOnStatusChange()
    {
        var changed = false;
        var connected = false;
        _sync.ConnectivityChanged += (_, isOnline) => { changed = true; connected = isOnline; };

        // Should fire when going from unknown to online
        await _sync.CheckConnectivityAsync();
        Assert.True(changed);
        Assert.True(connected);
    }

    [Fact]
    public async Task QueueAndSync_QueuedTransactionsPersisted()
    {
        var p1 = JsonSerializer.Serialize(new { Type = "Payment", TransactionId = Guid.NewGuid().ToString(), Amount = 100m, Method = "Cash", TenderedAmount = 100m });
        var p2 = JsonSerializer.Serialize(new { Type = "Suspend", TransactionId = Guid.NewGuid().ToString(), BasketData = "[]", GrandTotal = 50m, ItemCount = 3 });

        await _offline.QueueTransactionAsync(p1);
        await _offline.QueueTransactionAsync(p2);

        Assert.Equal(2, await _offline.GetPendingCountAsync());

        await _offline.MarkSyncedAsync(p1);
        Assert.Equal(1, await _offline.GetPendingCountAsync());

        await _offline.MarkSyncedAsync(p2);
        Assert.Equal(0, await _offline.GetPendingCountAsync());
    }

    public void Dispose()
    {
        _sync.Dispose();
        _offline.Dispose();
        _api.ClearSessionId();
        _listener.Stop();
        _listener.Close();

        // Restore real config
        try
        {
            if (_savedConfig != null)
                File.WriteAllText(_realConfigPath, _savedConfig);
            else if (File.Exists(_realConfigPath))
                File.Delete(_realConfigPath);
        }
        catch { }

        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }
}

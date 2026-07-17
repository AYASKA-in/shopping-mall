using System.Text.Json;
using ShoppingMall.Client.Offline;

namespace ShoppingMall.Client.Services;

public class BackgroundSyncService : IDisposable
{
    private readonly OfflineCache _offline;
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;
    private Timer? _timer;
    private bool _isSyncing;
    private bool _isOnline;

    public event EventHandler<bool>? ConnectivityChanged;
    public event EventHandler<int>? SyncProgress;

    public bool IsOnline
    {
        get => _isOnline;
        private set
        {
            if (_isOnline != value)
            {
                _isOnline = value;
                ConnectivityChanged?.Invoke(this, value);
            }
        }
    }

    public BackgroundSyncService(OfflineCache offline, ApiClient api, AppConfiguration config)
    {
        _offline = offline;
        _api = api;
        _config = config;
    }

    public void Start()
    {
        _timer = new Timer(async _ => await TrySyncAsync(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public async Task CheckConnectivityAsync()
    {
        try
        {
            var cfg = _config.Load();
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var response = await http.GetAsync($"{cfg.ServerUrl}/api/admin/health");
            IsOnline = response.IsSuccessStatusCode;
        }
        catch
        {
            IsOnline = false;
        }
    }

    private async Task SendHeartbeatAsync()
    {
        try
        {
            await _api.SendHeartbeatAsync(_config.Load().TerminalId);
        }
        catch { }
    }

    private async Task TrySyncAsync()
    {
        if (_isSyncing) return;
        _isSyncing = true;

        try
        {
            await CheckConnectivityAsync();
            if (!IsOnline) return;

            await SendHeartbeatAsync();

            var pending = await _offline.GetPendingTransactionsAsync();
            if (pending.Count == 0) return;

            var synced = 0;
            foreach (var payload in pending)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<JsonElement>(payload);
                    var type = data.GetProperty("Type").GetString();

                    switch (type)
                    {
                        case "AddLineItem":
                            var txnId = Guid.Parse(data.GetProperty("TransactionId").GetString()!);
                            var prodId = Guid.Parse(data.GetProperty("ProductId").GetString()!);
                            var qty = data.GetProperty("Quantity").GetDecimal();
                            await _api.AddLineItemAsync(txnId, prodId, qty);
                            break;

                        case "Payment":
                            var paymentTxnId = Guid.Parse(data.GetProperty("TransactionId").GetString()!);
                            var amount = data.GetProperty("Amount").GetDecimal();
                            var method = data.GetProperty("Method").GetString()!;
                            var tender = data.GetProperty("TenderedAmount").GetDecimal();
                            await _api.ProcessPaymentAsync(paymentTxnId, amount, method, tender);
                            break;

                        case "Suspend":
                            var suspendTxnId = Guid.Parse(data.GetProperty("TransactionId").GetString()!);
                            var basket = data.GetProperty("BasketData").GetString()!;
                            var total = data.GetProperty("GrandTotal").GetDecimal();
                            var count = data.GetProperty("ItemCount").GetInt32();
                            await _api.SuspendTransactionAsync(suspendTxnId, basket, total, count);
                            break;
                    }

                    await _offline.MarkSyncedAsync(payload);
                    synced++;
                }
                catch
                {
                    break;
                }
            }

            if (synced > 0)
                SyncProgress?.Invoke(this, synced);
        }
        finally
        {
            _isSyncing = false;
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

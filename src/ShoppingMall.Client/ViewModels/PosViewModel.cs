using System.Collections.ObjectModel;
using System.Windows.Input;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.ViewModels;

public class PosViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;
    private readonly CartService _cart;
    private readonly ThermalPrinterService _printer;
    private readonly Offline.OfflineCache _offline;
    private readonly Hardware.ScaleReader _scale;
    private ClientConfig _cfg = new();

    private Transaction? _currentTransaction;
    public Transaction? CurrentTransaction
    {
        get => _currentTransaction;
        set => SetProperty(ref _currentTransaction, value);
    }

    public ObservableCollection<CartLineItem> CartItems => _cart.Lines;

    public decimal SubTotal => _cart.SubTotal;
    public decimal DiscountTotal => _cart.DiscountTotal;
    public decimal TaxTotal => _cart.TaxTotal;
    public decimal GrandTotal => _cart.GrandTotal;
    public decimal TotalAmount => GrandTotal;
    public bool HasItems => _cart.HasItems;

    private string _barcodeInput = "";
    public string BarcodeInput
    {
        get => _barcodeInput;
        set
        {
            if (SetProperty(ref _barcodeInput, value) && value.Length >= 4)
                FireAndForget(() => LookupBarcodeAsync(value));
        }
    }

    private string _selectedPayment = "Cash";
    public string SelectedPayment
    {
        get => _selectedPayment;
        set
        {
            SetProperty(ref _selectedPayment, value);
            OnPropertyChanged(nameof(IsUpiPayment));
            if (value == "UPI")
                FireAndForget(GenerateUpiQrAsync);
        }
    }

    public bool IsUpiPayment => SelectedPayment == "UPI";

    private string _upiQrIntent = "";
    public string UpiQrIntent
    {
        get => _upiQrIntent;
        set => SetProperty(ref _upiQrIntent, value);
    }

    private decimal _scaleWeight;
    public decimal ScaleWeight
    {
        get => _scaleWeight;
        set => SetProperty(ref _scaleWeight, value);
    }

    private string _scaleStatus = "Not connected";
    public string ScaleStatus
    {
        get => _scaleStatus;
        set => SetProperty(ref _scaleStatus, value);
    }

    private decimal _tenderedAmount;
    public decimal TenderedAmount
    {
        get => _tenderedAmount;
        set => SetProperty(ref _tenderedAmount, value);
    }

    private decimal _changeAmount;
    public decimal ChangeAmount
    {
        get => _changeAmount;
        set => SetProperty(ref _changeAmount, value);
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    private bool _isOffline;
    public bool IsOffline
    {
        get => _isOffline;
        set => SetProperty(ref _isOffline, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _couponCode = "";
    public string CouponCode
    {
        get => _couponCode;
        set => SetProperty(ref _couponCode, value);
    }

    private string _couponStatus = "";
    public string CouponStatus
    {
        get => _couponStatus;
        set => SetProperty(ref _couponStatus, value);
    }

    private string? _loyaltyPhone;
    public string? LoyaltyPhone
    {
        get => _loyaltyPhone;
        set => SetProperty(ref _loyaltyPhone, value);
    }

    private string _loyaltyInfo = "";
    public string LoyaltyInfo
    {
        get => _loyaltyInfo;
        set => SetProperty(ref _loyaltyInfo, value);
    }

    private bool _isPromotionLoading;
    public bool IsPromotionLoading
    {
        get => _isPromotionLoading;
        set => SetProperty(ref _isPromotionLoading, value);
    }

    private string _promotionInfo = "";
    public string PromotionInfo
    {
        get => _promotionInfo;
        set => SetProperty(ref _promotionInfo, value);
    }

    private decimal _promotionDiscount;
    public decimal PromotionDiscount
    {
        get => _promotionDiscount;
        set => SetProperty(ref _promotionDiscount, value);
    }

    public string[] PaymentMethods { get; } = ["Cash", "Card", "UPI", "Wallet", "GiftCard", "StoreCredit"];

    public ICommand NewTransactionCommand { get; }
    public ICommand ScanBarcodeCommand { get; }
    public ICommand IncrementQtyCommand { get; }
    public ICommand DecrementQtyCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand ApplyDiscountCommand { get; }
    public ICommand PayCommand { get; }
    public ICommand QuickAmountCommand { get; }
    public ICommand SuspendCommand { get; }
    public ICommand RecallCommand { get; }
    public ICommand ReprintCommand { get; }
    public ICommand ApplyCouponCommand { get; }
    public ICommand LookupLoyaltyCommand { get; }
    public ICommand ConnectScaleCommand { get; }
    public ICommand CaptureWeightCommand { get; }
    public ICommand TareScaleCommand { get; }

    public PosViewModel(ApiClient api, AppConfiguration config, CartService cart, ThermalPrinterService printer, Offline.OfflineCache offline, Hardware.ScaleReader scale)
    {
        _api = api;
        _config = config;
        _cart = cart;
        _printer = printer;
        _offline = offline;
        _scale = scale;

        _cfg = _config.Load();
        _cart.Lines.CollectionChanged += (_, _) => RefreshTotals();

        NewTransactionCommand = new AsyncRelayCommand(async _ => await CreateNewTransactionAsync());
        ScanBarcodeCommand = new AsyncRelayCommand(async _ => await LookupBarcodeAsync(BarcodeInput));
        IncrementQtyCommand = new RelayCommand(item => AdjustQuantity((CartLineItem)item!, 1));
        DecrementQtyCommand = new RelayCommand(item => AdjustQuantity((CartLineItem)item!, -1));
        RemoveItemCommand = new RelayCommand(item => _cart.RemoveItem((CartLineItem)item!));
        ApplyDiscountCommand = new AsyncRelayCommand(async _ => await ShowDiscountDialogAsync());
        PayCommand = new AsyncRelayCommand(async _ => await ProcessPaymentAsync());
        QuickAmountCommand = new RelayCommand(amount =>
        {
            if (amount is string s)
            {
                if (s == "Exact") TenderedAmount = GrandTotal;
                else if (decimal.TryParse(s, out var d)) TenderedAmount = d;
            }
            else if (amount is decimal d)
            {
                TenderedAmount = d;
            }
        });
        SuspendCommand = new AsyncRelayCommand(async _ => await SuspendTransactionAsync());
        RecallCommand = new AsyncRelayCommand(async _ => await RecallTransactionAsync());
        ReprintCommand = new AsyncRelayCommand(async _ => await ReprintLastReceiptAsync());
        ApplyCouponCommand = new AsyncRelayCommand(async _ => await ApplyCouponAsync());
        LookupLoyaltyCommand = new AsyncRelayCommand(async _ => await LookupLoyaltyAsync());
        ConnectScaleCommand = new AsyncRelayCommand(async _ => await ConnectScaleAsync());
        CaptureWeightCommand = new AsyncRelayCommand(async _ => CaptureWeightFromScale());
        TareScaleCommand = new RelayCommand(_ => _scale.SendTareCommand());

        _scale.WeightReceived += (_, weight) =>
        {
            ScaleWeight = weight;
            ScaleStatus = $"{weight:F3} kg";
            CaptureWeightFromScale();
        };
        _scale.ErrorOccurred += (_, msg) => ScaleStatus = $"Error: {msg}";
    }

    public async Task CreateNewTransactionAsync()
    {
        try
        {
            var cfg = _config.Load();
            IsLoading = true;

            CurrentTransaction = await _api.CreateTransactionAsync(cfg.StoreId, cfg.TerminalId, null);
            _cart.Clear();
            RefreshTotals();
            if (CurrentTransaction != null)
                StatusText = $"Transaction #{CurrentTransaction.ReceiptNumber}";
            await LoadActivePromotionsAsync();
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            IsOffline = true;
        }
        finally { IsLoading = false; }
    }

    private async Task LoadActivePromotionsAsync()
    {
        IsPromotionLoading = true;
        try
        {
            var promotions = await _api.GetActivePromotionsAsync();
            if (promotions != null && promotions.Count > 0)
            {
                PromotionInfo = $"{promotions.Count} promotion(s) active";
            }
            else
            {
                PromotionInfo = "";
            }
        }
        catch
        {
            PromotionInfo = "";
        }
        finally { IsPromotionLoading = false; }
    }

    private async Task LookupBarcodeAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 4) return;

        StatusText = $"Scanning: {input}...";
        BarcodeInput = "";

        try
        {
            var product = await _api.GetProductByBarcodeAsync(input);
            if (product == null)
            {
                product = await _api.SearchProductsAsync(input).ContinueWith(t =>
                    t.Result?.FirstOrDefault());
            }

            if (product == null)
            {
                StatusText = $"Not found: {input}";
                return;
            }

            if (CurrentTransaction == null)
                await CreateNewTransactionAsync();

            if (CurrentTransaction == null) return;

            _cart.AddItem(product, product.IsWeighable ? 0 : 1);

            if (product.IsWeighable)
            {
                StatusText = $"Weigh {product.Name} — scan weight or enter kg";
                return;
            }

            try
            {
                await _api.AddLineItemAsync(CurrentTransaction.Id, product.Id, 1);
            }
            catch
            {
                if (CurrentTransaction != null)
                {
                _ = _offline.QueueTransactionAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    Type = "AddLineItem",
                    TransactionId = CurrentTransaction.Id,
                    ProductId = product.Id,
                    Quantity = 1
                }), _cfg.StoreId.ToString(), _cfg.TerminalId.ToString());
                }
            }

            StatusText = $"Added: {product.Name}";
        }
        catch (Exception ex)
        {
            StatusText = $"Scan error: {ex.Message}";
            IsOffline = true;
        }
    }

    private void AdjustQuantity(CartLineItem line, int delta)
    {
        var newQty = line.Quantity + delta;
        if (newQty <= 0) { _cart.RemoveItem(line); return; }
        _cart.UpdateQuantity(line, newQty);
    }

    private async Task ShowDiscountDialogAsync()
    {
        var line = CartItems.FirstOrDefault();
        if (line == null) return;
        _cart.ApplyLineDiscountPercent(line, 10);
        StatusText = "10% discount applied";
        await Task.CompletedTask;
    }

    private async Task ApplyCouponAsync()
    {
        if (string.IsNullOrWhiteSpace(CouponCode))
        {
            CouponStatus = "Enter a coupon code";
            return;
        }

        CouponStatus = "Validating...";
        try
        {
            var result = await _api.HttpPostAsync<CouponValidationResult>(
                "/api/promotions/coupons/validate",
                new { Code = CouponCode, CartTotal = GrandTotal });

            if (result != null && result.IsValid)
            {
                CouponStatus = $"Coupon applied: {result.CampaignName}";
            }
            else
            {
                CouponStatus = result?.Error ?? "Invalid coupon";
            }
        }
        catch
        {
            CouponStatus = "Validation failed";
        }
    }

    private async Task LookupLoyaltyAsync()
    {
        if (string.IsNullOrWhiteSpace(LoyaltyPhone))
        {
            LoyaltyInfo = "Enter phone number";
            return;
        }

        LoyaltyInfo = "Searching...";
        try
        {
            var result = await _api.LookupLoyaltyAsync(LoyaltyPhone);
            if (result != null)
            {
                LoyaltyInfo = $"{result.CustomerName} | Tier: {result.Tier} | Points: {result.PointsBalance} (₹{result.RedeemableValue})";
            }
            else
            {
                LoyaltyInfo = "Customer not found";
            }
        }
        catch
        {
            LoyaltyInfo = "Lookup failed";
        }
    }

    private async Task ProcessPaymentAsync()
    {
        if (CurrentTransaction == null || !_cart.HasItems)
        {
            StatusText = "No items in cart";
            return;
        }

        IsLoading = true;
        try
        {
            var cfg = _config.Load();
            var method = Enum.Parse<PaymentMethod>(SelectedPayment);
            var tender = TenderedAmount > 0 ? TenderedAmount : GrandTotal;

            try
            {
                await _api.ProcessPaymentAsync(CurrentTransaction.Id, GrandTotal, method.ToString(), tender);
            }
            catch
            {
                _ = _offline.QueueTransactionAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    Type = "Payment",
                    TransactionId = CurrentTransaction.Id,
                    Amount = GrandTotal,
                    Method = SelectedPayment,
                    TenderedAmount = tender
                }), _cfg.StoreId.ToString(), _cfg.TerminalId.ToString());
                IsOffline = true;
            }

            ChangeAmount = tender - GrandTotal;
            if (ChangeAmount < 0) ChangeAmount = 0;

            try
            {
                await _printer.PrintReceiptAsync(CurrentTransaction, new Store
                {
                    Name = "Shopping Mart",
                    GSTIN = cfg.StoreId.ToString(),
                    ReceiptFooter = "Thank you! Visit again!"
                }, "Cashier");
            }
            catch (Exception ex)
            {
                StatusText = $"Payment OK. Print failed: {ex.Message}";
            }

            var txn = CurrentTransaction;
            CurrentTransaction = null;
            _cart.Clear();
            CouponCode = "";
            CouponStatus = "";
            PromotionInfo = "";
            PromotionDiscount = 0;
            RefreshTotals();
            StatusText = $"₹{txn.GrandTotal:N2} Paid. Change: ₹{ChangeAmount:F2}";
        }
        catch (Exception ex)
        {
            StatusText = $"Payment error: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    private async Task SuspendTransactionAsync()
    {
        if (!_cart.HasItems || CurrentTransaction == null) return;

        var cfg = _config.Load();
        var data = _cart.Serialize();

        try
        {
            await _api.SuspendTransactionAsync(CurrentTransaction.Id, data, _cart.GrandTotal, _cart.ItemCount);
        }
        catch
        {
            await _offline.QueueTransactionAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                Type = "Suspend",
                TransactionId = CurrentTransaction.Id,
                BasketData = data,
                GrandTotal,
                ItemCount = _cart.ItemCount
            }), _cfg.StoreId.ToString(), _cfg.TerminalId.ToString());
        }

        CurrentTransaction = null;
        _cart.Clear();
        CouponCode = "";
        CouponStatus = "";
        PromotionInfo = "";
        PromotionDiscount = 0;
        RefreshTotals();
        StatusText = "Transaction suspended";
    }

    private async Task RecallTransactionAsync()
    {
        try
        {
            var cfg = _config.Load();
            var suspended = await _api.GetSuspendedTransactionsAsync(cfg.StoreId);
            if (!suspended.Any())
            {
                StatusText = "No suspended transactions";
                return;
            }

            var first = suspended.First();
            var lines = System.Text.Json.JsonSerializer.Deserialize<List<CartLineItem>>(first.BasketData);
            if (lines != null)
            {
                await _api.HttpPostAsync<object>($"/api/pos/transactions/{first.Id}/recall", new { });
                _cart.Clear();
                foreach (var line in lines)
                    _cart.Lines.Add(line);
                RefreshTotals();
                StatusText = $"Recalled: {first.ItemCount} items, ₹{first.BasketTotal:F2}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Recall error: {ex.Message}";
        }
        await Task.CompletedTask;
    }

    private async Task ReprintLastReceiptAsync()
    {
        try
        {
            var cfg = _config.Load();
            var todayTxns = await _api.GetTodayTransactionsAsync(cfg.StoreId);
            var last = todayTxns.LastOrDefault();
            if (last == null) { StatusText = "No transactions today"; return; }

            await _printer.PrintReceiptAsync(last, new Store
            {
                Name = "Shopping Mart",
                GSTIN = cfg.StoreId.ToString(),
                ReceiptFooter = "Thank you! Visit again!"
            }, "Cashier");
            StatusText = "Receipt reprinted";
        }
        catch
        {
            StatusText = "Reprint failed - check printer";
        }
    }

    private async Task ConnectScaleAsync()
    {
        var ports = _scale.GetAvailablePorts();
        if (ports.Length == 0)
        {
            ScaleStatus = "No COM ports found";
            return;
        }

        var connected = _scale.Connect(ports[0]);
        ScaleStatus = connected ? $"Connected to {ports[0]}" : "Connection failed";
        if (connected)
            await Task.Delay(100);
    }

    private void CaptureWeightFromScale()
    {
        if (ScaleWeight <= 0) return;

        var weighableItem = CartItems.FirstOrDefault(i => i.IsWeighable && i.Quantity == 0);
        if (weighableItem != null)
        {
            weighableItem.WeightKg = ScaleWeight;
            _cart.UpdateQuantity(weighableItem, ScaleWeight);
            StatusText = $"Weight: {ScaleWeight:F3} kg for {weighableItem.ProductName}";
            ScaleWeight = 0;
        }
    }

    private async Task GenerateUpiQrAsync()
    {
        if (CurrentTransaction == null || GrandTotal <= 0) return;

        try
        {
            var cfg = _config.Load();
            var response = await _api.HttpPostAsync<UpiQrResponse>("/api/upi/qr/dynamic", new
            {
                Amount = GrandTotal,
                TransactionRef = CurrentTransaction.ReceiptNumber,
                StoreName = "Shopping Mart"
            });

            UpiQrIntent = response?.Intent ?? "";
            if (!string.IsNullOrEmpty(UpiQrIntent))
                StatusText = "UPI QR ready — scan to pay";
        }
        catch
        {
            StatusText = "UPI QR generation failed";
        }
    }

    private void RefreshTotals()
    {
        OnPropertyChanged(nameof(SubTotal));
        OnPropertyChanged(nameof(DiscountTotal));
        OnPropertyChanged(nameof(TaxTotal));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(TotalAmount));
        OnPropertyChanged(nameof(CartItems));
    }
}

public record UpiQrResponse(string Intent, decimal Amount, string TransactionRef);

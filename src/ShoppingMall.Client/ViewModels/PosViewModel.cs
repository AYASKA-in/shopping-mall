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

    private string _barcodeInput = "";
    public string BarcodeInput
    {
        get => _barcodeInput;
        set
        {
            if (SetProperty(ref _barcodeInput, value) && value.Length >= 4)
                _ = LookupBarcodeAsync(value);
        }
    }

    private string _selectedPayment = "Cash";
    public string SelectedPayment
    {
        get => _selectedPayment;
        set => SetProperty(ref _selectedPayment, value);
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

    public PosViewModel(ApiClient api, AppConfiguration config, CartService cart, ThermalPrinterService printer, Offline.OfflineCache offline)
    {
        _api = api;
        _config = config;
        _cart = cart;
        _printer = printer;
        _offline = offline;

        _cfg = _config.Load();
        _cart.Lines.CollectionChanged += (_, _) => RefreshTotals();

        NewTransactionCommand = new RelayCommand(async _ => await CreateNewTransactionAsync());
        ScanBarcodeCommand = new RelayCommand(async _ => await LookupBarcodeAsync(BarcodeInput));
        IncrementQtyCommand = new RelayCommand(item => AdjustQuantity((CartLineItem)item!, 1));
        DecrementQtyCommand = new RelayCommand(item => AdjustQuantity((CartLineItem)item!, -1));
        RemoveItemCommand = new RelayCommand(item => _cart.RemoveItem((CartLineItem)item!));
        ApplyDiscountCommand = new RelayCommand(async _ => await ShowDiscountDialogAsync());
        PayCommand = new RelayCommand(async _ => await ProcessPaymentAsync());
        QuickAmountCommand = new RelayCommand(amount => TenderedAmount = (decimal)amount);
        SuspendCommand = new RelayCommand(async _ => await SuspendTransactionAsync());
        RecallCommand = new RelayCommand(async _ => await RecallTransactionAsync());
        ReprintCommand = new RelayCommand(async _ => await ReprintLastReceiptAsync());
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
            StatusText = $"Transaction #{CurrentTransaction.ReceiptNumber}";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            IsOffline = true;
        }
        finally { IsLoading = false; }
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
                _offline.QueueTransactionAsync(System.Text.Json.JsonSerializer.Serialize(new
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
                _offline.QueueTransactionAsync(System.Text.Json.JsonSerializer.Serialize(new
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

    private void RefreshTotals()
    {
        OnPropertyChanged(nameof(SubTotal));
        OnPropertyChanged(nameof(DiscountTotal));
        OnPropertyChanged(nameof(TaxTotal));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(CartItems));
    }
}

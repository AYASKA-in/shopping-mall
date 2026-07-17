using System.Collections.ObjectModel;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.ViewModels;

public class PosViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private Transaction? _currentTransaction;
    public Transaction? CurrentTransaction
    {
        get => _currentTransaction;
        set => SetProperty(ref _currentTransaction, value);
    }

    public ObservableCollection<TransactionLine> CartItems { get; } = new();

    private string _barcodeInput = "";
    public string BarcodeInput
    {
        get => _barcodeInput;
        set
        {
            if (SetProperty(ref _barcodeInput, value) && value.Length >= 8)
                _ = LookupBarcodeAsync(value);
        }
    }

    private decimal _totalAmount;
    public decimal TotalAmount
    {
        get => _totalAmount;
        set => SetProperty(ref _totalAmount, value);
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

    public ICommand NewTransactionCommand { get; }
    public ICommand ScanBarcodeCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand PayCashCommand { get; }
    public ICommand SuspendCommand { get; }
    public ICommand RecallCommand { get; }

    public PosViewModel(ApiClient api, AppConfiguration config)
    {
        _api = api;
        _config = config;

        NewTransactionCommand = new RelayCommand(async _ => await CreateNewTransactionAsync());
        ScanBarcodeCommand = new RelayCommand(async _ => await LookupBarcodeAsync(BarcodeInput));
        RemoveItemCommand = new RelayCommand(async item => await RemoveItemAsync((TransactionLine)item!));
        PayCashCommand = new RelayCommand(async _ => await ProcessCashPaymentAsync());
        SuspendCommand = new RelayCommand(async _ => await SuspendTransactionAsync());
        RecallCommand = new RelayCommand(async _ => await RecallTransactionAsync());
    }

    public async Task CreateNewTransactionAsync()
    {
        try
        {
            var config = _config.Load();
            CurrentTransaction = await _api.CreateTransactionAsync(config.StoreId, config.TerminalId, null);
            CartItems.Clear();
            TotalAmount = 0;
            StatusText = $"Transaction #{CurrentTransaction.ReceiptNumber}";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    private async Task LookupBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode) || barcode.Length < 8) return;

        StatusText = $"Scanning: {barcode}...";
        var product = await _api.GetProductByBarcodeAsync(barcode);

        if (product == null)
        {
            StatusText = $"Product not found: {barcode}";
            BarcodeInput = "";
            return;
        }

        if (CurrentTransaction == null)
            await CreateNewTransactionAsync();

        try
        {
            var line = await _api.AddLineItemAsync(CurrentTransaction!.Id, product.Id, 1);
            if (line != null)
            {
                CartItems.Add(line);
                TotalAmount = CartItems.Sum(i => i.NetAmount);
                StatusText = $"Added: {product.Name}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }

        BarcodeInput = "";
    }

    private async Task RemoveItemAsync(TransactionLine line)
    {
        CartItems.Remove(line);
        TotalAmount = CartItems.Sum(i => i.NetAmount);
        StatusText = "Item removed";
        await Task.CompletedTask;
    }

    private async Task ProcessCashPaymentAsync()
    {
        if (CurrentTransaction == null || CartItems.Count == 0) return;

        try
        {
            await _api.ProcessPaymentAsync(CurrentTransaction.Id, TotalAmount, "Cash", TenderedAmount > 0 ? TenderedAmount : TotalAmount);
            ChangeAmount = (TenderedAmount > 0 ? TenderedAmount : TotalAmount) - TotalAmount;
            StatusText = $"Payment complete. Change: ₹{ChangeAmount:F2}";
            CurrentTransaction = null;
            CartItems.Clear();
            TotalAmount = 0;
        }
        catch (Exception ex)
        {
            StatusText = $"Payment error: {ex.Message}";
        }
    }

    private async Task SuspendTransactionAsync()
    {
        StatusText = "Transaction suspended";
        CurrentTransaction = null;
        CartItems.Clear();
        await Task.CompletedTask;
    }

    private async Task RecallTransactionAsync()
    {
        StatusText = "Recall - select a suspended transaction";
        await Task.CompletedTask;
    }
}

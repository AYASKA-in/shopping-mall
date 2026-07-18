using System.Collections.ObjectModel;
using System.Windows.Input;
using ShoppingMall.Client.Services;

namespace ShoppingMall.Client.ViewModels;

public class InventoryViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FireAndForget(SearchAsync);
        }
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<StockDisplayItem> StockItems { get; } = new();

    public ICommand RefreshCommand { get; }

    public InventoryViewModel(ApiClient api, AppConfiguration config)
    {
        _api = api;
        _config = config;
        RefreshCommand = new AsyncRelayCommand(async _ => await LoadStockAsync());
    }

    public async Task LoadStockAsync()
    {
        IsLoading = true;
        StatusText = "Loading...";
        try
        {
            var cfg = _config.Load();
            var stocks = await _api.HttpGetAsync<List<StockDisplayItem>>($"/api/reports/stock/{cfg.StoreId}/summary");
            StockItems.Clear();
            if (stocks != null)
                foreach (var s in stocks) StockItems.Add(s);
            StatusText = $"Stock loaded: {StockItems.Count} items";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadStockAsync();
            return;
        }

        var filtered = StockItems.Where(s =>
            s.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            (s.SKU?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

        StockItems.Clear();
        foreach (var s in filtered) StockItems.Add(s);
    }
}

public class StockDisplayItem
{
    public string ProductName { get; set; } = "";
    public string? SKU { get; set; }
    public decimal OnHand { get; set; }
    public decimal Reserved { get; set; }
    public decimal Available { get; set; }
    public string StockStatus => Available <= 0 ? "Out of Stock" : Available <= 10 ? "Low Stock" : "In Stock";
}

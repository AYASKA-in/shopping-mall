using System.Collections.ObjectModel;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.ViewModels;

public class ProductListViewModel : BaseViewModel
{
    private readonly ApiClient _api;

    public ObservableCollection<Product> Products { get; } = new();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            _ = SearchAsync();
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private Product? _selectedProduct;
    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }

    public ProductListViewModel(ApiClient api)
    {
        _api = api;
        RefreshCommand = new RelayCommand(async _ => await LoadAsync());
        SearchCommand = new RelayCommand(async _ => await SearchAsync());
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var products = await _api.SearchProductsAsync("");
            Products.Clear();
            foreach (var p in products) Products.Add(p);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            var products = await _api.SearchProductsAsync(SearchText);
            Products.Clear();
            foreach (var p in products) Products.Add(p);
        }
        finally
        {
            IsLoading = false;
        }
    }
}

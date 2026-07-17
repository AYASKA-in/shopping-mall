using System.Collections.ObjectModel;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.ViewModels;

public class SupplierListViewModel : BaseViewModel
{
    private readonly ApiClient _api;

    public ObservableCollection<Supplier> Suppliers { get; } = new();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private Supplier? _selectedSupplier;
    public Supplier? SelectedSupplier
    {
        get => _selectedSupplier;
        set => SetProperty(ref _selectedSupplier, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }

    public SupplierListViewModel(ApiClient api)
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
            var response = await _api.HttpGetAsync<List<Supplier>>("/api/procurement/suppliers");
            Suppliers.Clear();
            if (response != null)
                foreach (var s in response) Suppliers.Add(s);
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
            var response = await _api.HttpGetAsync<List<Supplier>>($"/api/procurement/suppliers/search?q={SearchText}");
            Suppliers.Clear();
            if (response != null)
                foreach (var s in response) Suppliers.Add(s);
        }
        finally
        {
            IsLoading = false;
        }
    }
}

using System.Collections.ObjectModel;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.ViewModels;

public class CustomerViewModel : BaseViewModel
{
    private readonly ApiClient _api;

    public ObservableCollection<Transaction> PurchaseHistory { get; } = new();

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

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            SetProperty(ref _selectedCustomer, value);
            if (value != null) _ = LoadPurchaseHistoryAsync(value.Id);
        }
    }

    private string _customerInfo = "";
    public string CustomerInfo
    {
        get => _customerInfo;
        set => SetProperty(ref _customerInfo, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand CreateCustomerCommand { get; }

    private string _newFirstName = "";
    public string NewFirstName { get => _newFirstName; set => SetProperty(ref _newFirstName, value); }
    private string _newLastName = "";
    public string NewLastName { get => _newLastName; set => SetProperty(ref _newLastName, value); }
    private string _newPhone = "";
    public string NewPhone { get => _newPhone; set => SetProperty(ref _newPhone, value); }
    private string _newEmail = "";
    public string NewEmail { get => _newEmail; set => SetProperty(ref _newEmail, value); }

    public CustomerViewModel(ApiClient api)
    {
        _api = api;
        SearchCommand = new RelayCommand(async _ => await SearchAsync());
        CreateCustomerCommand = new RelayCommand(async _ => await CreateCustomerAsync());
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText)) return;
        IsLoading = true;
        try
        {
            var customers = await _api.HttpGetAsync<List<Customer>>($"/api/customers?search={SearchText}");
            if (customers != null && customers.Count == 1)
            {
                SelectedCustomer = customers[0];
                CustomerInfo = $"{customers[0].FirstName} {customers[0].LastName} - {customers[0].Phone}";
            }
            else if (customers != null && customers.Count > 1)
            {
                SelectedCustomer = customers[0];
                CustomerInfo = $"Found {customers.Count} customers";
            }
            else
            {
                CustomerInfo = "Customer not found";
                SelectedCustomer = null;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateCustomerAsync()
    {
        if (string.IsNullOrWhiteSpace(NewFirstName) || string.IsNullOrWhiteSpace(NewPhone)) return;
        IsLoading = true;
        try
        {
            var customer = new Customer
            {
                FirstName = NewFirstName,
                LastName = NewLastName,
                Phone = NewPhone,
                Email = string.IsNullOrWhiteSpace(NewEmail) ? null : NewEmail
            };
            var created = await _api.HttpPostAsync<Customer>("/api/customers", customer);
            if (created != null)
            {
                SelectedCustomer = created;
                CustomerInfo = $"{created.FirstName} {created.LastName} - {created.Phone}";
                NewFirstName = ""; NewLastName = ""; NewPhone = ""; NewEmail = "";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadPurchaseHistoryAsync(Guid customerId)
    {
        IsLoading = true;
        try
        {
            var result = await _api.HttpGetAsync<PurchaseHistoryResponse>($"/api/customers/{customerId}/purchases");
            PurchaseHistory.Clear();
            if (result?.Items != null)
                foreach (var tx in result.Items) PurchaseHistory.Add(tx);
        }
        finally
        {
            IsLoading = false;
        }
    }
}

public class PurchaseHistoryResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<Transaction> Items { get; set; } = new();
}

using ShoppingMall.Client.Services;

namespace ShoppingMall.Client.ViewModels;

public enum AppView { Login, Pos, Inventory, Reports, Admin, ProductList, SupplierList, Customer }

public class MainViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private AppView _currentView = AppView.Login;
    public AppView CurrentView
    {
        get => _currentView;
        set
        {
            SetProperty(ref _currentView, value);
            OnPropertyChanged(nameof(IsPosView));
            OnPropertyChanged(nameof(IsLoginView));
            OnPropertyChanged(nameof(IsProductListView));
            OnPropertyChanged(nameof(IsSupplierListView));
            OnPropertyChanged(nameof(IsCustomerView));
            OnPropertyChanged(nameof(IsInventoryView));
            OnPropertyChanged(nameof(IsReportsView));
            OnPropertyChanged(nameof(IsAdminView));
        }
    }

    public bool IsPosView => CurrentView == AppView.Pos;
    public bool IsLoginView => CurrentView == AppView.Login;
    public bool IsProductListView => CurrentView == AppView.ProductList;
    public bool IsSupplierListView => CurrentView == AppView.SupplierList;
    public bool IsCustomerView => CurrentView == AppView.Customer;
    public bool IsInventoryView => CurrentView == AppView.Inventory;
    public bool IsReportsView => CurrentView == AppView.Reports;
    public bool IsAdminView => CurrentView == AppView.Admin;

    private string _userDisplayName = "";
    public string UserDisplayName
    {
        get => _userDisplayName;
        set => SetProperty(ref _userDisplayName, value);
    }

    private string _userRole = "";
    public string UserRole
    {
        get => _userRole;
        set => SetProperty(ref _userRole, value);
    }

    public PosViewModel PosVM { get; }
    public ProductListViewModel ProductListVM { get; }
    public SupplierListViewModel SupplierListVM { get; }
    public CustomerViewModel CustomerVM { get; }
    public ReportsViewModel ReportsVM { get; }
    public AdminViewModel AdminVM { get; }
    public InventoryViewModel InventoryVM { get; }

    public ICommand NavigateToPosCommand { get; }
    public ICommand NavigateToInventoryCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToAdminCommand { get; }
    public ICommand NavigateToProductListCommand { get; }
    public ICommand NavigateToSupplierListCommand { get; }
    public ICommand NavigateToCustomerCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel(ApiClient api, AppConfiguration config, PosViewModel posVM,
        ProductListViewModel productListVM, SupplierListViewModel supplierListVM,
        CustomerViewModel customerVM, ReportsViewModel reportsVM, AdminViewModel adminVM,
        InventoryViewModel inventoryVM)
    {
        _api = api;
        _config = config;
        PosVM = posVM;
        ProductListVM = productListVM;
        SupplierListVM = supplierListVM;
        CustomerVM = customerVM;
        ReportsVM = reportsVM;
        AdminVM = adminVM;
        InventoryVM = inventoryVM;

        NavigateToPosCommand = new RelayCommand(_ => CurrentView = AppView.Pos);
        NavigateToInventoryCommand = new RelayCommand(_ => CurrentView = AppView.Inventory);
        NavigateToReportsCommand = new RelayCommand(_ => CurrentView = AppView.Reports);
        NavigateToAdminCommand = new RelayCommand(_ => CurrentView = AppView.Admin);
        NavigateToProductListCommand = new RelayCommand(_ => CurrentView = AppView.ProductList);
        NavigateToSupplierListCommand = new RelayCommand(_ => CurrentView = AppView.SupplierList);
        NavigateToCustomerCommand = new RelayCommand(_ => CurrentView = AppView.Customer);
        LogoutCommand = new AsyncRelayCommand(async _ =>
        {
            _api.ClearSessionId();
            CurrentView = AppView.Login;
            await Task.CompletedTask;
        });
    }

    public void OnLoginSucceeded(LoginResult result)
    {
        UserDisplayName = result.DisplayName;
        UserRole = result.Role;
        CurrentView = AppView.Pos;
    }
}

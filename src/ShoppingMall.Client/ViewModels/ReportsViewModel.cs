using System.Collections.ObjectModel;
using System.Windows.Input;
using ShoppingMall.Client.Services;

namespace ShoppingMall.Client.ViewModels;

public enum ReportTab { Sales, Products, Gst, Cashier }

public class ReportsViewModel : BaseViewModel
{
    private readonly ApiClient _api;
    private readonly AppConfiguration _config;

    private ReportTab _selectedTab = ReportTab.Sales;
    public ReportTab SelectedTab
    {
        get => _selectedTab;
        set
        {
            SetProperty(ref _selectedTab, value);
            OnPropertyChanged(nameof(IsSalesTab));
            OnPropertyChanged(nameof(IsProductsTab));
            OnPropertyChanged(nameof(IsGstTab));
            OnPropertyChanged(nameof(IsCashierTab));
            FireAndForget(LoadReportAsync);
        }
    }

    public bool IsSalesTab => SelectedTab == ReportTab.Sales;
    public bool IsProductsTab => SelectedTab == ReportTab.Products;
    public bool IsGstTab => SelectedTab == ReportTab.Gst;
    public bool IsCashierTab => SelectedTab == ReportTab.Cashier;

    private DateTime _fromDate = DateTime.Today;
    public DateTime FromDate
    {
        get => _fromDate;
        set { SetProperty(ref _fromDate, value); }
    }

    private DateTime _toDate = DateTime.Today;
    public DateTime ToDate
    {
        get => _toDate;
        set { SetProperty(ref _toDate, value); }
    }

    private string _statusText = "Select a report to view";
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

    private decimal _totalSales;
    public decimal TotalSales
    {
        get => _totalSales;
        set => SetProperty(ref _totalSales, value);
    }

    private int _totalTransactions;
    public int TotalTransactions
    {
        get => _totalTransactions;
        set => SetProperty(ref _totalTransactions, value);
    }

    private decimal _totalTax;
    public decimal TotalTax
    {
        get => _totalTax;
        set => SetProperty(ref _totalTax, value);
    }

    private decimal _totalDiscount;
    public decimal TotalDiscount
    {
        get => _totalDiscount;
        set => SetProperty(ref _totalDiscount, value);
    }

    private decimal _cgst;
    public decimal CGST
    {
        get => _cgst;
        set => SetProperty(ref _cgst, value);
    }

    private decimal _sgst;
    public decimal SGST
    {
        get => _sgst;
        set => SetProperty(ref _sgst, value);
    }

    private decimal _igst;
    public decimal IGST
    {
        get => _igst;
        set => SetProperty(ref _igst, value);
    }

    public ObservableCollection<TerminalSales> TerminalSales { get; } = new();
    public ObservableCollection<CashierSales> CashierSales { get; } = new();
    public ObservableCollection<ProductPerformance> TopSellers { get; } = new();
    public ObservableCollection<CategorySales> CategorySales { get; } = new();
    public ObservableCollection<GstSlab> GstSlabs { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand SelectSalesTabCommand { get; }
    public ICommand SelectProductsTabCommand { get; }
    public ICommand SelectGstTabCommand { get; }
    public ICommand SelectCashierTabCommand { get; }

    public ReportsViewModel(ApiClient api, AppConfiguration config)
    {
        _api = api;
        _config = config;

        RefreshCommand = new AsyncRelayCommand(async _ => await LoadReportAsync());
        SelectSalesTabCommand = new RelayCommand(_ => SelectedTab = ReportTab.Sales);
        SelectProductsTabCommand = new RelayCommand(_ => SelectedTab = ReportTab.Products);
        SelectGstTabCommand = new RelayCommand(_ => SelectedTab = ReportTab.Gst);
        SelectCashierTabCommand = new RelayCommand(_ => SelectedTab = ReportTab.Cashier);
    }

    public async Task LoadReportAsync()
    {
        IsLoading = true;
        StatusText = "Loading...";
        try
        {
            var cfg = _config.Load();

            switch (SelectedTab)
            {
                case ReportTab.Sales:
                    await LoadSalesReportAsync(cfg.StoreId);
                    break;
                case ReportTab.Products:
                    await LoadProductsReportAsync(cfg.StoreId);
                    break;
                case ReportTab.Gst:
                    await LoadGstReportAsync(cfg.StoreId);
                    break;
                case ReportTab.Cashier:
                    await LoadCashierReportAsync(cfg.StoreId);
                    break;
            }

            StatusText = "Report loaded successfully";
        }
        catch (Exception ex)
        {
            StatusText = $"Error loading report: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    private async Task LoadSalesReportAsync(Guid storeId)
    {
        var byTerminal = await _api.GetSalesByTerminalAsync(storeId, FromDate, ToDate);
        var byCashier = await _api.GetSalesByCashierAsync(storeId, FromDate, ToDate);

        TerminalSales.Clear();
        CashierSales.Clear();

        if (byTerminal != null)
        {
            TotalSales = byTerminal.Sum(t => t.TotalSales);
            TotalTransactions = byTerminal.Sum(t => t.TransactionCount);
            TotalTax = byTerminal.Sum(t => t.TotalTax);
            foreach (var t in byTerminal) TerminalSales.Add(t);
        }

        if (byCashier != null)
            foreach (var c in byCashier) CashierSales.Add(c);
    }

    private async Task LoadProductsReportAsync(Guid storeId)
    {
        var topSellers = await _api.GetTopSellersAsync(storeId, FromDate, ToDate);
        var byCategory = await _api.GetSalesByCategoryAsync(storeId, FromDate, ToDate);

        TopSellers.Clear();
        CategorySales.Clear();

        if (topSellers != null)
            foreach (var p in topSellers) TopSellers.Add(p);

        if (byCategory != null)
            foreach (var c in byCategory) CategorySales.Add(c);
    }

    private async Task LoadGstReportAsync(Guid storeId)
    {
        var gst = await _api.GetGstSummaryAsync(storeId, FromDate, ToDate);

        GstSlabs.Clear();

        if (gst != null)
        {
            CGST = gst.CGST;
            SGST = gst.SGST;
            IGST = gst.IGST;
            TotalTax = gst.TotalGST;
            foreach (var s in gst.Slabs) GstSlabs.Add(s);
        }
    }

    private async Task LoadCashierReportAsync(Guid storeId)
    {
        var byCashier = await _api.GetSalesByCashierAsync(storeId, FromDate, ToDate);

        CashierSales.Clear();

        if (byCashier != null)
        {
            foreach (var c in byCashier) CashierSales.Add(c);
            TotalSales = byCashier.Sum(c => c.TotalSales);
            TotalTransactions = byCashier.Sum(c => c.TransactionCount);
        }
    }
}

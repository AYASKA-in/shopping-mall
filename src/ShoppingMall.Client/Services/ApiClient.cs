using System.Net.Http.Json;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public ApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = config.GetValue<string>("ServerUrl") ?? "http://localhost:5194";
        _http.BaseAddress = new Uri(_baseUrl);
    }

    public void SetSessionId(Guid sessionId)
    {
        _http.DefaultRequestHeaders.Remove("X-Session-Id");
        _http.DefaultRequestHeaders.Add("X-Session-Id", sessionId.ToString());
    }

    public void ClearSessionId()
    {
        _http.DefaultRequestHeaders.Remove("X-Session-Id");
    }

    // Auth
    public async Task<LoginResult?> LoginAsync(string username, string pin, Guid terminalId)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { username, pin, terminalId });
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
        if (result != null) SetSessionId(result.SessionId);
        return result;
    }

    public async Task SendHeartbeatAsync(Guid terminalId)
    {
        try
        {
            await _http.PostAsync($"/api/admin/terminals/{terminalId}/heartbeat", null);
        }
        catch { }
    }

    public async Task LogoutAsync(Guid sessionId)
    {
        try { await _http.PostAsJsonAsync("/api/auth/logout", sessionId); } catch { }
        ClearSessionId();
    }

    // POS
    public async Task<Transaction?> CreateTransactionAsync(Guid storeId, Guid terminalId, Guid? userId)
    {
        var response = await _http.PostAsJsonAsync("/api/pos/transactions",
            new { storeId, terminalId, userId });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Transaction>();
    }

    public async Task<TransactionLine?> AddLineItemAsync(Guid transactionId, Guid productId, decimal qty, decimal? overridePrice = null)
    {
        var response = await _http.PostAsJsonAsync($"/api/pos/transactions/{transactionId}/items",
            new { productId, quantity = qty, overridePrice });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TransactionLine>();
    }

    public async Task<Payment?> ProcessPaymentAsync(Guid transactionId, decimal amount, string method, decimal? tenderedAmount = null)
    {
        var response = await _http.PostAsJsonAsync($"/api/pos/transactions/{transactionId}/payments",
            new { amount, method, tenderedAmount });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Payment>();
    }

    public async Task<Transaction?> GetTransactionAsync(Guid id)
        => await _http.GetFromJsonAsync<Transaction>($"/api/pos/transactions/{id}");

    public async Task<Transaction?> GetByReceiptAsync(string receipt)
        => await _http.GetFromJsonAsync<Transaction>($"/api/pos/transactions/receipt/{receipt}");

    // Products
    public async Task<List<Product>> SearchProductsAsync(string query)
    {
        try { return await _http.GetFromJsonAsync<List<Product>>($"/api/products?search={query}") ?? new(); }
        catch { return new(); }
    }

    public async Task<Product?> GetProductByBarcodeAsync(string barcode)
    {
        try { return await _http.GetFromJsonAsync<Product>($"/api/products/barcode/{barcode}"); }
        catch { return null; }
    }

    // Inventory
    public async Task<CurrentStock?> GetStockAsync(Guid storeId, Guid productId)
    {
        try { return await _http.GetFromJsonAsync<CurrentStock>($"/api/inventory/stock/{storeId}/{productId}"); }
        catch { return null; }
    }

    // Reports
    public async Task<SalesSummary?> GetTodaySalesAsync(Guid storeId)
    {
        try { return await _http.GetFromJsonAsync<SalesSummary>($"/api/reports/sales/{storeId}/today"); }
        catch { return null; }
    }

    public async Task<DashboardData?> GetDashboardAsync(Guid storeId)
    {
        try { return await _http.GetFromJsonAsync<DashboardData>($"/api/reports/dashboard/{storeId}"); }
        catch { return null; }
    }

    public async Task<List<TerminalSales>?> GetSalesByTerminalAsync(Guid storeId, DateTime from, DateTime to)
    {
        try { return await _http.GetFromJsonAsync<List<TerminalSales>>($"/api/reports/sales/{storeId}/by-terminal?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"); }
        catch { return null; }
    }

    public async Task<List<CashierSales>?> GetSalesByCashierAsync(Guid storeId, DateTime from, DateTime to)
    {
        try { return await _http.GetFromJsonAsync<List<CashierSales>>($"/api/reports/sales/{storeId}/by-cashier?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"); }
        catch { return null; }
    }

    public async Task<List<ProductPerformance>?> GetTopSellersAsync(Guid storeId, DateTime from, DateTime to, int count = 20)
    {
        try { return await _http.GetFromJsonAsync<List<ProductPerformance>>($"/api/reports/products/{storeId}/top-sellers?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&count={count}"); }
        catch { return null; }
    }

    public async Task<List<CategorySales>?> GetSalesByCategoryAsync(Guid storeId, DateTime from, DateTime to)
    {
        try { return await _http.GetFromJsonAsync<List<CategorySales>>($"/api/reports/products/{storeId}/by-category?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"); }
        catch { return null; }
    }

    public async Task<GstSummary?> GetGstSummaryAsync(Guid storeId, DateTime from, DateTime to)
    {
        try { return await _http.GetFromJsonAsync<GstSummary>($"/api/reports/gst/{storeId}/summary?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"); }
        catch { return null; }
    }

    // Suspend / Recall
    public async Task SuspendTransactionAsync(Guid transactionId, string basketData, decimal basketTotal, int itemCount)
    {
        var response = await _http.PostAsJsonAsync($"/api/pos/transactions/{transactionId}/suspend",
            new { basketData, basketTotal, itemCount });
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<SuspendedTransaction>> GetSuspendedTransactionsAsync(Guid storeId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<SuspendedTransaction>>($"/api/pos/transactions/suspended/{storeId}") ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<Transaction>> GetTodayTransactionsAsync(Guid storeId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Transaction>>($"/api/pos/transactions/store/{storeId}/today") ?? new();
        }
        catch { return new(); }
    }

    // Promotions
    public async Task<List<Promotion>?> GetActivePromotionsAsync()
    {
        try { return await _http.GetFromJsonAsync<List<Promotion>>("/api/promotions/active"); }
        catch { return null; }
    }

    // Loyalty
    public async Task<LoyaltyLookupResult?> LookupLoyaltyAsync(string phone)
    {
        try { return await _http.GetFromJsonAsync<LoyaltyLookupResult>($"/api/loyalty/lookup/{phone}"); }
        catch { return null; }
    }

    // Generic helpers
    public async Task<T?> HttpGetAsync<T>(string url) where T : class
    {
        try { return await _http.GetFromJsonAsync<T>(url); }
        catch { return null; }
    }

    public async Task<T?> HttpPostAsync<T>(string url, object body) where T : class
    {
        try
        {
            var response = await _http.PostAsJsonAsync(url, body);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch { return null; }
    }
}

public record LoginResult(Guid Id, string DisplayName, string Role, Guid? StoreId, Guid SessionId);
public record SalesSummary(int TransactionCount, decimal TotalSales, decimal TotalTax, decimal TotalDiscount, decimal AvgTransactionValue);
public record DashboardData(TodayData Today, AlertData Alerts);
public record TodayData(decimal Sales, int Transactions, decimal Tax, decimal Discounts);
public record AlertData(int LowStockCount, List<LowStockItem> LowStockItems);
public record LowStockItem(Guid ProductId, decimal Available);
public record LoyaltyLookupResult(Guid CustomerId, string CustomerName, string? Phone, string? CardNumber, int PointsBalance, int LifetimePoints, string Tier, decimal RedeemableValue);
public record CouponValidationResult(bool IsValid, string? Error, string? CampaignName, Guid? PromotionId);
public record TerminalSales(Guid TerminalId, string TerminalName, int TransactionCount, decimal TotalSales, decimal TotalTax);
public record CashierSales(Guid UserId, string CashierName, int TransactionCount, decimal TotalSales, int TotalVoids);
public record ProductPerformance(Guid ProductId, string ProductName, decimal TotalQuantity, decimal TotalSales, int TransactionCount);
public record CategorySales(Guid CategoryId, string CategoryName, decimal TotalQuantity, decimal TotalSales, int TransactionCount);
public record GstSlab(string TaxType, decimal TaxRate, decimal TotalTaxableAmount, decimal TotalTaxAmount, int TransactionCount);
public record GstSummary(decimal CGST, decimal SGST, decimal IGST, decimal TotalGST, List<GstSlab> Slabs);

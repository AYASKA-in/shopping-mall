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
        _baseUrl = config.GetValue<string>("ServerUrl") ?? "http://localhost:5000";
        _http.BaseAddress = new Uri(_baseUrl);
    }

    // Auth
    public async Task<LoginResult?> LoginAsync(string username, string pin, Guid terminalId)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { username, pin, terminalId });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<LoginResult>();
    }

    public async Task LogoutAsync(Guid sessionId)
        => await _http.PostAsJsonAsync("/api/auth/logout", sessionId);

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
        => await _http.GetFromJsonAsync<List<Product>>($"/api/products?search={query}") ?? new();

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
}

public record LoginResult(Guid Id, string DisplayName, string Role, Guid? StoreId, Guid SessionId);
public record SalesSummary(int TransactionCount, decimal TotalSales, decimal TotalTax, decimal TotalDiscount, decimal AvgTransactionValue);
public record DashboardData(TodayData Today, AlertData Alerts);
public record TodayData(decimal Sales, int Transactions, decimal Tax, decimal Discounts);
public record AlertData(int LowStockCount, List<LowStockItem> LowStockItems);
public record LowStockItem(Guid ProductId, decimal Available);

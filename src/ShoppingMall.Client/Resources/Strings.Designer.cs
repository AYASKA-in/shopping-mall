#nullable disable
namespace ShoppingMall.Client.Resources;

internal static class Strings
{
    private static readonly System.Resources.ResourceManager _manager =
        new("ShoppingMall.Client.Resources.Strings", typeof(Strings).Assembly);

    public static string? AppTitle => _manager.GetString("AppTitle");
    public static string? POS => _manager.GetString("POS");
    public static string? NewSale => _manager.GetString("NewSale");
    public static string? ScanBarcode => _manager.GetString("ScanBarcode");
    public static string? Coupon => _manager.GetString("Coupon");
    public static string? Apply => _manager.GetString("Apply");
    public static string? Loyalty => _manager.GetString("Loyalty");
    public static string? Lookup => _manager.GetString("Lookup");
    public static string? Tendered => _manager.GetString("Tendered");
    public static string? Change => _manager.GetString("Change");
    public static string? Total => _manager.GetString("Total");
    public static string? Discount => _manager.GetString("Discount");
    public static string? Tax => _manager.GetString("Tax");
    public static string? Suspend => _manager.GetString("Suspend");
    public static string? PayCash => _manager.GetString("PayCash");
    public static string? Receipt => _manager.GetString("Receipt");
    public static string? QuickActions => _manager.GetString("QuickActions");
    public static string? Reports => _manager.GetString("Reports");
    public static string? Admin => _manager.GetString("Admin");
    public static string? Products => _manager.GetString("Products");
    public static string? Suppliers => _manager.GetString("Suppliers");
    public static string? Customers => _manager.GetString("Customers");
    public static string? Inventory => _manager.GetString("Inventory");
    public static string? Logout => _manager.GetString("Logout");
    public static string? Ready => _manager.GetString("Ready");
    public static string? Search => _manager.GetString("Search");
    public static string? Print => _manager.GetString("Print");
    public static string? Save => _manager.GetString("Save");
    public static string? Cancel => _manager.GetString("Cancel");
    public static string? Confirm => _manager.GetString("Confirm");

    public static void SetCulture(string cultureCode)
    {
        System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(cultureCode);
    }
}

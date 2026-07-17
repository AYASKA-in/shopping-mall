namespace ShoppingMall.Core.Models;

public class BulkImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ImportRow
{
    public string? SKU { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CategoryName { get; set; }
    public string? BrandName { get; set; }
    public string? UOMName { get; set; }
    public string? HSNCode { get; set; }
    public decimal? Mrp { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Barcode { get; set; }
    public string? PLUCode { get; set; }
    public bool IsWeighable { get; set; }
    public int RowNumber { get; set; }
}

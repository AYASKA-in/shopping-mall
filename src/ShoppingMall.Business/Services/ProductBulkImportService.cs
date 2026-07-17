using System.Data;
using System.Globalization;
using System.Text;
using ExcelDataReader;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class ProductBulkImportService
{
    private readonly IProductRepository _productRepo;
    private readonly IRepository<Category> _categoryRepo;
    private readonly IRepository<Brand> _brandRepo;
    private readonly IRepository<UOM> _uomRepo;
    private readonly IRepository<HSN> _hsnRepo;
    private readonly IRepository<Barcode> _barcodeRepo;

    public ProductBulkImportService(
        IProductRepository productRepo,
        IRepository<Category> categoryRepo,
        IRepository<Brand> brandRepo,
        IRepository<UOM> uomRepo,
        IRepository<HSN> hsnRepo,
        IRepository<Barcode> barcodeRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
        _brandRepo = brandRepo;
        _uomRepo = uomRepo;
        _hsnRepo = hsnRepo;
        _barcodeRepo = barcodeRepo;
    }

    public async Task<BulkImportResult> ImportFromStreamAsync(Stream stream, string fileName)
    {
        var result = new BulkImportResult();
        var rows = new List<ImportRow>();

        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                using var csvReader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration
                {
                    FallbackEncoding = Encoding.UTF8
                });
                rows = ReadRows(csvReader).ToList();
            }
            else
            {
                using var excelReader = ExcelReaderFactory.CreateReader(stream);
                rows = ReadRows(excelReader).ToList();
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Failed to read file: {ex.Message}");
            return result;
        }

        result.TotalRows = rows.Count;

        var categories = await _categoryRepo.GetAllAsync();
        var brands = await _brandRepo.GetAllAsync();
        var uoms = await _uomRepo.GetAllAsync();
        var hsns = await _hsnRepo.GetAllAsync();

        foreach (var row in rows)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.SKU))
                {
                    result.Errors.Add($"Row {row.RowNumber}: SKU is required");
                    result.ErrorCount++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row.Name))
                {
                    result.Errors.Add($"Row {row.RowNumber}: Product name is required");
                    result.ErrorCount++;
                    continue;
                }

                var existing = await _productRepo.GetBySKUAsync(row.SKU);
                if (existing != null)
                {
                    result.Errors.Add($"Row {row.RowNumber}: SKU '{row.SKU}' already exists");
                    result.ErrorCount++;
                    continue;
                }

                var category = ResolveCategory(row.CategoryName, categories);
                var brand = ResolveBrand(row.BrandName, brands);
                var uom = ResolveUOM(row.UOMName, uoms);
                var hsn = ResolveHSN(row.HSNCode, hsns);

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    SKU = row.SKU,
                    Name = row.Name,
                    Description = row.Description,
                    CategoryId = category?.Id ?? categories.First().Id,
                    BrandId = brand?.Id,
                    BaseUOMId = uom?.Id ?? uoms.First().Id,
                    HSNId = hsn?.Id,
                    HSNCode = hsn?.Code ?? row.HSNCode,
                    TaxRate = hsn != null ? hsn.CGSTRate + hsn.SGSTRate : row.TaxRate ?? 0,
                    Mrp = row.Mrp,
                    SellingPrice = row.SellingPrice,
                    PurchasePrice = row.PurchasePrice,
                    IsWeighable = row.IsWeighable,
                    PLUCode = row.PLUCode,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _productRepo.AddAsync(product);

                if (!string.IsNullOrWhiteSpace(row.Barcode))
                {
                    var barcode = new Barcode
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Code = row.Barcode,
                        Type = "EAN-13",
                        IsDefault = true
                    };
                    await _barcodeRepo.AddAsync(barcode);
                }

                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Row {row.RowNumber}: {ex.Message}");
                result.ErrorCount++;
            }
        }

        return result;
    }

    private static IEnumerable<ImportRow> ReadRows(IExcelDataReader reader)
    {
        int rowNum = 0;
        var headers = new Dictionary<string, int>();

        while (reader.Read())
        {
            rowNum++;
            if (rowNum == 1)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var h = reader.GetValue(i)?.ToString()?.Trim().ToLower();
                    if (!string.IsNullOrEmpty(h))
                        headers[h] = i;
                }
                continue;
            }

            var row = new ImportRow { RowNumber = rowNum };
            row.SKU = GetString(reader, headers, "sku");
            row.Name = GetString(reader, headers, "name");
            row.Description = GetString(reader, headers, "description");
            row.CategoryName = GetString(reader, headers, "category");
            row.BrandName = GetString(reader, headers, "brand");
            row.UOMName = GetString(reader, headers, "uom");
            row.HSNCode = GetString(reader, headers, "hsncode") ?? GetString(reader, headers, "hsn_code") ?? GetString(reader, headers, "hsn code");
            row.Mrp = GetDecimal(reader, headers, "mrp");
            row.SellingPrice = GetDecimal(reader, headers, "sellingprice") ?? GetDecimal(reader, headers, "selling_price") ?? GetDecimal(reader, headers, "selling price");
            row.PurchasePrice = GetDecimal(reader, headers, "purchaseprice") ?? GetDecimal(reader, headers, "purchase_price") ?? GetDecimal(reader, headers, "purchase price");
            row.TaxRate = GetDecimal(reader, headers, "taxrate") ?? GetDecimal(reader, headers, "tax_rate") ?? GetDecimal(reader, headers, "tax rate");
            row.Barcode = GetString(reader, headers, "barcode");
            row.PLUCode = GetString(reader, headers, "plucode") ?? GetString(reader, headers, "plu_code") ?? GetString(reader, headers, "plu code") ?? GetString(reader, headers, "plu");
            row.IsWeighable = GetBool(reader, headers, "isweighable") ?? GetBool(reader, headers, "weighable") ?? false;

            yield return row;
        }
    }

    private static string? GetString(IExcelDataReader reader, Dictionary<string, int> headers, string key)
    {
        if (!headers.TryGetValue(key, out var idx)) return null;
        var val = reader.GetValue(idx);
        return val?.ToString()?.Trim();
    }

    private static decimal? GetDecimal(IExcelDataReader reader, Dictionary<string, int> headers, string key)
    {
        if (!headers.TryGetValue(key, out var idx)) return null;
        var val = reader.GetValue(idx);
        if (val == null) return null;
        if (decimal.TryParse(val.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return d;
        return null;
    }

    private static bool? GetBool(IExcelDataReader reader, Dictionary<string, int> headers, string key)
    {
        if (!headers.TryGetValue(key, out var idx)) return null;
        var val = reader.GetValue(idx);
        if (val == null) return null;
        if (bool.TryParse(val.ToString(), out var b)) return b;
        if (val.ToString() == "1" || val.ToString()?.ToLower() == "yes" || val.ToString()?.ToLower() == "y") return true;
        return false;
    }

    private static Category? ResolveCategory(string? name, IEnumerable<Category> categories)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return categories.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private static Brand? ResolveBrand(string? name, IEnumerable<Brand> brands)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return brands.FirstOrDefault(b =>
            b.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private static UOM? ResolveUOM(string? name, IEnumerable<UOM> uoms)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        return uoms.FirstOrDefault(u =>
            u.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            u.Abbreviation.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    private static HSN? ResolveHSN(string? code, IEnumerable<HSN> hsns)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return hsns.FirstOrDefault(h => h.Code == code);
    }
}

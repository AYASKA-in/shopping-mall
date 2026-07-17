using System.Text.RegularExpressions;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public partial class BarcodeService
{
    private readonly IProductRepository _productRepo;

    public BarcodeService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<BarcodeResult?> LookupAsync(string input)
    {
        var cleaned = input.Trim();

        var weightBarcode = TryParseWeightBarcode(cleaned);
        if (weightBarcode != null) return weightBarcode;

        var pluMatch = PluRegex().Match(cleaned);
        if (pluMatch.Success)
        {
            var plu = pluMatch.Value;
            var product = await _productRepo.GetByPLUAsync(plu);
            if (product != null)
                return new BarcodeResult(product, 1, null, plu);
        }

        var productByBarcode = await _productRepo.GetByBarcodeAsync(cleaned);
        if (productByBarcode != null)
            return new BarcodeResult(productByBarcode, 1, null, cleaned);

        var productBySku = await _productRepo.GetBySKUAsync(cleaned);
        if (productBySku != null)
            return new BarcodeResult(productBySku, 1, null, cleaned);

        return null;
    }

    private BarcodeResult? TryParseWeightBarcode(string code)
    {
        if (code.Length < 13) return null;

        if (code.StartsWith("02") || code.StartsWith("2"))
        {
            var priceStr = code[^5..];
            if (int.TryParse(priceStr, out var priceCents))
            {
                var weightStr = code[7..12];
                if (int.TryParse(weightStr, out var weightGrams))
                {
                    var productCode = code[2..7];
                    return new BarcodeResult(null!, 0, new WeightBarcodeData
                    {
                        ProductCode = productCode,
                        WeightGrams = weightGrams,
                        PriceCents = priceCents,
                        RawBarcode = code
                    }, code);
                }
            }
        }

        return null;
    }

    public static string NormalizeBarcode(string input)
    {
        var cleaned = input.Trim();
        if (cleaned.Length == 12 && cleaned.All(char.IsDigit))
            cleaned = cleaned.PadLeft(13, '0');
        return cleaned;
    }

    [GeneratedRegex(@"^\d{4,5}$")]
    private static partial Regex PluRegex();
}

public class BarcodeResult
{
    public Product? Product { get; }
    public decimal Quantity { get; }
    public WeightBarcodeData? WeightData { get; }
    public string RawInput { get; }

    public BarcodeResult(Product? product, decimal quantity, WeightBarcodeData? weightData, string rawInput)
    {
        Product = product;
        Quantity = quantity;
        WeightData = weightData;
        RawInput = rawInput;
    }
}

public class WeightBarcodeData
{
    public string ProductCode { get; set; } = "";
    public int WeightGrams { get; set; }
    public decimal WeightKg => WeightGrams / 1000m;
    public int PriceCents { get; set; }
    public decimal Price => PriceCents / 100m;
    public string RawBarcode { get; set; } = "";
}

using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class BarcodeServiceTests
{
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly BarcodeService _sut;

    public BarcodeServiceTests()
    {
        _sut = new BarcodeService(_productRepo.Object);
    }

    [Fact]
    public void NormalizeBarcode_12Digits_PadsTo13()
    {
        var result = BarcodeService.NormalizeBarcode("123456789012");
        result.Should().Be("0123456789012");
    }

    [Fact]
    public void NormalizeBarcode_13Digits_ReturnsAsIs()
    {
        var result = BarcodeService.NormalizeBarcode("8901234567890");
        result.Should().Be("8901234567890");
    }

    [Fact]
    public void NormalizeBarcode_WithSpaces_Trims()
    {
        var result = BarcodeService.NormalizeBarcode("  123456789012  ");
        result.Should().Be("0123456789012");
    }

    [Fact]
    public void NormalizeBarcode_NonDigits_DoesNotPad()
    {
        var result = BarcodeService.NormalizeBarcode("123-456-7890");
        result.Should().Be("123-456-7890");
    }

    [Fact]
    public async Task LookupAsync_ByBarcode_FindsProduct()
    {
        var barcode = "8901234567890";
        var product = new Product { Id = Guid.NewGuid(), Name = "Test", SKU = "TST-001" };

        _productRepo.Setup(r => r.GetByBarcodeAsync(barcode)).ReturnsAsync(product);

        var result = await _sut.LookupAsync(barcode);

        result.Should().NotBeNull();
        result!.Product.Should().Be(product);
        result.Quantity.Should().Be(1);
        result.RawInput.Should().Be(barcode);
    }

    [Fact]
    public async Task LookupAsync_ByPLU_FindsProduct()
    {
        var product = new Product { Id = Guid.NewGuid(), Name = "PLU Item", SKU = "PLU-001" };

        _productRepo.Setup(r => r.GetByPLUAsync("4011")).ReturnsAsync(product);

        var result = await _sut.LookupAsync("4011");

        result.Should().NotBeNull();
        result!.Product.Should().Be(product);
        result.RawInput.Should().Be("4011");
    }

    [Fact]
    public async Task LookupAsync_BySKU_FindsProduct()
    {
        var sku = "TST-SKU-001";
        var product = new Product { Id = Guid.NewGuid(), Name = "SKU Item" };

        _productRepo.Setup(r => r.GetBySKUAsync(sku)).ReturnsAsync(product);

        var result = await _sut.LookupAsync(sku);

        result.Should().NotBeNull();
        result!.Product.Should().Be(product);
    }

    [Fact]
    public async Task LookupAsync_WeightBarcode_ReturnsWeightData()
    {
        // Barcode format: 2 + flag + product (5) + weight(5) + price(5)
        // code[2..7]=productCode, code[7..12]=weightGrams, [^5..]=priceCents
        var code = "2401234000100";
        //   positions:   0123456789012
        //   values:      2401234000100
        //   productCode = "01234"
        //   weightGrams = 10 (chars 7-11 = "00010")
        //   priceCents  = 100 (chars 8-12 = "00100")

        var result = await _sut.LookupAsync(code);

        result.Should().NotBeNull();
        result!.WeightData.Should().NotBeNull();
        result.WeightData!.ProductCode.Should().Be("01234");
        result.WeightData.WeightGrams.Should().Be(10);
        result.WeightData.PriceCents.Should().Be(100);
        result.WeightData.Price.Should().Be(1.00m);
    }

    [Fact]
    public async Task LookupAsync_NotFound_ReturnsNull()
    {
        var result = await _sut.LookupAsync("NOSUCHBARCODE");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LookupAsync_EmptyInput_ReturnsNull()
    {
        var result = await _sut.LookupAsync("");

        result.Should().BeNull();
    }
}

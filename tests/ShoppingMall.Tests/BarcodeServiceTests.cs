using FluentAssertions;
using ShoppingMall.Business.Services;

namespace ShoppingMall.Tests;

public class BarcodeServiceTests
{
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
}

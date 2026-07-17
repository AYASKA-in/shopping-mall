using FluentAssertions;
using ShoppingMall.Business.Services;

namespace ShoppingMall.Tests;

public class PdfExportServiceTests
{
    private readonly PdfExportService _sut = new();

    [Fact]
    public void GenerateSalesReport_ReturnsValidPdfBytes()
    {
        var sales = new List<SalesReportRow>
        {
            new("RCP-001", "Cashier1", 100.50m, DateTime.UtcNow),
            new("RCP-002", "Cashier2", 200.75m, DateTime.UtcNow)
        };

        var result = _sut.GenerateSalesReport(sales, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, "Test Store");

        result.Should().NotBeNullOrEmpty();
        // PDF files start with %PDF
        result[0].Should().Be((byte)'%');
        result[1].Should().Be((byte)'P');
        result[2].Should().Be((byte)'D');
        result[3].Should().Be((byte)'F');
    }

    [Fact]
    public void GenerateSalesReport_EmptySales_ReturnsValidPdf()
    {
        var result = _sut.GenerateSalesReport(
            Enumerable.Empty<SalesReportRow>(),
            DateTime.UtcNow.AddDays(-7),
            DateTime.UtcNow,
            "Empty Store");

        result.Should().NotBeNullOrEmpty();
        result[0].Should().Be((byte)'%');
    }

    [Fact]
    public void GenerateGstReport_ReturnsValidPdfBytes()
    {
        var slabs = new List<GstReportRow>
        {
            new("CGST", 9m, 1000m, 90m, 5),
            new("SGST", 9m, 1000m, 90m, 5)
        };

        var result = _sut.GenerateGstReport(slabs,
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, "Test Store",
            90m, 90m, 0m, 180m);

        result.Should().NotBeNullOrEmpty();
        result[0].Should().Be((byte)'%');
    }

    [Fact]
    public void GenerateGstReport_EmptySlabs_ReturnsValidPdf()
    {
        var result = _sut.GenerateGstReport(
            Enumerable.Empty<GstReportRow>(),
            DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, "Test",
            0m, 0m, 0m, 0m);

        result.Should().NotBeNullOrEmpty();
        result[0].Should().Be((byte)'%');
    }

    [Fact]
    public void GenerateProductReport_ReturnsValidPdfBytes()
    {
        var products = new List<ProductReportRow>
        {
            new("Product A", 10m, 500m, 3),
            new("Product B", 5m, 250m, 2)
        };

        var result = _sut.GenerateProductReport(products,
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, "Test Store");

        result.Should().NotBeNullOrEmpty();
        result[0].Should().Be((byte)'%');
    }

    [Fact]
    public void GenerateProductReport_SingleProduct_ReturnsValidPdf()
    {
        var products = new List<ProductReportRow>
        {
            new("Single Product", 1m, 99.99m, 1)
        };

        var result = _sut.GenerateProductReport(products,
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, "Store");

        result.Should().NotBeNullOrEmpty();
        result[0].Should().Be((byte)'%');
    }
}

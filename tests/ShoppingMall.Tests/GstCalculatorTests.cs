using FluentAssertions;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;

namespace ShoppingMall.Tests;

public class GstCalculatorTests
{
    private readonly GstCalculator _sut = new();

    [Fact]
    public void Calculate_IntraState_SplitsCgstSgst()
    {
        var result = _sut.Calculate(1000, 18, SupplyType.IntraState);

        result.TotalTaxRate.Should().Be(18);
        result.CGST.Should().NotBeNull();
        result.SGST.Should().NotBeNull();
        result.IGST.Should().BeNull();
        result.CGST!.Amount.Should().Be(90);
        result.SGST!.Amount.Should().Be(90);
        result.TotalTaxAmount.Should().Be(180);
        result.GrandTotal.Should().Be(1180);
    }

    [Fact]
    public void Calculate_InterState_SetsIgst()
    {
        var result = _sut.Calculate(1000, 12, SupplyType.InterState);

        result.CGST.Should().BeNull();
        result.SGST.Should().BeNull();
        result.IGST.Should().NotBeNull();
        result.IGST!.Amount.Should().Be(120);
        result.TotalTaxAmount.Should().Be(120);
        result.GrandTotal.Should().Be(1120);
    }

    [Fact]
    public void Calculate_ZeroRate_ReturnsZeroTax()
    {
        var result = _sut.Calculate(500, 0, SupplyType.IntraState);

        result.TotalTaxAmount.Should().Be(0);
        result.GrandTotal.Should().Be(500);
    }

    [Fact]
    public void Calculate_NegativeTaxable_ReturnsZeroTax()
    {
        var result = _sut.Calculate(-100, 18, SupplyType.IntraState);

        result.TotalTaxAmount.Should().Be(0);
    }

    [Fact]
    public void DetermineSupplyType_SameState_ReturnsIntraState()
    {
        var result = _sut.DetermineSupplyType("Maharashtra", "Maharashtra");
        result.Should().Be(SupplyType.IntraState);
    }

    [Fact]
    public void DetermineSupplyType_DifferentState_ReturnsInterState()
    {
        var result = _sut.DetermineSupplyType("Maharashtra", "Gujarat");
        result.Should().Be(SupplyType.InterState);
    }
}

using FluentAssertions;
using ShoppingMall.Client.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class CartServiceTests
{
    private readonly CartService _sut = new();

    private static Product MakeProduct(string name = "Test", decimal? price = 100, decimal? mrp = 120, decimal taxRate = 18)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            SKU = $"SKU-{Guid.NewGuid():N}"[..12],
            SellingPrice = price,
            Mrp = mrp,
            TaxRate = taxRate
        };
    }

    [Fact]
    public void AddItem_NewProduct_AddsLine()
    {
        var product = MakeProduct();

        _sut.AddItem(product, 2);

        _sut.HasItems.Should().BeTrue();
        _sut.ItemCount.Should().Be(2);
        _sut.Lines.Should().HaveCount(1);
        _sut.Lines[0].ProductId.Should().Be(product.Id);
        _sut.Lines[0].Quantity.Should().Be(2);
    }

    [Fact]
    public void AddItem_ExistingProduct_IncrementsQuantity()
    {
        var product = MakeProduct();

        _sut.AddItem(product, 1);
        _sut.AddItem(product, 2);

        _sut.Lines.Should().HaveCount(1);
        _sut.Lines[0].Quantity.Should().Be(3);
    }

    [Fact]
    public void AddItem_MultipleProducts_AddsSeparateLines()
    {
        var p1 = MakeProduct("A");
        var p2 = MakeProduct("B");

        _sut.AddItem(p1);
        _sut.AddItem(p2);

        _sut.Lines.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveItem_RemovesLine()
    {
        var product = MakeProduct();
        _sut.AddItem(product);

        _sut.RemoveItem(_sut.Lines[0]);

        _sut.HasItems.Should().BeFalse();
        _sut.Lines.Should().BeEmpty();
    }

    [Fact]
    public void UpdateQuantity_Zero_RemovesLine()
    {
        var product = MakeProduct();
        _sut.AddItem(product);

        _sut.UpdateQuantity(_sut.Lines[0], 0);

        _sut.Lines.Should().BeEmpty();
    }

    [Fact]
    public void UpdateQuantity_Positive_Updates()
    {
        var product = MakeProduct();
        _sut.AddItem(product, 1);

        _sut.UpdateQuantity(_sut.Lines[0], 5);

        _sut.Lines[0].Quantity.Should().Be(5);
    }

    [Fact]
    public void SubTotal_CalculatesCorrectly()
    {
        var p1 = MakeProduct("A", 100);
        var p2 = MakeProduct("B", 200);

        _sut.AddItem(p1, 2);
        _sut.AddItem(p2, 1);

        _sut.SubTotal.Should().Be(400);
    }

    [Fact]
    public void ApplyLineDiscount_ReducesTotal()
    {
        var product = MakeProduct(price: 200);
        _sut.AddItem(product, 2);

        _sut.ApplyLineDiscount(_sut.Lines[0], 50, "Manual discount");

        _sut.Lines[0].DiscountAmount.Should().Be(50);
        _sut.DiscountTotal.Should().Be(50);
        _sut.Lines[0].DiscountReason.Should().Be("Manual discount");
    }

    [Fact]
    public void ApplyLineDiscount_ClampsToLineTotal()
    {
        var product = MakeProduct(price: 100);
        _sut.AddItem(product, 1);

        _sut.ApplyLineDiscount(_sut.Lines[0], 999);

        _sut.Lines[0].DiscountAmount.Should().Be(100);
    }

    [Fact]
    public void ApplyLineDiscountPercent_CalculatesCorrectly()
    {
        var product = MakeProduct(price: 200);
        _sut.AddItem(product, 2);

        _sut.ApplyLineDiscountPercent(_sut.Lines[0], 10);

        _sut.Lines[0].DiscountAmount.Should().Be(40);
        _sut.Lines[0].DiscountReason.Should().Be("10% off");
    }

    [Fact]
    public void ApplyHeaderDiscountPercent_EmptyCart_NoOp()
    {
        _sut.ApplyHeaderDiscountPercent(10);
        _sut.HasItems.Should().BeFalse();
    }

    [Fact]
    public void GrandTotal_IncludesTaxAndDiscount()
    {
        var product = MakeProduct(price: 100, taxRate: 18);
        _sut.AddItem(product, 1);
        // LineTotal=100, Discount=0, TaxAmount=18, GrandTotal=118

        _sut.GrandTotal.Should().Be(118);
    }

    [Fact]
    public void GrandTotal_WithDiscount_ReducesCorrectly()
    {
        var product = MakeProduct(price: 100, taxRate: 18);
        _sut.AddItem(product, 1);
        _sut.ApplyLineDiscount(_sut.Lines[0], 10);
        // TaxableAmount=90, TaxAmount=90*18/100=16.20, GrandTotal=90+16.20=106.20

        _sut.GrandTotal.Should().Be(106.20m);
    }

    [Fact]
    public void Clear_RemovesAllLines()
    {
        _sut.AddItem(MakeProduct("A"));
        _sut.AddItem(MakeProduct("B"));

        _sut.Clear();

        _sut.HasItems.Should().BeFalse();
        _sut.Lines.Should().BeEmpty();
    }

    [Fact]
    public void Serialize_ReturnsJsonArray()
    {
        _sut.AddItem(MakeProduct("A", 50));
        _sut.AddItem(MakeProduct("B", 75));

        var json = _sut.Serialize();

        json.Should().StartWith("[");
        json.Should().EndWith("]");
        json.Should().Contain("ProductName");
    }

    [Fact]
    public void Serialize_EmptyCart_ReturnsEmptyArray()
    {
        var json = _sut.Serialize();
        json.Should().Be("[]");
    }

    [Fact]
    public void AddItem_DefaultQuantity_AddsOne()
    {
        var product = MakeProduct();

        _sut.AddItem(product);

        _sut.Lines[0].Quantity.Should().Be(1);
    }

    [Fact]
    public void ItemCount_SumOfQuantities()
    {
        _sut.AddItem(MakeProduct("A"), 2);
        _sut.AddItem(MakeProduct("B"), 3);

        _sut.ItemCount.Should().Be(5);
    }
}

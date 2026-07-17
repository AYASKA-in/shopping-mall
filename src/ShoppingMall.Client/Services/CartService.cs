using System.Collections.ObjectModel;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.Services;

public class CartService
{
    public ObservableCollection<CartLineItem> Lines { get; } = new();

    public decimal SubTotal => Lines.Sum(l => l.LineTotal);
    public decimal DiscountTotal => Lines.Sum(l => l.DiscountAmount);
    public decimal TaxTotal => Lines.Sum(l => l.TaxAmount);

    public decimal GrandTotal => SubTotal - DiscountTotal + TaxTotal;

    public int ItemCount => Lines.Sum(l => (int)l.Quantity);
    public bool HasItems => Lines.Count > 0;

    public void AddItem(Product product, decimal quantity = 1)
    {
        var existing = Lines.FirstOrDefault(l => l.ProductId == product.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
            return;
        }

        var line = new CartLineItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            SKU = product.SKU,
            Barcode = product.Barcodes.FirstOrDefault()?.Code,
            Quantity = quantity,
            UnitPrice = product.SellingPrice ?? 0,
            Mrp = product.Mrp ?? 0,
            TaxRate = product.TaxRate,
            IsWeighable = product.IsWeighable
        };

        Lines.Add(line);
    }

    public void RemoveItem(CartLineItem line)
    {
        Lines.Remove(line);
    }

    public void UpdateQuantity(CartLineItem line, decimal quantity)
    {
        if (quantity <= 0)
        {
            RemoveItem(line);
            return;
        }
        line.Quantity = quantity;
    }

    public void ApplyLineDiscount(CartLineItem line, decimal discountAmount, string? reason = null)
    {
        if (discountAmount > line.LineTotal)
            discountAmount = line.LineTotal;
        line.DiscountAmount = discountAmount;
        line.DiscountReason = reason;
    }

    public void ApplyLineDiscountPercent(CartLineItem line, decimal percent)
    {
        var amount = Math.Round(line.LineTotal * percent / 100, 2);
        ApplyLineDiscount(line, amount, $"{percent}% off");
    }

    public void ApplyHeaderDiscountPercent(decimal percent)
    {
        if (!HasItems) return;
        var perLine = Math.Round(percent / Lines.Count, 2);
        foreach (var line in Lines)
            ApplyLineDiscountPercent(line, perLine);
    }

    public void Clear()
    {
        Lines.Clear();
    }

    public string Serialize()
    {
        var data = Lines.Select(l => new
        {
            l.ProductId, l.ProductName, l.SKU, l.Barcode,
            l.Quantity, l.UnitPrice, l.Mrp, l.DiscountAmount,
            l.DiscountReason, l.TaxRate, l.IsWeighable, l.WeightKg
        });
        return System.Text.Json.JsonSerializer.Serialize(data);
    }
}

public class CartLineItem : ObservableObject
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string? SKU { get; set; }
    public string? Barcode { get; set; }

    private decimal _quantity = 1;
    public decimal Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); OnPropertyChanged(nameof(NetAmount)); }
    }

    public decimal UnitPrice { get; set; }

    private decimal _discountAmount;
    public decimal DiscountAmount
    {
        get => _discountAmount;
        set { _discountAmount = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); OnPropertyChanged(nameof(NetAmount)); }
    }

    public string? DiscountReason { get; set; }
    public decimal Mrp { get; set; }
    public decimal TaxRate { get; set; }
    public bool IsWeighable { get; set; }

    private decimal? _weightKg;
    public decimal? WeightKg
    {
        get => _weightKg;
        set { _weightKg = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); }
    }

    public decimal LineTotal => Math.Round(Quantity * UnitPrice, 2);
    public decimal TaxableAmount => LineTotal - DiscountAmount;
    public decimal TaxAmount => Math.Round(TaxableAmount * TaxRate / 100, 2);
    public decimal CgstAmount => Math.Round(TaxAmount / 2, 2);
    public decimal SgstAmount => Math.Round(TaxAmount / 2, 2);
    public decimal NetAmount => TaxableAmount + TaxAmount;
}

public class ObservableObject : System.ComponentModel.INotifyPropertyChanged
{
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
}

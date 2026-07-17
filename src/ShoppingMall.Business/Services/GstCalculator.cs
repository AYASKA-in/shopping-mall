using ShoppingMall.Core.Enums;

namespace ShoppingMall.Business.Services;

public class GstCalculator
{
    public GstResult Calculate(decimal taxableAmount, decimal taxRate, SupplyType supplyType)
    {
        var result = new GstResult
        {
            TaxableAmount = taxableAmount,
            TotalTaxRate = taxRate,
            SupplyType = supplyType
        };

        if (taxRate <= 0 || taxableAmount <= 0)
            return result;

        if (supplyType == SupplyType.IntraState)
        {
            var halfRate = taxRate / 2;
            result.CGST = new TaxComponent { Type = TaxType.CGST, Rate = halfRate, Amount = Math.Round(taxableAmount * halfRate / 100, 2) };
            result.SGST = new TaxComponent { Type = TaxType.SGST, Rate = halfRate, Amount = Math.Round(taxableAmount * halfRate / 100, 2) };
        }
        else
        {
            result.IGST = new TaxComponent { Type = TaxType.IGST, Rate = taxRate, Amount = Math.Round(taxableAmount * taxRate / 100, 2) };
        }

        result.TotalTaxAmount = result.CGST?.Amount + result.SGST?.Amount + result.IGST?.Amount ?? 0;
        result.GrandTotal = taxableAmount + result.TotalTaxAmount;
        return result;
    }

    public (decimal taxableAmount, decimal taxAmount) CalculateInclusive(decimal grossAmount, decimal taxRate, SupplyType supplyType)
    {
        var taxable = Math.Round(grossAmount / (1 + taxRate / 100), 2);
        var tax = grossAmount - taxable;
        return (taxable, tax);
    }

    public SupplyType DetermineSupplyType(string supplierState, string storeState)
    {
        return supplierState.Equals(storeState, StringComparison.OrdinalIgnoreCase)
            ? SupplyType.IntraState
            : SupplyType.InterState;
    }
}

public class GstResult
{
    public decimal TaxableAmount { get; set; }
    public decimal TotalTaxRate { get; set; }
    public SupplyType SupplyType { get; set; }
    public TaxComponent? CGST { get; set; }
    public TaxComponent? SGST { get; set; }
    public TaxComponent? IGST { get; set; }
    public decimal TotalTaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
}

public class TaxComponent
{
    public TaxType Type { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}

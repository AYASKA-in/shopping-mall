using FluentValidation;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Validators;

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(x => x.StoreId).NotEmpty();
        RuleFor(x => x.TerminalId).NotEmpty();
        RuleFor(x => x.ReceiptNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.GrandTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Lines).NotEmpty().When(x => x.Status == Core.Enums.TransactionStatus.Completed);
    }
}

public class TransactionLineValidator : AbstractValidator<TransactionLine>
{
    public TransactionLineValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NetAmount).GreaterThanOrEqualTo(0);
    }
}

public class PaymentValidator : AbstractValidator<Payment>
{
    public PaymentValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.TransactionId).NotEmpty();
        RuleFor(x => x.Method).IsInEnum();
    }
}

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.BaseUOMId).NotEmpty();
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
    }
}

public class PurchaseOrderValidator : AbstractValidator<PurchaseOrder>
{
    public PurchaseOrderValidator()
    {
        RuleFor(x => x.PONumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.StoreId).NotEmpty();
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.GrandTotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Lines).NotEmpty();
    }
}

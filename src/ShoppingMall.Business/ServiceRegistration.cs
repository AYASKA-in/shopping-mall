using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShoppingMall.Business.Services;
using ShoppingMall.Business.Validators;

namespace ShoppingMall.Business;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<PosService>();
        services.AddScoped<GstCalculator>();
        services.AddScoped<InventoryService>();
        services.AddScoped<PromotionEngine>();
        services.AddScoped<LoyaltyService>();
        services.AddScoped<BarcodeService>();
        services.AddScoped<ProductBulkImportService>();
        services.AddScoped<PurchaseOrderService>();

        services.AddValidatorsFromAssemblyContaining<TransactionValidator>();

        return services;
    }
}

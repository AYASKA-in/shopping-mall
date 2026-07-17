using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Data.DbContext;
using ShoppingMall.Data.Repositories;

namespace ShoppingMall.Data;

public static class ServiceRegistration
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShoppingMallDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICurrentStockRepository, CurrentStockRepository>();

        return services;
    }
}

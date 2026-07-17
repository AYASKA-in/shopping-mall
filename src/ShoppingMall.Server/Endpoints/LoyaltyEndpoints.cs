using ShoppingMall.Business.Services;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Server.Endpoints;

public static class LoyaltyEndpoints
{
    public static void MapLoyaltyEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/loyalty").WithTags("Loyalty");

        group.MapGet("/account/{customerId}", async (Guid customerId, LoyaltyService svc) =>
        {
            var account = await svc.GetOrCreateAccountAsync(customerId);
            return Results.Ok(account);
        });

        group.MapGet("/lookup/{phone}", async (string phone, IRepository<Customer> customerRepo, LoyaltyService svc) =>
        {
            var customers = await customerRepo.FindAsync(c => c.Phone == phone);
            var customer = customers.FirstOrDefault();
            if (customer == null)
                return Results.NotFound("Customer not found");

            var account = await svc.GetOrCreateAccountAsync(customer.Id);
            return Results.Ok(new
            {
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                customer.Phone,
                account.CardNumber,
                account.PointsBalance,
                account.LifetimePoints,
                account.Tier,
                RedeemableValue = svc.ConvertPointsToCurrency(account.PointsBalance)
            });
        });

        group.MapPost("/earn", async (EarnPointsRequest request, LoyaltyService svc) =>
        {
            try
            {
                var txn = await svc.EarnPointsAsync(request.CustomerId, request.QualifyingSpend, request.TransactionId);
                return Results.Ok(txn);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/redeem", async (RedeemPointsRequest request, LoyaltyService svc) =>
        {
            try
            {
                var txn = await svc.RedeemPointsAsync(request.CustomerId, request.Points, request.TransactionId);
                return Results.Ok(txn);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}

public record EarnPointsRequest(Guid CustomerId, decimal QualifyingSpend, Guid TransactionId);
public record RedeemPointsRequest(Guid CustomerId, int Points, Guid TransactionId);

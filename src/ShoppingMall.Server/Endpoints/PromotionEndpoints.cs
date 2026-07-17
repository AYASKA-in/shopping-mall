using ShoppingMall.Business.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Server.Endpoints;

public static class PromotionEndpoints
{
    public static void MapPromotionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/promotions").WithTags("Promotions");

        group.MapGet("/active", async (PromotionEngine engine) =>
        {
            var promotions = await engine.GetActivePromotionsAsync();
            return Results.Ok(promotions);
        });

        group.MapPost("/evaluate", async (EvaluatePromotionRequest request, PromotionEngine engine) =>
        {
            var cart = new CartContext
            {
                SubTotal = request.SubTotal,
                TotalQuantity = request.Items.Sum(i => (int)i.Quantity),
                Items = request.Items.Select(i => new CartItem
                {
                    ProductId = i.ProductId,
                    CategoryId = i.CategoryId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var result = await engine.EvaluateAsync(cart, request.CouponCode);
            return Results.Ok(result);
        });

        group.MapPost("/coupons/validate", async (ValidateCouponRequest request, PromotionEngine engine) =>
        {
            var result = await engine.ValidateCouponAsync(request.Code, request.CartTotal);
            return Results.Ok(result);
        });
    }
}

public record EvaluatePromotionRequest(decimal SubTotal, List<EvaluateCartItem> Items, string? CouponCode);
public record EvaluateCartItem(Guid ProductId, Guid CategoryId, decimal Quantity, decimal UnitPrice);
public record ValidateCouponRequest(string Code, decimal CartTotal);

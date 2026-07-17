using ShoppingMall.Business.Services;

namespace ShoppingMall.Server.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory");

        group.MapGet("/stock/{storeId}/{productId}", async (Guid storeId, Guid productId, InventoryService inv) =>
        {
            var stock = await inv.GetStockAsync(storeId, productId);
            return stock is null ? Results.NotFound() : Results.Ok(stock);
        });

        group.MapGet("/stock/{storeId}/low", async (Guid storeId, InventoryService inv) =>
            Results.Ok(await inv.GetLowStockAsync(storeId)));

        group.MapGet("/stock/{storeId}/movements/{productId}", async (Guid storeId, Guid productId, DateTime from, DateTime to, InventoryService inv) =>
            Results.Ok(await inv.GetStockMovementAsync(storeId, productId, from, to)));

        group.MapPost("/grn", async (GoodsReceipt grn, IRepository<GoodsReceipt> grnRepo, InventoryService inv) =>
        {
            grn.Id = Guid.NewGuid();
            grn.Status = Core.Enums.GRNStatus.Completed;
            var created = await grnRepo.AddAsync(grn);

            foreach (var line in grn.Lines)
            {
                await inv.AddStockAsync(grn.StoreId, line.ProductId, line.AcceptedQty, "GRN", created.Id, line.UnitPrice);
            }

            return Results.Created($"/api/inventory/grn/{created.Id}", created);
        });

        group.MapPost("/adjustments", async (StockAdjustment adjustment, IRepository<StockAdjustment> adjRepo, InventoryService inv) =>
        {
            adjustment.Id = Guid.NewGuid();
            var created = await adjRepo.AddAsync(adjustment);

            foreach (var line in adjustment.Lines)
            {
                if (line.QuantityChange > 0)
                    await inv.AddStockAsync(created.StoreId, line.ProductId, line.QuantityChange, "ADJUSTMENT", created.Id, line.UnitCost);
                else
                    await inv.DeductStockAsync(created.StoreId, line.ProductId, Math.Abs(line.QuantityChange), "ADJUSTMENT", created.Id, line.UnitCost);
            }

            return Results.Created($"/api/inventory/adjustments/{created.Id}", created);
        });

        group.MapGet("/grn/{storeId}", async (Guid storeId, IRepository<GoodsReceipt> grnRepo) =>
        {
            var grns = await grnRepo.FindAsync(g => g.StoreId == storeId);
            return Results.Ok(grns.OrderByDescending(g => g.CreatedAt));
        });
    }
}

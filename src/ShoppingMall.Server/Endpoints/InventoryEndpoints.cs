using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Models;
using ShoppingMall.Core.Interfaces;

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

        group.MapPost("/grn", async (GoodsReceipt grn, IRepository<GoodsReceipt> grnRepo,
            IRepository<PurchaseOrder> poRepo, IRepository<POLine> poLineRepo,
            PurchaseOrderService poService, InventoryService inv) =>
        {
            grn.Id = Guid.NewGuid();
            grn.Status = GRNStatus.Completed;
            var created = await grnRepo.AddAsync(grn);

            foreach (var line in grn.Lines)
            {
                await inv.AddStockAsync(grn.StoreId, line.ProductId, line.AcceptedQty, "GRN", created.Id, line.UnitPrice);

                if (grn.POId.HasValue)
                {
                    var poLines = await poLineRepo.FindAsync(pl => pl.POId == grn.POId && pl.ProductId == line.ProductId);
                    foreach (var poLine in poLines)
                    {
                        poLine.ReceivedQty += line.ReceivedQty;
                        poLine.AcceptedQty += line.AcceptedQty;
                        poLine.RejectedQty += line.RejectedQty;
                        await poLineRepo.UpdateAsync(poLine);
                    }

                    var allLines = await poLineRepo.FindAsync(pl => pl.POId == grn.POId);
                    var totalOrdered = allLines.Sum(pl => pl.OrderedQty);
                    var totalAccepted = allLines.Sum(pl => pl.AcceptedQty);

                    if (totalAccepted >= totalOrdered)
                        await poService.MarkFullyReceivedAsync(grn.POId.Value);
                    else
                        await poService.MarkPartiallyReceivedAsync(grn.POId.Value);
                }
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

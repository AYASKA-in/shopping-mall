using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Models;
using ShoppingMall.Core.Interfaces;

namespace ShoppingMall.Server.Endpoints;

public static class ProcurementEndpoints
{
    public static void MapProcurementEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/procurement").WithTags("Procurement");

        group.MapGet("/purchase-orders/{storeId}", async (Guid storeId, IRepository<PurchaseOrder> repo) =>
        {
            var pos = await repo.FindAsync(p => p.StoreId == storeId);
            return Results.Ok(pos.OrderByDescending(p => p.CreatedAt));
        });

        group.MapGet("/purchase-orders/detail/{id}", async (Guid id, IRepository<PurchaseOrder> repo) =>
        {
            var po = await repo.GetByIdAsync(id);
            return po is null ? Results.NotFound() : Results.Ok(po);
        });

        group.MapPost("/purchase-orders", async (PurchaseOrder po, PurchaseOrderService poService) =>
        {
            var created = await poService.CreateAsync(po);
            return Results.Created($"/api/procurement/purchase-orders/detail/{created.Id}", created);
        });

        group.MapPost("/purchase-orders/{id}/submit", async (Guid id, PurchaseOrderService poService) =>
        {
            var po = await poService.SubmitAsync(id);
            return po is null ? Results.NotFound() : Results.Ok(po);
        });

        group.MapPost("/purchase-orders/{id}/close", async (Guid id, PurchaseOrderService poService) =>
        {
            var po = await poService.CloseAsync(id);
            return po is null ? Results.NotFound() : Results.Ok(po);
        });

        group.MapGet("/suppliers", async (IRepository<Supplier> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/suppliers/search", async (string? q, IRepository<Supplier> repo) =>
        {
            if (string.IsNullOrWhiteSpace(q))
                return Results.Ok(await repo.GetAllAsync());
            var suppliers = await repo.FindAsync(s =>
                s.Name.Contains(q) || s.Code.Contains(q) ||
                s.Phone!.Contains(q) || s.GSTIN!.Contains(q));
            return Results.Ok(suppliers);
        });

        group.MapPost("/suppliers", async (Supplier supplier, IRepository<Supplier> repo) =>
        {
            supplier.Id = Guid.NewGuid();
            var created = await repo.AddAsync(supplier);
            return Results.Created($"/api/procurement/suppliers/{created.Id}", created);
        });

        group.MapPut("/suppliers/{id}", async (Guid id, Supplier supplier, IRepository<Supplier> repo) =>
        {
            supplier.Id = id;
            supplier.UpdatedAt = DateTime.UtcNow;
            await repo.UpdateAsync(supplier);
            return Results.Ok(supplier);
        });

        group.MapGet("/vendor-invoices/{poId}", async (Guid poId, IRepository<VendorInvoice> repo) =>
        {
            var invoices = await repo.FindAsync(i => i.POId == poId);
            return Results.Ok(invoices);
        });
    }
}

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

        group.MapPost("/purchase-orders", async (PurchaseOrder po, IRepository<PurchaseOrder> poRepo) =>
        {
            po.Id = Guid.NewGuid();
            po.Status = Core.Enums.POStatus.Draft;
            po.CreatedAt = DateTime.UtcNow;
            var created = await poRepo.AddAsync(po);
            return Results.Created($"/api/procurement/purchase-orders/detail/{created.Id}", created);
        });

        group.MapPut("/purchase-orders/{id}/status", async (Guid id, string status, IRepository<PurchaseOrder> poRepo) =>
        {
            var po = await poRepo.GetByIdAsync(id);
            if (po is null) return Results.NotFound();
            po.Status = Enum.Parse<Core.Enums.POStatus>(status);
            await poRepo.UpdateAsync(po);
            return Results.Ok(po);
        });

        group.MapGet("/suppliers", async (IRepository<Supplier> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapPost("/suppliers", async (Supplier supplier, IRepository<Supplier> repo) =>
        {
            supplier.Id = Guid.NewGuid();
            var created = await repo.AddAsync(supplier);
            return Results.Created($"/api/procurement/suppliers/{created.Id}", created);
        });

        group.MapGet("/vendor-invoices/{poId}", async (Guid poId, IRepository<VendorInvoice> repo) =>
        {
            var invoices = await repo.FindAsync(i => i.POId == poId);
            return Results.Ok(invoices);
        });
    }
}

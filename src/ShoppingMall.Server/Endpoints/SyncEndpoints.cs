namespace ShoppingMall.Server.Endpoints;

public static class SyncEndpoints
{
    public static void MapSyncEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sync").WithTags("Sync");

        group.MapGet("/queue/{storeId}", async (Guid storeId, IRepository<SyncQueue> repo) =>
        {
            var items = await repo.FindAsync(q => q.StoreId == storeId && q.Status == Core.Enums.SyncStatus.Pending);
            return Results.Ok(items.OrderBy(q => q.CreatedAt).Take(100));
        });

        group.MapPost("/queue", async (SyncQueue item, IRepository<SyncQueue> repo) =>
        {
            item.Id = Guid.NewGuid();
            item.CreatedAt = DateTime.UtcNow;
            var created = await repo.AddAsync(item);
            return Results.Created($"/api/sync/queue/{created.Id}", created);
        });

        group.MapPut("/queue/{id}/complete", async (Guid id, IRepository<SyncQueue> repo) =>
        {
            var item = await repo.GetByIdAsync(id);
            if (item is null) return Results.NotFound();
            item.Status = Core.Enums.SyncStatus.Completed;
            item.ProcessedAt = DateTime.UtcNow;
            await repo.UpdateAsync(item);
            return Results.Ok();
        });

        group.MapGet("/logs/{storeId}", async (Guid storeId, IRepository<SyncLog> repo) =>
        {
            var logs = await repo.FindAsync(l => l.StoreId == storeId);
            return Results.Ok(logs.OrderByDescending(l => l.StartedAt).Take(50));
        });

        group.MapPost("/backup", async (CloudBackup backup, IRepository<CloudBackup> repo) =>
        {
            backup.Id = Guid.NewGuid();
            backup.CreatedAt = DateTime.UtcNow;
            var created = await repo.AddAsync(backup);
            return Results.Created($"/api/sync/backup/{created.Id}", created);
        });

        group.MapGet("/backup/{storeId}/latest", async (Guid storeId, IRepository<CloudBackup> repo) =>
        {
            var backups = await repo.FindAsync(b => b.StoreId == storeId);
            return Results.Ok(backups.OrderByDescending(b => b.CreatedAt).FirstOrDefault());
        });
    }
}

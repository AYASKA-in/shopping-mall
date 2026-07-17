namespace ShoppingMall.Server.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin");

        group.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

        group.MapPost("/terminals/register", async (RegisterTerminalRequest request,
            IRepository<Store> storeRepo, IRepository<Terminal> termRepo) =>
        {
            var stores = await storeRepo.FindAsync(s => s.Code == request.StoreCode && s.IsActive);
            var store = stores.FirstOrDefault();
            if (store is null) return Results.NotFound("Store not found");

            var terminal = new Terminal
            {
                Id = Guid.NewGuid(),
                StoreId = store.Id,
                Name = request.Name,
                DeviceId = Guid.NewGuid().ToString(),
                Mode = TerminalMode.Client,
                IsActive = true,
                LastHeartbeat = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await termRepo.AddAsync(terminal);

            return Results.Ok(new TerminalRegistrationResponse(
                terminal.Id, terminal.StoreId, terminal.Name, store.Name));
        });

        group.MapGet("/stores", async (IRepository<Store> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapPost("/stores", async (Store store, IRepository<Store> repo) =>
        {
            store.Id = Guid.NewGuid();
            store.CreatedAt = DateTime.UtcNow;
            var created = await repo.AddAsync(store);
            return Results.Created($"/api/admin/stores/{created.Id}", created);
        });

        group.MapGet("/users", async (IRepository<User> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapPut("/users/{id}/pin", async (Guid id, string newPin, IRepository<User> userRepo) =>
        {
            var user = await userRepo.GetByIdAsync(id);
            if (user is null) return Results.NotFound();
            var (hash, salt) = AuthService.HashPin(newPin);
            user.PinHash = hash;
            user.PinSalt = salt;
            await userRepo.UpdateAsync(user);
            return Results.Ok();
        });

        group.MapPost("/terminals", async (Terminal terminal, IRepository<Terminal> repo) =>
        {
            terminal.Id = Guid.NewGuid();
            var created = await repo.AddAsync(terminal);
            return Results.Created($"/api/admin/terminals/{created.Id}", created);
        });

        group.MapGet("/terminals/{storeId}", async (Guid storeId, IRepository<Terminal> repo) =>
        {
            var terminals = await repo.FindAsync(t => t.StoreId == storeId);
            return Results.Ok(terminals);
        });

        group.MapGet("/config/{storeId}", async (Guid storeId, IRepository<StoreConfig> repo) =>
        {
            var configs = await repo.FindAsync(c => c.StoreId == storeId);
            return Results.Ok(configs.ToDictionary(c => c.ConfigKey, c => c.ConfigValue));
        });

        group.MapPut("/config/{storeId}", async (Guid storeId, Dictionary<string, string> configs, IRepository<StoreConfig> repo) =>
        {
            foreach (var kvp in configs)
            {
                var existing = await repo.FindAsync(c => c.StoreId == storeId && c.ConfigKey == kvp.Key);
                var config = existing.FirstOrDefault();
                if (config == null)
                {
                    await repo.AddAsync(new StoreConfig
                    {
                        Id = Guid.NewGuid(),
                        StoreId = storeId,
                        ConfigKey = kvp.Key,
                        ConfigValue = kvp.Value
                    });
                }
                else
                {
                    config.ConfigValue = kvp.Value;
                    await repo.UpdateAsync(config);
                }
            }
            return Results.Ok();
        });

        group.MapPost("/terminals/{id}/heartbeat", async (Guid id, IRepository<Terminal> repo) =>
        {
            var terminal = await repo.GetByIdAsync(id);
            if (terminal is null) return Results.NotFound();
            terminal.LastHeartbeat = DateTime.UtcNow;
            await repo.UpdateAsync(terminal);
            return Results.Ok(new { lastHeartbeat = terminal.LastHeartbeat });
        });

        group.MapGet("/hsn-codes", async (IRepository<HSN> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapPost("/hsn-codes", async (HSN hsn, IRepository<HSN> repo) =>
        {
            hsn.Id = Guid.NewGuid();
            var created = await repo.AddAsync(hsn);
            return Results.Created($"/api/admin/hsn-codes/{created.Id}", created);
        });
    }
}

public record RegisterTerminalRequest(string StoreCode, string Name);
public record TerminalRegistrationResponse(Guid Id, Guid StoreId, string Name, string StoreName);

using ShoppingMall.Business.Services;
using ShoppingMall.Core.Models;

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

        group.MapPut("/stores/{id}", async (Guid id, UpdateStoreRequest request, IRepository<Store> repo) =>
        {
            var store = await repo.GetByIdAsync(id);
            if (store is null) return Results.NotFound();

            if (request.Name != null) store.Name = request.Name;
            if (request.Code != null) store.Code = request.Code;
            if (request.GSTIN != null) store.GSTIN = request.GSTIN;
            if (request.Phone != null) store.Phone = request.Phone;
            if (request.Status.HasValue) store.Status = request.Status.Value;
            if (request.IsActive.HasValue) store.IsActive = request.IsActive.Value;
            if (request.ReceiptFooter != null) store.ReceiptFooter = request.ReceiptFooter;
            store.UpdatedAt = DateTime.UtcNow;
            await repo.UpdateAsync(store);
            return Results.Ok(store);
        });

        group.MapGet("/users", async (IRepository<User> repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapPost("/users", async (CreateUserRequest request, IRepository<User> userRepo) =>
        {
            var existing = await userRepo.FindAsync(u => u.Username == request.Username);
            if (existing.Any()) return Results.BadRequest(new { error = "Username already exists" });

            var (hash, salt) = AuthService.HashPin(request.Pin);
            var user = new User
            {
                Id = Guid.NewGuid(),
                StoreId = request.StoreId,
                Username = request.Username,
                DisplayName = request.DisplayName,
                PinHash = hash,
                PinSalt = salt,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var created = await userRepo.AddAsync(user);
            return Results.Created($"/api/admin/users/{created.Id}", created);
        });

        group.MapPut("/users/{id}", async (Guid id, UpdateUserRequest request, IRepository<User> userRepo) =>
        {
            var user = await userRepo.GetByIdAsync(id);
            if (user is null) return Results.NotFound();

            if (request.DisplayName != null) user.DisplayName = request.DisplayName;
            if (request.Role.HasValue) user.Role = request.Role.Value;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;
            if (request.Pin != null)
            {
                var (hash, salt) = AuthService.HashPin(request.Pin);
                user.PinHash = hash;
                user.PinSalt = salt;
            }
            user.UpdatedAt = DateTime.UtcNow;
            await userRepo.UpdateAsync(user);
            return Results.Ok(user);
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

        group.MapPost("/backups/{storeId}", async (Guid storeId, CloudBackupService backup) =>
        {
            try
            {
                var result = await backup.CreateBackupAsync(storeId);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapGet("/backups/{storeId}", async (Guid storeId, CloudBackupService backup) =>
        {
            var backups = await backup.ListBackupsAsync(storeId);
            return Results.Ok(backups);
        });

        group.MapGet("/audit-log", async (IRepository<SyncLog> syncLogRepo) =>
        {
            var logs = await syncLogRepo.FindAsync(l => true);
            return Results.Ok(logs.OrderByDescending(l => l.StartedAt).Take(100));
        });
    }
}

public record RegisterTerminalRequest(string StoreCode, string Name);
public record TerminalRegistrationResponse(Guid Id, Guid StoreId, string Name, string StoreName);
public record CreateUserRequest(string Username, string DisplayName, string Pin, UserRole Role, Guid? StoreId);
public record UpdateUserRequest(string? DisplayName, UserRole? Role, bool? IsActive, string? Pin);
public record UpdateStoreRequest(string? Name, string? Code, string? GSTIN, string? Phone, StoreStatus? Status, bool? IsActive, string? ReceiptFooter);

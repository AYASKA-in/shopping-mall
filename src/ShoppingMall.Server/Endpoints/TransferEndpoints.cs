using ShoppingMall.Business.Services;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Server.Endpoints;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/transfers").WithTags("Transfers");

        group.MapGet("/store/{storeId}", async (Guid storeId, InterStoreTransferService svc) =>
        {
            var transfers = await svc.GetByStoreAsync(storeId);
            return Results.Ok(transfers.OrderByDescending(t => t.CreatedAt));
        });

        group.MapPost("/", async (CreateTransferRequest request, InterStoreTransferService svc) =>
        {
            try
            {
                var transfer = await svc.CreateAsync(
                    request.FromStoreId, request.ToStoreId, request.CreatedByUserId,
                    request.Lines, request.Notes);
                return Results.Created($"/api/transfers/{transfer.Id}", transfer);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/{id}/ship", async (Guid id, ShipTransferRequest? request, InterStoreTransferService svc) =>
        {
            try
            {
                var transfer = await svc.ShipAsync(id, request?.Lines);
                return transfer is null ? Results.NotFound() : Results.Ok(transfer);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/{id}/receive", async (Guid id, ReceiveTransferRequest? request, InterStoreTransferService svc) =>
        {
            try
            {
                var transfer = await svc.ReceiveAsync(id, request?.Lines);
                return transfer is null ? Results.NotFound() : Results.Ok(transfer);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPost("/{id}/cancel", async (Guid id, InterStoreTransferService svc) =>
        {
            try
            {
                var transfer = await svc.CancelAsync(id);
                return transfer is null ? Results.NotFound() : Results.Ok(transfer);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }
}

public record CreateTransferRequest(Guid FromStoreId, Guid ToStoreId, Guid CreatedByUserId, List<CreateTransferLine> Lines, string? Notes);
public record ShipTransferRequest(List<ShipLineUpdate>? Lines);
public record ReceiveTransferRequest(List<ShipLineUpdate>? Lines);

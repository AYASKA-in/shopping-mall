using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;

namespace ShoppingMall.Server.Endpoints;

public static class PosEndpoints
{
    public static void MapPosEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/pos").WithTags("POS");

        group.MapPost("/transactions", async (CreateTransactionRequest request, PosService pos) =>
        {
            var txn = await pos.CreateTransactionAsync(request.StoreId, request.TerminalId, request.UserId);
            return Results.Created($"/api/pos/transactions/{txn.Id}", txn);
        });

        group.MapPost("/transactions/{id}/items", async (Guid id, AddLineItemRequest request, PosService pos) =>
        {
            var line = await pos.AddLineItemAsync(id, request.ProductId, request.Quantity, request.OverridePrice);
            return Results.Ok(line);
        });

        group.MapPost("/transactions/{id}/payments", async (Guid id, ProcessPaymentRequest request, PosService pos) =>
        {
            var payment = await pos.ProcessPaymentAsync(id, request.Amount, request.Method, request.TenderedAmount);
            return Results.Ok(payment);
        });

        group.MapGet("/transactions/{id}", async (Guid id, ITransactionRepository txnRepo) =>
        {
            var txn = await txnRepo.GetByIdAsync(id);
            return txn is null ? Results.NotFound() : Results.Ok(txn);
        });

        group.MapGet("/transactions/receipt/{receiptNumber}", async (string receiptNumber, PosService pos) =>
        {
            var txn = await pos.LookupByReceiptAsync(receiptNumber);
            return txn is null ? Results.NotFound() : Results.Ok(txn);
        });

        group.MapGet("/transactions/phone/{phone}", async (string phone, PosService pos) =>
        {
            var txn = await pos.LookupByPhoneAsync(phone);
            return txn is null ? Results.NotFound() : Results.Ok(txn);
        });

        group.MapGet("/transactions/store/{storeId}/today", async (Guid storeId, ITransactionRepository txnRepo) =>
        {
            var txns = await txnRepo.GetByStoreAndDateAsync(storeId, DateTime.UtcNow);
            return Results.Ok(txns);
        });
    }
}

public record CreateTransactionRequest(Guid StoreId, Guid TerminalId, Guid? UserId);
public record AddLineItemRequest(Guid ProductId, decimal Quantity, decimal? OverridePrice);
public record ProcessPaymentRequest(decimal Amount, PaymentMethod Method, decimal? TenderedAmount);

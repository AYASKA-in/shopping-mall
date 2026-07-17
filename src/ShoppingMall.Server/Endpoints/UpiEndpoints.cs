using ShoppingMall.Business.Services;

namespace ShoppingMall.Server.Endpoints;

public static class UpiEndpoints
{
    public static void MapUpiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/upi").WithTags("UPI");

        group.MapGet("/qr/static/{terminalId}", async (Guid terminalId, UpiPaymentService upi, IRepository<Terminal> terminalRepo, IRepository<Store> storeRepo) =>
        {
            var terminal = await terminalRepo.GetByIdAsync(terminalId);
            if (terminal == null) return Results.NotFound("Terminal not found");

            var store = await storeRepo.GetByIdAsync(terminal.StoreId);
            var upiId = app.Configuration.GetValue<string>("Upi:Id") ?? $"store{store?.Code ?? "xxx"}@paytm";
            var intent = upi.GenerateStaticQrIntent(upiId, store?.Name ?? "Store");
            return Results.Ok(new { upiId, intent, storeName = store?.Name });
        });

        group.MapPost("/qr/dynamic", async (DynamicQrRequest request, UpiPaymentService upi) =>
        {
            var upiId = app.Configuration.GetValue<string>("Upi:Id") ?? "store@paytm";
            var intent = upi.GenerateDynamicQrIntent(upiId, request.StoreName, request.Amount, request.TransactionRef, request.Note);
            return Results.Ok(new { intent, amount = request.Amount, transactionRef = request.TransactionRef });
        });

        group.MapPost("/order", async (CreateOrderRequest request, UpiPaymentService upi) =>
        {
            var order = await upi.CreateRazorpayOrderAsync(request.Amount, request.ReceiptId);
            return order is null
                ? Results.BadRequest(new { error = "Razorpay not configured. Set Razorpay:KeyId and Razorpay:KeySecret." })
                : Results.Ok(order);
        });

        group.MapPost("/verify", async (VerifyPaymentRequest request, UpiPaymentService upi) =>
        {
            var isValid = upi.VerifyRazorpayPayment(request.RazorpayOrderId, request.RazorpayPaymentId, request.RazorpaySignature);
            return Results.Ok(new { isValid });
        });

        group.MapPost("/webhook", async (HttpContext context, UpiPaymentService upi, IRepository<Payment> paymentRepo, ITransactionRepository txnRepo) =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = context.Request.Headers["X-Razorpay-Signature"].FirstOrDefault() ?? "";
            var webhookSecret = app.Configuration.GetValue<string>("Razorpay:WebhookSecret") ?? "";

            var isValid = upi.VerifyPaymentWebhook(payload, signature, webhookSecret);
            if (!isValid)
                return Results.Unauthorized();

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(payload);
                var root = doc.RootElement;
                var @event = root.GetProperty("event").GetString();

                if (@event == "payment.captured" || @event == "order.paid")
                {
                    var paymentEntity = root.GetProperty("payload").GetProperty("payment").GetProperty("entity");
                    var razorpayPaymentId = paymentEntity.GetProperty("id").GetString() ?? "";
                    var razorpayOrderId = paymentEntity.GetProperty("order_id").GetString() ?? "";
                    var status = paymentEntity.GetProperty("status").GetString() ?? "";
                    var amountPaise = paymentEntity.GetProperty("amount").GetInt64();

                    var payments = await paymentRepo.FindAsync(p =>
                        p.GatewayResponse != null && p.GatewayResponse.Contains(razorpayOrderId));
                    var payment = payments.FirstOrDefault();

                    if (payment != null)
                    {
                        payment.Status = status == "captured" ? Core.Enums.PaymentStatus.Captured
                            : status == "refunded" ? Core.Enums.PaymentStatus.Refunded
                            : payment.Status;
                        payment.ReferenceNumber = razorpayPaymentId;
                        payment.SettledAt = DateTime.UtcNow;
                        await paymentRepo.UpdateAsync(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<UpiPaymentService>>();
                logger.LogWarning(ex, "Failed to process UPI webhook");
            }

            return Results.Ok(new { status = "received" });
        });
    }
}

public record DynamicQrRequest(decimal Amount, string TransactionRef, string StoreName, string? Note = null);
public record CreateOrderRequest(decimal Amount, string ReceiptId);
public record VerifyPaymentRequest(string RazorpayOrderId, string RazorpayPaymentId, string RazorpaySignature);

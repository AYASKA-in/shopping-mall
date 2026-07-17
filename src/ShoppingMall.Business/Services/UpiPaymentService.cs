using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace ShoppingMall.Business.Services;

public class UpiPaymentService
{
    private readonly IConfiguration _configuration;

    public UpiPaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateStaticQrIntent(string upiId, string payeeName)
    {
        return $"upi://pay?pa={upiId}&pn={Uri.EscapeDataString(payeeName)}&cu=INR";
    }

    public string GenerateDynamicQrIntent(string upiId, string payeeName, decimal amount, string transactionRef, string? note = null)
    {
        var intent = $"upi://pay?pa={upiId}&pn={Uri.EscapeDataString(payeeName)}&am={amount:F2}&tr={Uri.EscapeDataString(transactionRef)}&cu=INR";
        if (!string.IsNullOrEmpty(note))
            intent += $"&tn={Uri.EscapeDataString(note)}";
        return intent;
    }

    public Task<RazorpayOrder?> CreateRazorpayOrderAsync(decimal amount, string receiptId)
    {
        var keyId = _configuration["Razorpay:KeyId"];
        var keySecret = _configuration["Razorpay:KeySecret"];

        if (string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(keySecret))
            return Task.FromResult<RazorpayOrder?>(null);

        var client = new RazorpayClient(keyId, keySecret);

        var options = new Dictionary<string, object>
        {
            { "amount", (int)(amount * 100) },
            { "currency", "INR" },
            { "receipt", receiptId },
            { "payment_capture", 1 }
        };

        var order = client.Order.Create(options);
        return Task.FromResult<RazorpayOrder?>(new RazorpayOrder(
            order["id"].ToString() ?? "",
            (int)order["amount"],
            order["currency"]?.ToString() ?? "INR",
            order["receipt"]?.ToString() ?? receiptId,
            order["status"]?.ToString() ?? ""
        ));
    }

    public bool VerifyPaymentWebhook(string payload, string signature, string secret)
    {
        var expected = GenerateHmacSha256(payload, secret);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(signature));
    }

    public bool VerifyRazorpayPayment(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature)
    {
        var expected = GenerateHmacSha256($"{razorpayOrderId}|{razorpayPaymentId}",
            _configuration["Razorpay:KeySecret"] ?? "");
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(razorpaySignature));
    }

    private static string GenerateHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

public record RazorpayOrder(string OrderId, int AmountPaise, string Currency, string Receipt, string Status);

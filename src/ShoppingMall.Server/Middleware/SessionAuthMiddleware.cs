using System.Text.Json;
using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Server.Middleware;

public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly TimeSpan MaxSessionDuration = TimeSpan.FromHours(8);
    private static readonly HashSet<string> PublicPathPrefixes =
    [
        "/api/auth/login",
        "/api/admin/health",
        "/api/admin/terminals/register"
    ];

    public SessionAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ShoppingMallDbContext db)
    {
        var path = context.Request.Path.Value ?? "";

        if (PublicPathPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Session-Id", out var sessionIdValues))
        {
            await WriteUnauthorized(context, "Missing session");
            return;
        }

        if (!Guid.TryParse(sessionIdValues.FirstOrDefault(), out var sessionId))
        {
            await WriteUnauthorized(context, "Invalid session format");
            return;
        }

        var session = await db.Sessions.FindAsync(sessionId);
        if (session == null || !session.IsActive)
        {
            await WriteUnauthorized(context, "Invalid or inactive session");
            return;
        }

        if (session.LogoutAt.HasValue)
        {
            await WriteUnauthorized(context, "Session ended");
            return;
        }

        if (DateTime.UtcNow - session.LoginAt > MaxSessionDuration)
        {
            session.IsActive = false;
            await db.SaveChangesAsync();
            await WriteUnauthorized(context, "Session expired");
            return;
        }

        context.Items["SessionId"] = session.Id;
        context.Items["UserId"] = session.UserId;
        context.Items["TerminalId"] = session.TerminalId;

        await _next(context);
    }

    private static async Task WriteUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}

using ShoppingMall.Business.Services;

namespace ShoppingMall.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", async (LoginRequest request, AuthService auth, IRepository<Terminal> terminalRepo) =>
        {
            var result = await auth.AuthenticateAsync(request.Username, request.Pin, request.TerminalId);
            if (!result.IsSuccess)
                return Results.Unauthorized();

            return Results.Ok(new
            {
                result.User!.Id,
                result.User.DisplayName,
                result.User.Role,
                result.User.StoreId,
                SessionId = result.SessionId
            });
        });

        group.MapPost("/logout", async (Guid sessionId, AuthService auth) =>
        {
            await auth.LogoutAsync(sessionId);
            return Results.Ok();
        });

        group.MapPost("/register", async (RegisterUserRequest request, AuthService auth, IRepository<User> userRepo) =>
        {
            var (hash, salt) = AuthService.HashPin(request.Pin);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                DisplayName = request.DisplayName,
                PinHash = hash,
                PinSalt = salt,
                Role = request.Role,
                StoreId = request.StoreId,
                CreatedAt = DateTime.UtcNow
            };
            await userRepo.AddAsync(user);
            return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Username, user.DisplayName });
        });
    }
}

public record LoginRequest(string Username, string Pin, Guid TerminalId);
public record RegisterUserRequest(string Username, string DisplayName, string Pin, Core.Enums.UserRole Role, Guid? StoreId);

using System.Security.Cryptography;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class AuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Session> _sessionRepo;
    private readonly IRepository<Terminal> _terminalRepo;

    public AuthService(IRepository<User> userRepo, IRepository<Session> sessionRepo, IRepository<Terminal> terminalRepo)
    {
        _userRepo = userRepo;
        _sessionRepo = sessionRepo;
        _terminalRepo = terminalRepo;
    }

    public async Task<AuthResult> AuthenticateAsync(string username, string pin, Guid terminalId)
    {
        var users = await _userRepo.FindAsync(u => u.Username == username && u.IsActive);
        var user = users.FirstOrDefault();

        if (user == null)
            return AuthResult.Failure("Invalid credentials");

        if (!VerifyPin(pin, user.PinHash, user.PinSalt))
            return AuthResult.Failure("Invalid credentials");

        var terminal = await _terminalRepo.GetByIdAsync(terminalId);
        if (terminal == null)
            return AuthResult.Failure("Terminal not registered");

        if (user.StoreId.HasValue && user.StoreId != terminal.StoreId)
            return AuthResult.Failure("User not assigned to this store");

        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TerminalId = terminalId,
            LoginAt = DateTime.UtcNow,
            IsActive = true
        };
        await _sessionRepo.AddAsync(session);

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);

        return AuthResult.Success(user, session.Id);
    }

    public async Task LogoutAsync(Guid sessionId)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session != null)
        {
            session.LogoutAt = DateTime.UtcNow;
            session.IsActive = false;
            await _sessionRepo.UpdateAsync(session);
        }
    }

    private static readonly System.Text.Encoding _enc = System.Text.Encoding.UTF8;

    public static (string hash, string salt) HashPin(string pin)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToHexString(saltBytes).ToLowerInvariant();
        var hash = Convert.ToHexString(Rfc2898DeriveBytes.Pbkdf2(
            _enc.GetBytes(pin), saltBytes, 100_000, HashAlgorithmName.SHA256, 32)).ToLowerInvariant();
        return (hash, salt);
    }

    public static bool VerifyPin(string pin, string hash, string? salt)
    {
        if (string.IsNullOrEmpty(salt))
            return LegacyHash(pin) == hash;

        var saltBytes = Convert.FromHexString(salt);
        var computed = Convert.ToHexString(Rfc2898DeriveBytes.Pbkdf2(
            _enc.GetBytes(pin), saltBytes, 100_000, HashAlgorithmName.SHA256, 32)).ToLowerInvariant();
        return computed == hash;
    }

    private static string LegacyHash(string pin)
    {
        var bytes = SHA256.HashData(_enc.GetBytes(pin));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public User? User { get; set; }
    public Guid? SessionId { get; set; }

    public static AuthResult Success(User user, Guid sessionId) => new()
    {
        IsSuccess = true,
        User = user,
        SessionId = sessionId
    };

    public static AuthResult Failure(string message) => new()
    {
        IsSuccess = false,
        ErrorMessage = message
    };
}

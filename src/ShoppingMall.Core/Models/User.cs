namespace ShoppingMall.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid? StoreId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PinHash { get; set; } = string.Empty;
    public string? PinSalt { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public UserRole Role { get; set; } = UserRole.Cashier;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Store? Store { get; set; }
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}

public class Session
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TerminalId { get; set; }
    public DateTime LoginAt { get; set; } = DateTime.UtcNow;
    public DateTime? LogoutAt { get; set; }
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
    public Terminal Terminal { get; set; } = null!;
}

public class RolePermission
{
    public Guid Id { get; set; }
    public UserRole Role { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string? Resource { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}

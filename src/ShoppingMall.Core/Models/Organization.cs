namespace ShoppingMall.Core.Models;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? GSTIN { get; set; }
    public string? PAN { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Store
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? GSTIN { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public StoreStatus Status { get; set; } = StoreStatus.Active;
    public string? ReceiptFooter { get; set; }
    public string? TimeZone { get; set; } = "Asia/Kolkata";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Organization Organization { get; set; } = null!;
    public ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();
    public ICollection<StoreConfig> Configs { get; set; } = new List<StoreConfig>();
}

public class Terminal
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public TerminalMode Mode { get; set; } = TerminalMode.Client;
    public string? IpAddress { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastHeartbeat { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Store Store { get; set; } = null!;
}

public class StoreConfig
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Store Store { get; set; } = null!;
}

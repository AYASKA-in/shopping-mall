namespace ShoppingMall.Core.Models;

public class SyncQueue
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public SyncStatus Status { get; set; } = SyncStatus.Pending;
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}

public class SyncLog
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public SyncDirection Direction { get; set; }
    public SyncStatus Status { get; set; }
    public int ItemsProcessed { get; set; }
    public int ItemsFailed { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public class CloudBackup
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? Checksum { get; set; }
    public string? StorageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

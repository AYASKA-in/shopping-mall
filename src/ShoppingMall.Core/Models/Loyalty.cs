namespace ShoppingMall.Core.Models;

public class LoyaltyAccount
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? CardNumber { get; set; }
    public int PointsBalance { get; set; }
    public int LifetimePoints { get; set; }
    public LoyaltyTier Tier { get; set; } = LoyaltyTier.Bronze;
    public DateTime? LastEarnedAt { get; set; }
    public DateTime? LastRedeemedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;
    public ICollection<LoyaltyTransaction> Transactions { get; set; } = new List<LoyaltyTransaction>();
}

public class LoyaltyTransaction
{
    public Guid Id { get; set; }
    public Guid LoyaltyAccountId { get; set; }
    public LoyaltyTransactionType Type { get; set; }
    public int Points { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? Description { get; set; }
    public Guid? IdempotencyKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public LoyaltyAccount LoyaltyAccount { get; set; } = null!;
}

public class TierConfig
{
    public Guid Id { get; set; }
    public LoyaltyTier Tier { get; set; }
    public int MinimumLifetimePoints { get; set; }
    public decimal PointsMultiplier { get; set; } = 1.0m;
    public string? Benefits { get; set; }
    public bool IsActive { get; set; } = true;
}

namespace ShoppingMall.Core.Enums;

public enum LoyaltyTier
{
    Bronze,
    Silver,
    Gold,
    Platinum
}

public enum LoyaltyTransactionType
{
    Earn,
    Redeem,
    Expire,
    Adjust,
    Reverse,
    Bonus
}

public enum SyncDirection
{
    Upload,
    Download
}

public enum SyncStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

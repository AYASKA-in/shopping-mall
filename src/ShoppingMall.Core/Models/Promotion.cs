namespace ShoppingMall.Core.Models;

public class Promotion
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PromotionType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public PromotionStackability Stackability { get; set; } = PromotionStackability.Stackable;
    public decimal? Budget { get; set; }
    public decimal? DailyBudget { get; set; }
    public decimal? BudgetSpent { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedByUserId { get; set; }

    public ICollection<PromotionRuleGroup> RuleGroups { get; set; } = new List<PromotionRuleGroup>();
    public ICollection<PromotionBenefit> Benefits { get; set; } = new List<PromotionBenefit>();
}

public class PromotionRuleGroup
{
    public Guid Id { get; set; }
    public Guid PromotionId { get; set; }
    public string Combinator { get; set; } = "AND";
    public int SortOrder { get; set; }

    public Promotion Promotion { get; set; } = null!;
    public ICollection<PromotionRule> Rules { get; set; } = new List<PromotionRule>();
}

public class PromotionRule
{
    public Guid Id { get; set; }
    public Guid RuleGroupId { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public PromotionRuleGroup RuleGroup { get; set; } = null!;
}

public class PromotionBenefit
{
    public Guid Id { get; set; }
    public Guid PromotionId { get; set; }
    public string BenefitType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int? BuyQty { get; set; }
    public int? GetQty { get; set; }
    public Guid? FreeProductId { get; set; }
    public string? ApplicableOn { get; set; } = "ALL";

    public Promotion Promotion { get; set; } = null!;
}

public class CouponCampaign
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? PromotionId { get; set; }
    public int MaxUses { get; set; }
    public int? MaxUsesPerCustomer { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<CouponCode> Codes { get; set; } = new List<CouponCode>();
}

public class CouponCode
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Code { get; set; } = string.Empty;
    public CouponStatus Status { get; set; } = CouponStatus.Active;
    public int UseCount { get; set; }
    public DateTime? UsedAt { get; set; }

    public CouponCampaign Campaign { get; set; } = null!;
}

public class PromotionUsage
{
    public Guid Id { get; set; }
    public Guid PromotionId { get; set; }
    public Guid TransactionId { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid? CouponCodeId { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("promotions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Stackability).HasConversion<string>().HasMaxLength(15);
    }
}

public class PromotionRuleGroupConfiguration : IEntityTypeConfiguration<PromotionRuleGroup>
{
    public void Configure(EntityTypeBuilder<PromotionRuleGroup> builder)
    {
        builder.ToTable("promotion_rule_groups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Combinator).HasMaxLength(5).IsRequired();
        builder.HasOne(x => x.Promotion).WithMany(p => p.RuleGroups).HasForeignKey(x => x.PromotionId);
    }
}

public class PromotionRuleConfiguration : IEntityTypeConfiguration<PromotionRule>
{
    public void Configure(EntityTypeBuilder<PromotionRule> builder)
    {
        builder.ToTable("promotion_rules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RuleType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Operator).HasMaxLength(20).IsRequired();
        builder.HasOne(x => x.RuleGroup).WithMany(g => g.Rules).HasForeignKey(x => x.RuleGroupId);
    }
}

public class PromotionBenefitConfiguration : IEntityTypeConfiguration<PromotionBenefit>
{
    public void Configure(EntityTypeBuilder<PromotionBenefit> builder)
    {
        builder.ToTable("promotion_benefits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BenefitType).HasMaxLength(30).IsRequired();
        builder.HasOne(x => x.Promotion).WithMany(p => p.Benefits).HasForeignKey(x => x.PromotionId);
    }
}

public class CouponCampaignConfiguration : IEntityTypeConfiguration<CouponCampaign>
{
    public void Configure(EntityTypeBuilder<CouponCampaign> builder)
    {
        builder.ToTable("coupon_campaigns");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
    }
}

public class CouponCodeConfiguration : IEntityTypeConfiguration<CouponCode>
{
    public void Configure(EntityTypeBuilder<CouponCode> builder)
    {
        builder.ToTable("coupon_codes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(x => x.Campaign).WithMany(c => c.Codes).HasForeignKey(x => x.CampaignId);
    }
}

public class PromotionUsageConfiguration : IEntityTypeConfiguration<PromotionUsage>
{
    public void Configure(EntityTypeBuilder<PromotionUsage> builder)
    {
        builder.ToTable("promotion_usages");
        builder.HasKey(x => x.Id);
    }
}

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("loyalty_accounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Tier).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(x => x.Customer).WithOne(c => c.LoyaltyAccount).HasForeignKey<LoyaltyAccount>(x => x.CustomerId);
    }
}

public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.ToTable("loyalty_transactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(x => x.LoyaltyAccount).WithMany(a => a.Transactions).HasForeignKey(x => x.LoyaltyAccountId);
        builder.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("\"IdempotencyKey\" IS NOT NULL");
    }
}

public class TierConfigConfiguration : IEntityTypeConfiguration<TierConfig>
{
    public void Configure(EntityTypeBuilder<TierConfig> builder)
    {
        builder.ToTable("tier_configs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Tier).HasConversion<string>().HasMaxLength(10);
    }
}

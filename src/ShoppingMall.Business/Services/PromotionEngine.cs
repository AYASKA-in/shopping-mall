using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class PromotionEngine
{
    private readonly IRepository<Promotion> _promotionRepo;
    private readonly IRepository<CouponCampaign> _campaignRepo;
    private readonly IRepository<CouponCode> _couponRepo;
    private readonly IRepository<PromotionUsage> _usageRepo;

    public PromotionEngine(
        IRepository<Promotion> promotionRepo,
        IRepository<CouponCampaign> campaignRepo,
        IRepository<CouponCode> couponRepo,
        IRepository<PromotionUsage> usageRepo)
    {
        _promotionRepo = promotionRepo;
        _campaignRepo = campaignRepo;
        _couponRepo = couponRepo;
        _usageRepo = usageRepo;
    }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
    {
        var now = DateTime.UtcNow;
        return await _promotionRepo.FindAsync(p =>
            p.IsActive && p.StartDate <= now && p.EndDate >= now);
    }

    public async Task<PromotionResult> EvaluateAsync(CartContext cart, string? couponCode = null)
    {
        var result = new PromotionResult();
        var promotions = (await GetActivePromotionsAsync()).ToList();
        if (!promotions.Any()) return result;

        CouponCampaign? campaign = null;
        if (!string.IsNullOrEmpty(couponCode))
        {
            var codes = await _couponRepo.FindAsync(c =>
                c.Code == couponCode && c.Status == Core.Enums.CouponStatus.Active);
            var coupon = codes.FirstOrDefault();
            if (coupon == null)
            {
                result.Errors.Add("Invalid or expired coupon code");
                return result;
            }
            var campaigns = await _campaignRepo.FindAsync(c => c.Id == coupon.CampaignId);
            campaign = campaigns.FirstOrDefault();
            if (campaign == null || !campaign.IsActive || campaign.EndDate < DateTime.UtcNow)
            {
                result.Errors.Add("Coupon campaign is no longer active");
                return result;
            }
            if (campaign.MinOrderAmount.HasValue && cart.SubTotal < campaign.MinOrderAmount.Value)
            {
                result.Errors.Add($"Minimum order amount of \u20B9{campaign.MinOrderAmount:F0} required");
                return result;
            }
            result.CouponCodeId = coupon.Id;
            result.CouponCampaign = campaign;
        }

        foreach (var promotion in promotions.OrderBy(p => p.Priority))
        {
            if (!promotion.IsActive || DateTime.UtcNow < promotion.StartDate || DateTime.UtcNow > promotion.EndDate)
                continue;

            if (campaign?.PromotionId != null && campaign.PromotionId != promotion.Id)
                continue;

            var eligible = EvaluateRules(promotion.RuleGroups, cart);
            if (!eligible) continue;

            foreach (var benefit in promotion.Benefits)
            {
                var discounts = CalculateDiscount(benefit, cart);
                if (discounts.Count == 0) continue;

                foreach (var d in discounts)
                {
                    result.AppliedPromotions.Add(new AppliedPromotion
                    {
                        PromotionId = promotion.Id,
                        PromotionName = promotion.Name,
                        DiscountAmount = d.DiscountAmount,
                        BenefitType = benefit.BenefitType,
                        LineItemIndex = d.LineItemIndex
                    });
                    result.TotalDiscount += d.DiscountAmount;
                }
            }

            if (promotion.Stackability == Core.Enums.PromotionStackability.Exclusive)
                break;
        }

        return result;
    }

    public async Task<CouponValidationResult> ValidateCouponAsync(string code, decimal cartTotal)
    {
        var result = new CouponValidationResult { IsValid = false };

        var codes = await _couponRepo.FindAsync(c =>
            c.Code == code && c.Status == Core.Enums.CouponStatus.Active);
        var coupon = codes.FirstOrDefault();
        if (coupon == null)
        {
            result.Error = "Invalid or expired coupon code";
            return result;
        }

        var campaigns = await _campaignRepo.FindAsync(c => c.Id == coupon.CampaignId);
        var campaign = campaigns.FirstOrDefault();
        if (campaign == null || !campaign.IsActive)
        {
            result.Error = "Coupon campaign is no longer active";
            return result;
        }

        if (campaign.EndDate < DateTime.UtcNow)
        {
            result.Error = "Coupon campaign has expired";
            return result;
        }

        if (campaign.MaxUses > 0 && coupon.UseCount >= campaign.MaxUses)
        {
            result.Error = "Coupon has reached maximum usage limit";
            return result;
        }

        if (campaign.MinOrderAmount.HasValue && cartTotal < campaign.MinOrderAmount.Value)
        {
            result.Error = $"Minimum order amount of \u20B9{campaign.MinOrderAmount:F0} required";
            return result;
        }

        result.IsValid = true;
        result.CampaignName = campaign.Name;
        result.PromotionId = campaign.PromotionId;
        return result;
    }

    public async Task RecordUsageAsync(Guid promotionId, Guid transactionId, decimal discountAmount, Guid? couponCodeId)
    {
        await _usageRepo.AddAsync(new PromotionUsage
        {
            Id = Guid.NewGuid(),
            PromotionId = promotionId,
            TransactionId = transactionId,
            DiscountAmount = discountAmount,
            CouponCodeId = couponCodeId,
            UsedAt = DateTime.UtcNow
        });

        if (couponCodeId.HasValue)
        {
            var coupon = await _couponRepo.GetByIdAsync(couponCodeId.Value);
            if (coupon != null)
            {
                coupon.UseCount++;
                coupon.UsedAt = DateTime.UtcNow;
                if (coupon.Campaign.MaxUses > 0 && coupon.UseCount >= coupon.Campaign.MaxUses)
                    coupon.Status = Core.Enums.CouponStatus.Used;
                await _couponRepo.UpdateAsync(coupon);
            }
        }
    }

    private bool EvaluateRules(ICollection<PromotionRuleGroup> ruleGroups, CartContext cart)
    {
        if (ruleGroups.Count == 0) return true;

        foreach (var group in ruleGroups)
        {
            var groupResult = group.Combinator == "AND"
                ? group.Rules.All(r => EvaluateRule(r, cart))
                : group.Rules.Any(r => EvaluateRule(r, cart));

            if (groupResult) return true;
        }

        return false;
    }

    private bool EvaluateRule(PromotionRule rule, CartContext cart)
    {
        return rule.RuleType switch
        {
            "MIN_CART_VALUE" => cart.SubTotal >= decimal.Parse(rule.Value),
            "MIN_QUANTITY" => cart.TotalQuantity >= int.Parse(rule.Value),
            "PRODUCT_CATEGORY" => cart.Items.Any(i => i.CategoryId.ToString() == rule.Value),
            "SPECIFIC_PRODUCT" => cart.Items.Any(i => i.ProductId.ToString() == rule.Value),
            "DAY_OF_WEEK" => ((int)DateTime.UtcNow.DayOfWeek).ToString() == rule.Value,
            "TIME_RANGE" => EvaluateTimeRange(rule.Value),
            "CUSTOMER_GROUP" => true,
            "MIN_LINE_QUANTITY" => cart.Items.Any(i => i.Quantity >= int.Parse(rule.Value)),
            _ => false
        };
    }

    private bool EvaluateTimeRange(string value)
    {
        var parts = value.Split('-');
        if (parts.Length != 2) return false;
        if (!TimeSpan.TryParse(parts[0], out var start)) return false;
        if (!TimeSpan.TryParse(parts[1], out var end)) return false;
        var now = DateTime.UtcNow.TimeOfDay;
        return now >= start && now <= end;
    }

    private List<DiscountInfo> CalculateDiscount(PromotionBenefit benefit, CartContext cart)
    {
        var discounts = new List<DiscountInfo>();
        var applicableItems = GetApplicableItems(benefit, cart);

        switch (benefit.BenefitType)
        {
            case "PERCENTAGE_OFF":
                foreach (var item in applicableItems)
                {
                    var discount = Math.Round(item.LineTotal * benefit.Value / 100, 2);
                    if (benefit.MaxDiscount.HasValue && discount > benefit.MaxDiscount.Value)
                        discount = benefit.MaxDiscount.Value;
                    discounts.Add(new DiscountInfo
                    {
                        DiscountAmount = discount,
                        LineItemIndex = item.Index,
                        BenefitType = benefit.BenefitType
                    });
                }
                break;

            case "FIXED_AMOUNT_OFF":
            case "CART_DISCOUNT":
                var cartDiscount = benefit.Value;
                if (benefit.MaxDiscount.HasValue && cartDiscount > benefit.MaxDiscount.Value)
                    cartDiscount = benefit.MaxDiscount.Value;
                if (applicableItems.Count > 0)
                {
                    var perItem = Math.Round(cartDiscount / applicableItems.Count, 2);
                    foreach (var item in applicableItems)
                    {
                        discounts.Add(new DiscountInfo
                        {
                            DiscountAmount = Math.Min(perItem, item.LineTotal),
                            LineItemIndex = item.Index,
                            BenefitType = benefit.BenefitType
                        });
                    }
                }
                break;

            case "BUY_X_GET_Y":
                if (benefit.BuyQty.HasValue && benefit.GetQty.HasValue)
                {
                    foreach (var item in applicableItems)
                    {
                        var eligibleSets = (int)Math.Floor(item.Quantity / benefit.BuyQty.Value);
                        var freeQty = eligibleSets * benefit.GetQty.Value;
                        if (freeQty > 0)
                        {
                            var discount = Math.Round(freeQty * item.UnitPrice, 2);
                            discounts.Add(new DiscountInfo
                            {
                                DiscountAmount = discount,
                                LineItemIndex = item.Index,
                                BenefitType = benefit.BenefitType,
                                FreeQuantity = freeQty
                            });
                        }
                    }
                }
                break;

            case "FREE_ITEM":
                if (benefit.FreeProductId.HasValue)
                {
                    var freeIdx = cart.Items.FindIndex(i => i.ProductId == benefit.FreeProductId.Value);
                    if (freeIdx >= 0)
                    {
                        var freeItem = cart.Items[freeIdx];
                        discounts.Add(new DiscountInfo
                        {
                            DiscountAmount = freeItem.LineTotal,
                            LineItemIndex = freeIdx,
                            BenefitType = benefit.BenefitType
                        });
                    }
                }
                break;

            case "BUNDLE":
                var bundleDiscount = benefit.Value;
                if (benefit.MaxDiscount.HasValue && bundleDiscount > benefit.MaxDiscount.Value)
                    bundleDiscount = benefit.MaxDiscount.Value;
                if (applicableItems.Count >= 2)
                {
                    var perItemDiscount = Math.Round(bundleDiscount / applicableItems.Count, 2);
                    foreach (var item in applicableItems)
                    {
                        discounts.Add(new DiscountInfo
                        {
                            DiscountAmount = Math.Min(perItemDiscount, item.LineTotal),
                            LineItemIndex = item.Index,
                            BenefitType = benefit.BenefitType
                        });
                    }
                }
                break;

            case "TIERED_DISCOUNT":
                foreach (var item in applicableItems)
                {
                    var tierDiscount = item.Quantity >= 10 ? benefit.Value * 1.5m
                        : item.Quantity >= 5 ? benefit.Value
                        : item.Quantity >= 3 ? benefit.Value * 0.5m
                        : 0;
                    if (tierDiscount > 0)
                    {
                        var discount = Math.Round(item.LineTotal * tierDiscount / 100, 2);
                        if (benefit.MaxDiscount.HasValue && discount > benefit.MaxDiscount.Value)
                            discount = benefit.MaxDiscount.Value;
                        discounts.Add(new DiscountInfo
                        {
                            DiscountAmount = discount,
                            LineItemIndex = item.Index,
                            BenefitType = benefit.BenefitType
                        });
                    }
                }
                break;
        }

        return discounts;
    }

    private List<CartItemInfo> GetApplicableItems(PromotionBenefit benefit, CartContext cart)
    {
        var items = cart.Items.Select((item, idx) => new CartItemInfo
        {
            ProductId = item.ProductId,
            CategoryId = item.CategoryId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal,
            Index = idx
        }).ToList();

        if (string.IsNullOrEmpty(benefit.ApplicableOn) || benefit.ApplicableOn == "ALL")
            return items;

        if (benefit.ApplicableOn == "CHEAPEST")
            return items.OrderBy(i => i.UnitPrice).Take(1).ToList();

        if (benefit.ApplicableOn == "MOST_EXPENSIVE")
            return items.OrderByDescending(i => i.UnitPrice).Take(1).ToList();

        return items;
    }
}

public class CartContext
{
    public decimal SubTotal { get; set; }
    public int TotalQuantity { get; set; }
    public List<CartItem> Items { get; set; } = new();
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public class CartItemInfo
{
    public Guid ProductId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int Index { get; set; }
}

public class PromotionResult
{
    public decimal TotalDiscount { get; set; }
    public List<AppliedPromotion> AppliedPromotions { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public Guid? CouponCodeId { get; set; }
    public CouponCampaign? CouponCampaign { get; set; }
}

public class AppliedPromotion
{
    public Guid PromotionId { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public string BenefitType { get; set; } = string.Empty;
    public int? LineItemIndex { get; set; }
    public int? FreeQuantity { get; set; }
}

public class DiscountInfo
{
    public decimal DiscountAmount { get; set; }
    public int? LineItemIndex { get; set; }
    public string BenefitType { get; set; } = string.Empty;
    public int? FreeQuantity { get; set; }
}

public class CouponValidationResult
{
    public bool IsValid { get; set; }
    public string? Error { get; set; }
    public string? CampaignName { get; set; }
    public Guid? PromotionId { get; set; }
}

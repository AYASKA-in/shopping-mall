using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class PromotionEngine
{
    public PromotionResult Evaluate(IEnumerable<Promotion> promotions, CartContext cart)
    {
        var result = new PromotionResult();

        foreach (var promotion in promotions.OrderBy(p => p.Priority))
        {
            if (!promotion.IsActive || DateTime.UtcNow < promotion.StartDate || DateTime.UtcNow > promotion.EndDate)
                continue;

            var eligible = EvaluateRules(promotion.RuleGroups, cart);
            if (!eligible) continue;

            var benefit = promotion.Benefits.FirstOrDefault();
            if (benefit == null) continue;

            var discount = CalculateDiscount(benefit, cart);
            if (discount <= 0) continue;

            result.AppliedPromotions.Add(new AppliedPromotion
            {
                PromotionId = promotion.Id,
                PromotionName = promotion.Name,
                DiscountAmount = discount,
                BenefitType = benefit.BenefitType
            });

            result.TotalDiscount += discount;

            if (promotion.Stackability == Core.Enums.PromotionStackability.Exclusive)
                break;
        }

        return result;
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

    private decimal CalculateDiscount(PromotionBenefit benefit, CartContext cart)
    {
        var discount = benefit.BenefitType switch
        {
            "PERCENTAGE_OFF" => cart.SubTotal * benefit.Value / 100,
            "FIXED_AMOUNT_OFF" => benefit.Value,
            "CART_DISCOUNT" => benefit.Value,
            _ => 0
        };

        if (benefit.MaxDiscount.HasValue && discount > benefit.MaxDiscount.Value)
            discount = benefit.MaxDiscount.Value;

        return Math.Min(discount, cart.SubTotal);
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
}

public class PromotionResult
{
    public decimal TotalDiscount { get; set; }
    public List<AppliedPromotion> AppliedPromotions { get; set; } = new();
}

public class AppliedPromotion
{
    public Guid PromotionId { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public string BenefitType { get; set; } = string.Empty;
}

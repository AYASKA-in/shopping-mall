using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class PromotionEngineTests
{
    private readonly Mock<IRepository<Promotion>> _promoRepo = new();
    private readonly Mock<IRepository<CouponCampaign>> _campaignRepo = new();
    private readonly Mock<IRepository<CouponCode>> _couponRepo = new();
    private readonly Mock<IRepository<PromotionUsage>> _usageRepo = new();
    private readonly PromotionEngine _sut;

    public PromotionEngineTests()
    {
        _sut = new PromotionEngine(_promoRepo.Object, _campaignRepo.Object, _couponRepo.Object, _usageRepo.Object);
    }

    [Fact]
    public async Task GetActivePromotionsAsync_ReturnsOnlyActive()
    {
        var now = DateTime.UtcNow;
        var promotions = new List<Promotion>
        {
            new() { Id = Guid.NewGuid(), Name = "Active", IsActive = true, StartDate = now.AddDays(-1), EndDate = now.AddDays(1) },
            new() { Id = Guid.NewGuid(), Name = "Expired", IsActive = true, StartDate = now.AddDays(-2), EndDate = now.AddDays(-1) },
            new() { Id = Guid.NewGuid(), Name = "Inactive", IsActive = false, StartDate = now.AddDays(-1), EndDate = now.AddDays(1) }
        };

        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(promotions.Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now));

        var result = await _sut.GetActivePromotionsAsync();

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active");
    }

    [Fact]
    public async Task EvaluateAsync_NoActivePromotions_ReturnsEmptyResult()
    {
        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(Enumerable.Empty<Promotion>());

        var cart = new CartContext { SubTotal = 100 };
        var result = await _sut.EvaluateAsync(cart);

        result.TotalDiscount.Should().Be(0);
        result.AppliedPromotions.Should().BeEmpty();
    }

    [Fact]
    public async Task EvaluateAsync_MinCartValueMet_AppliesPercentDiscount()
    {
        var cart = new CartContext
        {
            SubTotal = 500,
            Items = new List<CartItem>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 500 }
            }
        };

        var promo = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "10% Off",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            RuleGroups = new List<PromotionRuleGroup>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Combinator = "AND",
                    Rules = new List<PromotionRule>
                    {
                        new() { RuleType = "MIN_CART_VALUE", Operator = ">=", Value = "200" }
                    }
                }
            },
            Benefits = new List<PromotionBenefit>
            {
                new() { BenefitType = "PERCENTAGE_OFF", Value = 10 }
            }
        };

        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(new[] { promo });

        var result = await _sut.EvaluateAsync(cart);

        result.TotalDiscount.Should().Be(50);
        result.AppliedPromotions.Should().ContainSingle();
        result.AppliedPromotions[0].PromotionName.Should().Be("10% Off");
        result.AppliedPromotions[0].DiscountAmount.Should().Be(50);
    }

    [Fact]
    public async Task EvaluateAsync_MinCartValueNotMet_NoDiscount()
    {
        var cart = new CartContext { SubTotal = 100 };

        var promo = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "10% Off",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            RuleGroups = new List<PromotionRuleGroup>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Rules = new List<PromotionRule>
                    {
                        new() { RuleType = "MIN_CART_VALUE", Operator = ">=", Value = "200" }
                    }
                }
            },
            Benefits = new List<PromotionBenefit>
            {
                new() { BenefitType = "PERCENTAGE_OFF", Value = 10 }
            }
        };

        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(new[] { promo });

        var result = await _sut.EvaluateAsync(cart);

        result.TotalDiscount.Should().Be(0);
    }

    [Fact]
    public async Task EvaluateAsync_BuyXGetY_AppliesDiscount()
    {
        var productId = Guid.NewGuid();
        var cart = new CartContext
        {
            Items = new List<CartItem>
            {
                new() { ProductId = productId, Quantity = 3, UnitPrice = 100 }
            }
        };

        var promo = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "Buy 2 Get 1 Free",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            RuleGroups = new List<PromotionRuleGroup>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Rules = new List<PromotionRule>
                    {
                        new() { RuleType = "MIN_LINE_QUANTITY", Operator = ">=", Value = "2" }
                    }
                }
            },
            Benefits = new List<PromotionBenefit>
            {
                new() { BenefitType = "BUY_X_GET_Y", BuyQty = 2, GetQty = 1 }
            }
        };

        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(new[] { promo });

        var result = await _sut.EvaluateAsync(cart);

        result.TotalDiscount.Should().Be(100);
        result.AppliedPromotions.Should().ContainSingle();
    }

    [Fact]
    public async Task EvaluateAsync_FreeItem_DiscountsFreeItemLine()
    {
        var freeProductId = Guid.NewGuid();
        var cart = new CartContext
        {
            SubTotal = 250,
            Items = new List<CartItem>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 200 },
                new() { ProductId = freeProductId, Quantity = 1, UnitPrice = 50 }
            }
        };

        var promo = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "Free Gift",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            RuleGroups = new List<PromotionRuleGroup>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Rules = new List<PromotionRule>
                    {
                        new() { RuleType = "MIN_CART_VALUE", Operator = ">=", Value = "100" }
                    }
                }
            },
            Benefits = new List<PromotionBenefit>
            {
                new() { BenefitType = "FREE_ITEM", FreeProductId = freeProductId }
            }
        };

        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(new[] { promo });

        var result = await _sut.EvaluateAsync(cart);

        result.TotalDiscount.Should().Be(50);
    }

    [Fact]
    public async Task EvaluateAsync_Exclusive_StopsAfterFirstMatch()
    {
        var cart = new CartContext
        {
            SubTotal = 1000,
            Items = new List<CartItem>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 1000 }
            }
        };

        var promo1 = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "First - Exclusive 10%",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            Priority = 1,
            Stackability = ShoppingMall.Core.Enums.PromotionStackability.Exclusive,
            RuleGroups = new List<PromotionRuleGroup>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Rules = new List<PromotionRule>
                    {
                        new() { RuleType = "MIN_CART_VALUE", Operator = ">=", Value = "1" }
                    }
                }
            },
            Benefits = new List<PromotionBenefit>
            {
                new() { BenefitType = "PERCENTAGE_OFF", Value = 10 }
            }
        };

        var promo2 = new Promotion
        {
            Id = Guid.NewGuid(),
            Name = "Second 20% (should NOT apply)",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            Priority = 2,
            RuleGroups = new List<PromotionRuleGroup>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Rules = new List<PromotionRule>
                    {
                        new() { RuleType = "MIN_CART_VALUE", Operator = ">=", Value = "1" }
                    }
                }
            },
            Benefits = new List<PromotionBenefit>
            {
                new() { BenefitType = "PERCENTAGE_OFF", Value = 20 }
            }
        };

        _promoRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Promotion, bool>>>()))
            .ReturnsAsync(new[] { promo1, promo2 });

        var result = await _sut.EvaluateAsync(cart);

        result.AppliedPromotions.Should().HaveCount(1);
        result.AppliedPromotions[0].PromotionName.Should().Be("First - Exclusive 10%");
        result.TotalDiscount.Should().Be(100);
    }

    [Fact]
    public async Task ValidateCouponAsync_ValidCoupon_ReturnsValid()
    {
        var campaign = new CouponCampaign
        {
            Id = Guid.NewGuid(),
            Name = "Test Campaign",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            MaxUses = 100
        };

        var coupon = new CouponCode
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            Code = "SAVE10",
            Status = ShoppingMall.Core.Enums.CouponStatus.Active,
            UseCount = 0
        };
        coupon.Campaign = campaign;

        _couponRepo.Setup(r => r.FindAsync(c =>
            c.Code == "SAVE10" && c.Status == ShoppingMall.Core.Enums.CouponStatus.Active))
            .ReturnsAsync(new[] { coupon });
        _campaignRepo.Setup(r => r.FindAsync(c => c.Id == coupon.CampaignId))
            .ReturnsAsync(new[] { campaign });

        var result = await _sut.ValidateCouponAsync("SAVE10", 500);

        result.IsValid.Should().BeTrue();
        result.CampaignName.Should().Be("Test Campaign");
    }

    [Fact]
    public async Task ValidateCouponAsync_ExpiredCampaign_ReturnsInvalid()
    {
        var campaign = new CouponCampaign
        {
            Id = Guid.NewGuid(),
            Name = "Expired",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(-5),
            MaxUses = 100
        };

        var coupon = new CouponCode
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            Code = "EXPIRED",
            Status = ShoppingMall.Core.Enums.CouponStatus.Active,
            UseCount = 0
        };

        _couponRepo.Setup(r => r.FindAsync(c =>
            c.Code == "EXPIRED" && c.Status == ShoppingMall.Core.Enums.CouponStatus.Active))
            .ReturnsAsync(new[] { coupon });
        _campaignRepo.Setup(r => r.FindAsync(c => c.Id == coupon.CampaignId))
            .ReturnsAsync(new[] { campaign });

        var result = await _sut.ValidateCouponAsync("EXPIRED", 500);

        result.IsValid.Should().BeFalse();
        result.Error.Should().Be("Coupon campaign has expired");
    }

    [Fact]
    public async Task ValidateCouponAsync_MaxUsesReached_ReturnsInvalid()
    {
        var campaign = new CouponCampaign
        {
            Id = Guid.NewGuid(),
            Name = "Maxed",
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            MaxUses = 5
        };

        var coupon = new CouponCode
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            Code = "MAXED",
            Status = ShoppingMall.Core.Enums.CouponStatus.Active,
            UseCount = 5
        };

        _couponRepo.Setup(r => r.FindAsync(c =>
            c.Code == "MAXED" && c.Status == ShoppingMall.Core.Enums.CouponStatus.Active))
            .ReturnsAsync(new[] { coupon });
        _campaignRepo.Setup(r => r.FindAsync(c => c.Id == coupon.CampaignId))
            .ReturnsAsync(new[] { campaign });

        var result = await _sut.ValidateCouponAsync("MAXED", 500);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task RecordUsageAsync_IncrementsUseCount()
    {
        var couponId = Guid.NewGuid();
        var campaign = new CouponCampaign
        {
            Id = Guid.NewGuid(),
            Name = "Camp",
            MaxUses = 50,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1)
        };
        var coupon = new CouponCode
        {
            Id = couponId,
            CampaignId = campaign.Id,
            Code = "TEST",
            UseCount = 0,
            Status = ShoppingMall.Core.Enums.CouponStatus.Active
        };
        coupon.Campaign = campaign;

        _couponRepo.Setup(r => r.GetByIdAsync(couponId)).ReturnsAsync(coupon);
        _usageRepo.Setup(r => r.AddAsync(It.IsAny<PromotionUsage>())).ReturnsAsync((PromotionUsage u) => u);

        await _sut.RecordUsageAsync(Guid.NewGuid(), Guid.NewGuid(), 50, couponId);

        coupon.UseCount.Should().Be(1);
        _couponRepo.Verify(r => r.UpdateAsync(coupon), Times.Once);
    }
}

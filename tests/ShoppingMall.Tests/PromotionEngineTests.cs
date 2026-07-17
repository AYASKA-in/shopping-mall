using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
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
    public void ConvertPointsToCurrency_100Points_Returns1Rupee()
    {
        var svc = new LoyaltyService(
            new Mock<IRepository<LoyaltyAccount>>().Object,
            new Mock<IRepository<LoyaltyTransaction>>().Object,
            new Mock<IRepository<TierConfig>>().Object,
            new Mock<IRepository<Customer>>().Object);

        var result = svc.ConvertPointsToCurrency(100);
        result.Should().Be(1);
    }
}

using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Enums;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class LoyaltyServiceTests
{
    private readonly Mock<IRepository<LoyaltyAccount>> _accountRepo = new();
    private readonly Mock<IRepository<LoyaltyTransaction>> _txnRepo = new();
    private readonly Mock<IRepository<TierConfig>> _tierRepo = new();
    private readonly Mock<IRepository<Customer>> _customerRepo = new();
    private readonly LoyaltyService _sut;

    public LoyaltyServiceTests()
    {
        _sut = new LoyaltyService(_accountRepo.Object, _txnRepo.Object, _tierRepo.Object, _customerRepo.Object);
    }

    [Fact]
    public async Task GetOrCreateAccountAsync_Existing_ReturnsIt()
    {
        var customerId = Guid.NewGuid();
        var account = new LoyaltyAccount { Id = Guid.NewGuid(), CustomerId = customerId, PointsBalance = 500 };

        _accountRepo.Setup(r => r.FindAsync(a => a.CustomerId == customerId))
            .ReturnsAsync(new[] { account });

        var result = await _sut.GetOrCreateAccountAsync(customerId);

        result.Should().Be(account);
        result.PointsBalance.Should().Be(500);
    }

    [Fact]
    public async Task GetOrCreateAccountAsync_New_Creates()
    {
        var customerId = Guid.NewGuid();

        _accountRepo.Setup(r => r.FindAsync(a => a.CustomerId == customerId))
            .ReturnsAsync(Enumerable.Empty<LoyaltyAccount>());
        _accountRepo.Setup(r => r.AddAsync(It.IsAny<LoyaltyAccount>()))
            .ReturnsAsync((LoyaltyAccount a) => a);

        var result = await _sut.GetOrCreateAccountAsync(customerId);

        result.CustomerId.Should().Be(customerId);
        result.PointsBalance.Should().Be(0);
        result.Tier.Should().Be(LoyaltyTier.Bronze);
    }

    [Fact]
    public async Task EarnPointsAsync_AddsPoints()
    {
        var customerId = Guid.NewGuid();
        var txnId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PointsBalance = 100,
            LifetimePoints = 100,
            Tier = LoyaltyTier.Bronze
        };

        _accountRepo.Setup(r => r.FindAsync(a => a.CustomerId == customerId))
            .ReturnsAsync(new[] { account });
        _tierRepo.Setup(r => r.FindAsync(t => t.Tier == LoyaltyTier.Bronze && t.IsActive))
            .ReturnsAsync(new[] { new TierConfig { Tier = LoyaltyTier.Bronze, PointsMultiplier = 1.0m } });
        _tierRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new[] { new TierConfig { Tier = LoyaltyTier.Bronze, MinimumLifetimePoints = 0, IsActive = true } });
        _txnRepo.Setup(r => r.AddAsync(It.IsAny<LoyaltyTransaction>()))
            .ReturnsAsync((LoyaltyTransaction t) => t);

        var result = await _sut.EarnPointsAsync(customerId, 250m, txnId);

        result.Points.Should().Be(250);
        result.Type.Should().Be(LoyaltyTransactionType.Earn);
        account.PointsBalance.Should().Be(350);
        account.LifetimePoints.Should().Be(350);
    }

    [Fact]
    public async Task EarnPointsAsync_AppliesTierMultiplier()
    {
        var customerId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PointsBalance = 0,
            LifetimePoints = 5000,
            Tier = LoyaltyTier.Gold
        };

        _accountRepo.Setup(r => r.FindAsync(a => a.CustomerId == customerId))
            .ReturnsAsync(new[] { account });
        _tierRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TierConfig, bool>>>()))
            .ReturnsAsync(new[] { new TierConfig { Tier = LoyaltyTier.Gold, PointsMultiplier = 1.5m } });
        _tierRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new[]
            {
                new TierConfig { Tier = LoyaltyTier.Bronze, MinimumLifetimePoints = 0, IsActive = true },
                new TierConfig { Tier = LoyaltyTier.Silver, MinimumLifetimePoints = 500, IsActive = true },
                new TierConfig { Tier = LoyaltyTier.Gold, MinimumLifetimePoints = 2000, IsActive = true },
            });
        _txnRepo.Setup(r => r.AddAsync(It.IsAny<LoyaltyTransaction>()))
            .ReturnsAsync((LoyaltyTransaction t) => t);

        var result = await _sut.EarnPointsAsync(customerId, 100m, Guid.NewGuid());

        result.Points.Should().Be(150);
    }

    [Fact]
    public async Task RedeemPointsAsync_SufficientPoints_Reduces()
    {
        var customerId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PointsBalance = 500,
            LifetimePoints = 1000
        };

        _accountRepo.Setup(r => r.FindAsync(a => a.CustomerId == customerId))
            .ReturnsAsync(new[] { account });
        _txnRepo.Setup(r => r.AddAsync(It.IsAny<LoyaltyTransaction>()))
            .ReturnsAsync((LoyaltyTransaction t) => t);

        var result = await _sut.RedeemPointsAsync(customerId, 200, Guid.NewGuid());

        result.Points.Should().Be(-200);
        account.PointsBalance.Should().Be(300);
    }

    [Fact]
    public async Task RedeemPointsAsync_InsufficientPoints_Throws()
    {
        var customerId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PointsBalance = 50
        };

        _accountRepo.Setup(r => r.FindAsync(a => a.CustomerId == customerId))
            .ReturnsAsync(new[] { account });

        await FluentActions
            .Invoking(() => _sut.RedeemPointsAsync(customerId, 200, Guid.NewGuid()))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient points");
    }

    [Fact]
    public void ConvertPointsToCurrency_100Points_Returns1()
    {
        var result = _sut.ConvertPointsToCurrency(100);
        result.Should().Be(1);
    }

    [Fact]
    public void ConvertPointsToCurrency_0Points_Returns0()
    {
        var result = _sut.ConvertPointsToCurrency(0);
        result.Should().Be(0);
    }
}

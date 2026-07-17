using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class LoyaltyService
{
    private readonly IRepository<LoyaltyAccount> _accountRepo;
    private readonly IRepository<LoyaltyTransaction> _txnRepo;
    private readonly IRepository<TierConfig> _tierRepo;

    public LoyaltyService(
        IRepository<LoyaltyAccount> accountRepo,
        IRepository<LoyaltyTransaction> txnRepo,
        IRepository<TierConfig> tierRepo)
    {
        _accountRepo = accountRepo;
        _txnRepo = txnRepo;
        _tierRepo = tierRepo;
    }

    public async Task<LoyaltyAccount> GetOrCreateAccountAsync(Guid customerId)
    {
        var accounts = await _accountRepo.FindAsync(a => a.CustomerId == customerId);
        var account = accounts.FirstOrDefault();

        if (account == null)
        {
            account = new LoyaltyAccount
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                PointsBalance = 0,
                LifetimePoints = 0,
                Tier = Core.Enums.LoyaltyTier.Bronze,
                CreatedAt = DateTime.UtcNow
            };
            await _accountRepo.AddAsync(account);
        }

        return account;
    }

    public async Task<LoyaltyTransaction> EarnPointsAsync(Guid customerId, decimal qualifyingSpend, Guid transactionId)
    {
        var account = await GetOrCreateAccountAsync(customerId);
        var basePoints = (int)Math.Floor(qualifyingSpend);
        var multiplier = await GetTierMultiplierAsync(account.Tier);
        var earned = (int)Math.Floor(basePoints * multiplier);

        var txn = new LoyaltyTransaction
        {
            Id = Guid.NewGuid(),
            LoyaltyAccountId = account.Id,
            Type = Core.Enums.LoyaltyTransactionType.Earn,
            Points = earned,
            BalanceBefore = account.PointsBalance,
            BalanceAfter = account.PointsBalance + earned,
            ReferenceType = "TRANSACTION",
            ReferenceId = transactionId,
            IdempotencyKey = transactionId,
            CreatedAt = DateTime.UtcNow
        };
        await _txnRepo.AddAsync(txn);

        account.PointsBalance += earned;
        account.LifetimePoints += earned;
        account.LastEarnedAt = DateTime.UtcNow;
        account.Tier = await EvaluateTierAsync(account.LifetimePoints);
        await _accountRepo.UpdateAsync(account);

        return txn;
    }

    public async Task<LoyaltyTransaction> RedeemPointsAsync(Guid customerId, int points, Guid transactionId)
    {
        var accounts = await _accountRepo.FindAsync(a => a.CustomerId == customerId);
        var account = accounts.FirstOrDefault()
            ?? throw new InvalidOperationException("Loyalty account not found");

        if (account.PointsBalance < points)
            throw new InvalidOperationException("Insufficient points");

        var txn = new LoyaltyTransaction
        {
            Id = Guid.NewGuid(),
            LoyaltyAccountId = account.Id,
            Type = Core.Enums.LoyaltyTransactionType.Redeem,
            Points = -points,
            BalanceBefore = account.PointsBalance,
            BalanceAfter = account.PointsBalance - points,
            ReferenceType = "TRANSACTION",
            ReferenceId = transactionId,
            IdempotencyKey = transactionId,
            CreatedAt = DateTime.UtcNow
        };
        await _txnRepo.AddAsync(txn);

        account.PointsBalance -= points;
        account.LastRedeemedAt = DateTime.UtcNow;
        await _accountRepo.UpdateAsync(account);

        return txn;
    }

    private async Task<decimal> GetTierMultiplierAsync(Core.Enums.LoyaltyTier tier)
    {
        var configs = await _tierRepo.FindAsync(t => t.Tier == tier && t.IsActive);
        return configs.FirstOrDefault()?.PointsMultiplier ?? 1.0m;
    }

    private async Task<Core.Enums.LoyaltyTier> EvaluateTierAsync(int lifetimePoints)
    {
        var configs = await _tierRepo.GetAllAsync();
        var applicable = configs
            .Where(t => t.IsActive && lifetimePoints >= t.MinimumLifetimePoints)
            .OrderByDescending(t => t.MinimumLifetimePoints)
            .ToList();

        return applicable.FirstOrDefault()?.Tier ?? Core.Enums.LoyaltyTier.Bronze;
    }
}

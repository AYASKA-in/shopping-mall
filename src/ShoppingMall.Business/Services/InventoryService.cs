using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Business.Services;

public class InventoryService
{
    private readonly ICurrentStockRepository _stockRepo;
    private readonly IRepository<StockLedger> _ledgerRepo;
    private readonly IProductRepository _productRepo;

    public InventoryService(
        ICurrentStockRepository stockRepo,
        IRepository<StockLedger> ledgerRepo,
        IProductRepository productRepo)
    {
        _stockRepo = stockRepo;
        _ledgerRepo = ledgerRepo;
        _productRepo = productRepo;
    }

    public async Task DeductStockAsync(Guid storeId, Guid productId, decimal quantity, string referenceType, Guid referenceId, decimal unitCost = 0)
    {
        var stock = await _stockRepo.GetByStoreAndProductAsync(storeId, productId);
        if (stock == null)
            throw new InvalidOperationException("Stock record not found");

        if (stock.Available < quantity)
            throw new InvalidOperationException("Insufficient stock");

        var beforeOnHand = stock.OnHand;
        stock.OnHand -= quantity;
        stock.Reserved -= quantity;
        stock.Available = stock.OnHand - stock.Reserved;
        stock.LastUpdatedAt = DateTime.UtcNow;
        await _stockRepo.UpdateAsync(stock);

        var ledger = new StockLedger
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            ProductId = productId,
            MovementType = Core.Enums.MovementType.Sale,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Quantity = -quantity,
            QuantityBefore = beforeOnHand,
            QuantityAfter = stock.OnHand,
            UnitCost = unitCost,
            TotalCost = unitCost * quantity,
            CreatedAt = DateTime.UtcNow
        };
        await _ledgerRepo.AddAsync(ledger);
    }

    public async Task AddStockAsync(Guid storeId, Guid productId, decimal quantity, string referenceType, Guid referenceId, decimal unitCost = 0)
    {
        var stock = await _stockRepo.GetByStoreAndProductAsync(storeId, productId);

        var beforeOnHand = stock?.OnHand ?? 0;
        if (stock == null)
        {
            stock = new CurrentStock
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                ProductId = productId,
                OnHand = quantity,
                Reserved = 0,
                Available = quantity,
                LastUpdatedAt = DateTime.UtcNow
            };
            await _stockRepo.AddAsync(stock);
        }
        else
        {
            stock.OnHand += quantity;
            stock.Available = stock.OnHand - stock.Reserved;
            stock.LastUpdatedAt = DateTime.UtcNow;
            await _stockRepo.UpdateAsync(stock);
        }

        var ledger = new StockLedger
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            ProductId = productId,
            MovementType = Core.Enums.MovementType.GRN,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Quantity = quantity,
            QuantityBefore = beforeOnHand,
            QuantityAfter = stock.OnHand,
            UnitCost = unitCost,
            TotalCost = unitCost * quantity,
            CreatedAt = DateTime.UtcNow
        };
        await _ledgerRepo.AddAsync(ledger);
    }

    public async Task<CurrentStock?> GetStockAsync(Guid storeId, Guid productId)
        => await _stockRepo.GetByStoreAndProductAsync(storeId, productId);

    public async Task<IEnumerable<CurrentStock>> GetLowStockAsync(Guid storeId)
        => await _stockRepo.GetLowStockItemsAsync(storeId);

    public async Task<IEnumerable<StockLedger>> GetStockMovementAsync(Guid storeId, Guid productId, DateTime from, DateTime to)
        => await _ledgerRepo.FindAsync(l =>
            l.StoreId == storeId && l.ProductId == productId &&
            l.CreatedAt >= from && l.CreatedAt <= to);
}

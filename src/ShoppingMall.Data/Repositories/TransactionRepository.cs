using Microsoft.EntityFrameworkCore;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;
using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Data.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(ShoppingMallDbContext context) : base(context) { }

    public async Task<Transaction?> GetByReceiptNumberAsync(string receiptNumber)
        => await _dbSet.FirstOrDefaultAsync(t => t.ReceiptNumber == receiptNumber);

    public async Task<IEnumerable<Transaction>> GetByStoreAndDateAsync(Guid storeId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        return await _dbSet
            .Where(t => t.StoreId == storeId && t.CreatedAt >= startOfDay && t.CreatedAt < endOfDay)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateReceiptNumberAsync(Guid storeId)
    {
        var store = await _context.Stores.FindAsync(storeId);
        var storeCode = store?.Code ?? "XXX";
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var count = await _dbSet
            .CountAsync(t => t.CreatedAt >= todayStart && t.CreatedAt < todayEnd) + 1;

        return $"{storeCode}-{datePart}-{count:D4}";
    }
}

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(ShoppingMallDbContext context) : base(context) { }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
        => await _dbSet
            .Include(p => p.Barcodes)
            .FirstOrDefaultAsync(p => p.Barcodes.Any(b => b.Code == barcode));

    public async Task<Product?> GetBySKUAsync(string sku)
        => await _dbSet.FirstOrDefaultAsync(p => p.SKU == sku);

    public async Task<Product?> GetByPLUAsync(string pluCode)
        => await _dbSet.FirstOrDefaultAsync(p => p.PLUCode == pluCode);

    public async Task<IEnumerable<Product>> SearchAsync(string query)
    {
        var q = query.ToLower();
        return await _dbSet
            .Include(p => p.Barcodes)
            .Where(p => p.Name.ToLower().Contains(q)
                || p.SKU.ToLower().Contains(q)
                || p.Barcodes.Any(b => b.Code.Contains(q)))
            .Take(20)
            .ToListAsync();
    }
}

public class CurrentStockRepository : BaseRepository<CurrentStock>, ICurrentStockRepository
{
    public CurrentStockRepository(ShoppingMallDbContext context) : base(context) { }

    public async Task<CurrentStock?> GetByStoreAndProductAsync(Guid storeId, Guid productId)
        => await _dbSet.FirstOrDefaultAsync(s => s.StoreId == storeId && s.ProductId == productId);

    public async Task<IEnumerable<CurrentStock>> GetLowStockItemsAsync(Guid storeId)
        => await _dbSet
            .Where(s => s.StoreId == storeId && s.Available <= 10)
            .Include(s => s.Product)
            .ToListAsync();
}

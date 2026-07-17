using System.Linq.Expressions;

namespace ShoppingMall.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<Transaction?> GetByReceiptNumberAsync(string receiptNumber);
    Task<IEnumerable<Transaction>> GetByStoreAndDateAsync(Guid storeId, DateTime date);
    Task<string> GenerateReceiptNumberAsync(Guid storeId);
}

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<Product?> GetBySKUAsync(string sku);
    Task<IEnumerable<Product>> SearchAsync(string query);
}

public interface ICurrentStockRepository : IRepository<CurrentStock>
{
    Task<CurrentStock?> GetByStoreAndProductAsync(Guid storeId, Guid productId);
    Task<IEnumerable<CurrentStock>> GetLowStockItemsAsync(Guid storeId);
}

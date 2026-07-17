using System.IO;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace ShoppingMall.Client.Offline;

public class OfflineCache : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly string _dbPath;

    public OfflineCache()
    {
        _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ShoppingMall", "offline_cache.db");
        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath)!);

        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _connection.Open();
        Initialize();
    }

    private void Initialize()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS pending_transactions (
                id TEXT PRIMARY KEY,
                store_id TEXT NOT NULL,
                terminal_id TEXT NOT NULL,
                payload TEXT NOT NULL,
                created_at TEXT NOT NULL,
                synced INTEGER DEFAULT 0
            );
            CREATE TABLE IF NOT EXISTS cached_products (
                id TEXT PRIMARY KEY,
                barcode TEXT,
                sku TEXT,
                name TEXT NOT NULL,
                data TEXT NOT NULL,
                cached_at TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_pending_synced ON pending_transactions(synced);
            CREATE INDEX IF NOT EXISTS idx_cached_barcode ON cached_products(barcode);
        ";
        cmd.ExecuteNonQuery();
    }

    public async Task QueueTransactionAsync(string payload)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO pending_transactions (id, store_id, terminal_id, payload, created_at, synced)
            VALUES (@id, @storeId, @terminalId, @payload, @createdAt, 0)";
        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@storeId", "");
        cmd.Parameters.AddWithValue("@terminalId", "");
        cmd.Parameters.AddWithValue("@payload", payload);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<string>> GetPendingTransactionsAsync()
    {
        var result = new List<string>();
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT payload FROM pending_transactions WHERE synced = 0 ORDER BY created_at LIMIT 100";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(reader.GetString(0));
        return result;
    }

    public async Task MarkSyncedAsync(string payload)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "UPDATE pending_transactions SET synced = 1 WHERE payload = @payload";
        cmd.Parameters.AddWithValue("@payload", payload);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task CacheProductAsync(object product)
    {
        var json = JsonSerializer.Serialize(product);
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            INSERT OR REPLACE INTO cached_products (id, barcode, sku, name, data, cached_at)
            VALUES (@id, @barcode, @sku, @name, @data, @cachedAt)";
        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@cachedAt", DateTime.UtcNow.ToString("O"));
        await cmd.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}

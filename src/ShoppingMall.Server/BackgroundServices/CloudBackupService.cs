using System.Security.Cryptography;
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;
using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Business.Services;

public class CloudBackupService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CloudBackupService> _logger;

    public CloudBackupService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<CloudBackupService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CloudBackup> CreateBackupAsync(Guid storeId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShoppingMallDbContext>();
        var backupRepo = scope.ServiceProvider.GetRequiredService<IRepository<CloudBackup>>();

        var snapshot = await BuildSnapshotAsync(db, storeId);
        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = false });
        var plainBytes = System.Text.Encoding.UTF8.GetBytes(json);

        var (encrypted, key, iv) = Encrypt(plainBytes);
        var fileName = $"backup_{storeId:N}_{DateTime.UtcNow:yyyyMMddHHmmss}.enc";

        var connectionString = _configuration.GetValue<string>("CloudSync:StorageConnectionString");
        var containerName = _configuration.GetValue<string>("CloudSync:ContainerName") ?? "shopping-mall-backups";
        string? storageUrl = null;

        if (!string.IsNullOrEmpty(connectionString))
        {
            try
            {
                var blobClient = new BlobContainerClient(connectionString, containerName);
                await blobClient.CreateIfNotExistsAsync();

                var blob = blobClient.GetBlobClient(fileName);
                using var stream = new MemoryStream(encrypted);
                await blob.UploadAsync(stream);
                storageUrl = blob.Uri.ToString();
                _logger.LogInformation("Backup uploaded to {Url}", storageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Azure upload failed, saving locally");
            }
        }

        if (storageUrl == null)
        {
            var localDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            Directory.CreateDirectory(localDir);
            var localPath = Path.Combine(localDir, fileName);
            await File.WriteAllBytesAsync(localPath, encrypted);
            storageUrl = localPath;
        }

        var checksum = Convert.ToHexString(SHA256.HashData(plainBytes));
        var backup = new CloudBackup
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            FileName = fileName,
            FileSizeBytes = encrypted.Length,
            Checksum = checksum,
            StorageUrl = storageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await backupRepo.AddAsync(backup);
        return backup;
    }

    public async Task<IEnumerable<CloudBackup>> ListBackupsAsync(Guid storeId)
    {
        using var scope = _serviceProvider.CreateScope();
        var backupRepo = scope.ServiceProvider.GetRequiredService<IRepository<CloudBackup>>();
        return (await backupRepo.FindAsync(b => b.StoreId == storeId))
            .OrderByDescending(b => b.CreatedAt);
    }

    public (byte[] encrypted, byte[] key, byte[] iv) Encrypt(byte[] plaintext)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
        return (encrypted, aes.Key, aes.IV);
    }

    public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
    }

    private async Task<object> BuildSnapshotAsync(ShoppingMallDbContext db, Guid storeId)
    {
        var products = await db.Products.Where(p => p.IsActive).ToListAsync();
        var stock = await db.CurrentStocks.Where(s => s.StoreId == storeId).ToListAsync();
        var categories = await db.Categories.Where(c => c.IsActive).ToListAsync();
        var customers = await db.Customers.Where(c => c.IsActive).ToListAsync();
        var suppliers = await db.Suppliers.Where(s => s.IsActive).ToListAsync();
        var transactions = await db.Transactions
            .Where(t => t.StoreId == storeId && t.CreatedAt >= DateTime.UtcNow.AddDays(-90))
            .Include(t => t.Lines)
            .Include(t => t.Payments)
            .ToListAsync();
        var store = await db.Stores.FindAsync(storeId);

        return new
        {
            StoreName = store?.Name,
            StoreCode = store?.Code,
            GeneratedAt = DateTime.UtcNow,
            Products = products.Select(p => new { p.Id, p.SKU, p.Name, p.SellingPrice, p.TaxRate }),
            Stock = stock.Select(s => new { s.ProductId, s.OnHand, s.Available }),
            Categories = categories.Select(c => new { c.Id, c.Name }),
            Customers = customers.Select(c => new { c.Id, c.FirstName, c.LastName, c.Phone }),
            Suppliers = suppliers.Select(s => new { s.Id, s.Name, s.GSTIN }),
            RecentTransactions = transactions.Select(t => new
            {
                t.ReceiptNumber, t.GrandTotal, t.CreatedAt,
                Lines = t.Lines.Select(l => new { l.ProductName, l.Quantity, l.NetAmount }),
                Payments = t.Payments.Select(p => new { p.Method, p.Amount })
            })
        };
    }
}

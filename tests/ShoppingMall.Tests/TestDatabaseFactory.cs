using Microsoft.EntityFrameworkCore;
using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Tests;

public static class TestDatabaseFactory
{
    public static ShoppingMallDbContext CreateInMemoryDbContext()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ShoppingMallDbContext>()
            .UseSqlite(connection)
            .Options;

        var ctx = new ShoppingMallDbContext(options);
        ctx.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
        try
        {
            ctx.Database.EnsureCreated();
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            ctx.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""stores"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""OrganizationId"" TEXT,
                    ""Code"" TEXT NOT NULL,
                    ""Name"" TEXT NOT NULL,
                    ""GSTIN"" TEXT,
                    ""AddressLine1"" TEXT,
                    ""AddressLine2"" TEXT,
                    ""City"" TEXT,
                    ""State"" TEXT,
                    ""PostalCode"" TEXT,
                    ""Phone"" TEXT,
                    ""Email"" TEXT,
                    ""Status"" TEXT NOT NULL DEFAULT 'Active',
                    ""ReceiptFooter"" TEXT,
                    ""TimeZone"" TEXT,
                    ""IsActive"" INTEGER NOT NULL DEFAULT 1,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS ""terminals"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""StoreId"" TEXT NOT NULL,
                    ""Name"" TEXT NOT NULL,
                    ""DeviceId"" TEXT,
                    ""DeviceInfo"" TEXT,
                    ""Mode"" TEXT NOT NULL DEFAULT 'Client',
                    ""IsActive"" INTEGER NOT NULL DEFAULT 1,
                    ""LastHeartbeat"" TEXT,
                    ""CreatedAt"" TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS ""transactions"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""StoreId"" TEXT NOT NULL,
                    ""TerminalId"" TEXT NOT NULL,
                    ""UserId"" TEXT,
                    ""CustomerId"" TEXT,
                    ""ReceiptNumber"" TEXT NOT NULL,
                    ""Status"" TEXT NOT NULL DEFAULT 'Active',
                    ""SubTotal"" REAL NOT NULL DEFAULT 0,
                    ""DiscountTotal"" REAL NOT NULL DEFAULT 0,
                    ""TaxTotal"" REAL NOT NULL DEFAULT 0,
                    ""GrandTotal"" REAL NOT NULL DEFAULT 0,
                    ""RoundingAmount"" REAL NOT NULL DEFAULT 0,
                    ""Notes"" TEXT,
                    ""IdempotencyKey"" TEXT,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL,
                    ""RowVersion"" INTEGER NOT NULL DEFAULT 0
                );
                CREATE TABLE IF NOT EXISTS ""transaction_lines"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""TransactionId"" TEXT NOT NULL,
                    ""ProductId"" TEXT NOT NULL,
                    ""LineNumber"" INTEGER NOT NULL DEFAULT 0,
                    ""ProductName"" TEXT NOT NULL,
                    ""SKU"" TEXT,
                    ""Barcode"" TEXT,
                    ""Quantity"" REAL NOT NULL,
                    ""UOMId"" TEXT NOT NULL,
                    ""UnitPrice"" REAL NOT NULL,
                    ""Mrp"" REAL NOT NULL,
                    ""DiscountAmount"" REAL NOT NULL,
                    ""DiscountPercent"" REAL,
                    ""DiscountReason"" TEXT,
                    ""TaxRate"" REAL NOT NULL,
                    ""TaxAmount"" REAL NOT NULL,
                    ""NetAmount"" REAL NOT NULL,
                    ""IsWeighable"" INTEGER NOT NULL DEFAULT 0,
                    ""WeightKg"" REAL,
                    ""ParentLineId"" TEXT
                );
                CREATE TABLE IF NOT EXISTS ""payments"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""TransactionId"" TEXT NOT NULL,
                    ""Method"" TEXT NOT NULL,
                    ""Amount"" REAL NOT NULL,
                    ""TenderedAmount"" REAL,
                    ""ChangeAmount"" REAL,
                    ""ReferenceNumber"" TEXT,
                    ""GatewayResponse"" TEXT,
                    ""Status"" TEXT NOT NULL DEFAULT 'Pending',
                    ""IdempotencyKey"" TEXT,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""SettledAt"" TEXT
                );
                CREATE TABLE IF NOT EXISTS ""products"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""SKU"" TEXT NOT NULL,
                    ""Name"" TEXT NOT NULL,
                    ""CategoryId"" TEXT NOT NULL,
                    ""BrandId"" TEXT,
                    ""BaseUOMId"" TEXT NOT NULL,
                    ""HSNCode"" TEXT,
                    ""TaxRate"" REAL NOT NULL DEFAULT 0,
                    ""SellingPrice"" REAL,
                    ""Mrp"" REAL,
                    ""PurchasePrice"" REAL,
                    ""IsWeighable"" INTEGER NOT NULL DEFAULT 0,
                    ""IsActive"" INTEGER NOT NULL DEFAULT 1,
                    ""PLUCode"" TEXT,
                    ""CreatedAt"" TEXT NOT NULL,
                    ""UpdatedAt"" TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS ""barcodes"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""ProductId"" TEXT NOT NULL,
                    ""Code"" TEXT NOT NULL,
                    ""IsPrimary"" INTEGER NOT NULL DEFAULT 0
                );
                CREATE TABLE IF NOT EXISTS ""current_stocks"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""StoreId"" TEXT NOT NULL,
                    ""ProductId"" TEXT NOT NULL,
                    ""OnHand"" REAL NOT NULL DEFAULT 0,
                    ""Reserved"" REAL NOT NULL DEFAULT 0,
                    ""Available"" REAL NOT NULL DEFAULT 0,
                    ""UnitCost"" REAL,
                    ""UpdatedAt"" TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS ""stock_ledgers"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""StoreId"" TEXT NOT NULL,
                    ""ProductId"" TEXT NOT NULL,
                    ""TransactionId"" TEXT,
                    ""ReferenceNumber"" TEXT,
                    ""MovementType"" TEXT NOT NULL,
                    ""QuantityChange"" REAL NOT NULL,
                    ""UnitCost"" REAL,
                    ""BalanceBefore"" REAL NOT NULL,
                    ""BalanceAfter"" REAL NOT NULL,
                    ""CreatedAt"" TEXT NOT NULL
                );
            ");
        }
        ctx.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");
        return ctx;
    }
}

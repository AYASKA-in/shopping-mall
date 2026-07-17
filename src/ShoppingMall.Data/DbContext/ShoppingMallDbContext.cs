using Microsoft.EntityFrameworkCore;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.DbContext;

public class ShoppingMallDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ShoppingMallDbContext(DbContextOptions<ShoppingMallDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Terminal> Terminals => Set<Terminal>();
    public DbSet<StoreConfig> StoreConfigs => Set<StoreConfig>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Barcode> Barcodes => Set<Barcode>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<UOM> UOMs => Set<UOM>();
    public DbSet<UOMConversion> UOMConversions => Set<UOMConversion>();
    public DbSet<ProductUOM> ProductUOMs => Set<ProductUOM>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListLine> PriceListLines => Set<PriceListLine>();
    public DbSet<HSN> HSNs => Set<HSN>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<StoreProductOverride> StoreProductOverrides => Set<StoreProductOverride>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionLine> TransactionLines => Set<TransactionLine>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<TaxBreakdown> TaxBreakdowns => Set<TaxBreakdown>();
    public DbSet<SuspendedTransaction> SuspendedTransactions => Set<SuspendedTransaction>();
    public DbSet<VoidLog> VoidLogs => Set<VoidLog>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();
    public DbSet<CashDeclaration> CashDeclarations => Set<CashDeclaration>();
    public DbSet<StockLedger> StockLedgers => Set<StockLedger>();
    public DbSet<CurrentStock> CurrentStocks => Set<CurrentStock>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GRNLine> GRNLines => Set<GRNLine>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockAdjustmentLine> StockAdjustmentLines => Set<StockAdjustmentLine>();
    public DbSet<InterStoreTransfer> InterStoreTransfers => Set<InterStoreTransfer>();
    public DbSet<InterStoreTransferLine> InterStoreTransferLines => Set<InterStoreTransferLine>();
    public DbSet<ReorderRule> ReorderRules => Set<ReorderRule>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<POLine> POLines => Set<POLine>();
    public DbSet<VendorInvoice> VendorInvoices => Set<VendorInvoice>();
    public DbSet<DebitNote> DebitNotes => Set<DebitNote>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<PromotionRuleGroup> PromotionRuleGroups => Set<PromotionRuleGroup>();
    public DbSet<PromotionRule> PromotionRules => Set<PromotionRule>();
    public DbSet<PromotionBenefit> PromotionBenefits => Set<PromotionBenefit>();
    public DbSet<CouponCampaign> CouponCampaigns => Set<CouponCampaign>();
    public DbSet<CouponCode> CouponCodes => Set<CouponCode>();
    public DbSet<PromotionUsage> PromotionUsages => Set<PromotionUsage>();
    public DbSet<LoyaltyAccount> LoyaltyAccounts => Set<LoyaltyAccount>();
    public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();
    public DbSet<TierConfig> TierConfigs => Set<TierConfig>();
    public DbSet<SyncQueue> SyncQueues => Set<SyncQueue>();
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();
    public DbSet<CloudBackup> CloudBackups => Set<CloudBackup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingMallDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

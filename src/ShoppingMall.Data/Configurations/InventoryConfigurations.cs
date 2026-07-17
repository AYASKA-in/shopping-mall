using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class StockLedgerConfiguration : IEntityTypeConfiguration<StockLedger>
{
    public void Configure(EntityTypeBuilder<StockLedger> builder)
    {
        builder.ToTable("stock_ledger");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MovementType).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        builder.HasIndex(x => new { x.StoreId, x.ProductId, x.CreatedAt });
    }
}

public class CurrentStockConfiguration : IEntityTypeConfiguration<CurrentStock>
{
    public void Configure(EntityTypeBuilder<CurrentStock> builder)
    {
        builder.ToTable("current_stock");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        builder.HasIndex(x => new { x.StoreId, x.ProductId }).IsUnique();
    }
}

public class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("goods_receipts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GRNNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.GRNNumber).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.PurchaseOrder).WithMany(po => po.GoodsReceipts).HasForeignKey(x => x.POId).IsRequired(false);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId);
    }
}

public class GRNLineConfiguration : IEntityTypeConfiguration<GRNLine>
{
    public void Configure(EntityTypeBuilder<GRNLine> builder)
    {
        builder.ToTable("grn_lines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InspectionStatus).HasConversion<string>().HasMaxLength(15);
        builder.HasOne(x => x.GoodsReceipt).WithMany(g => g.Lines).HasForeignKey(x => x.GRNId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    }
}

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("stock_adjustments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AdjustmentNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
    }
}

public class StockAdjustmentLineConfiguration : IEntityTypeConfiguration<StockAdjustmentLine>
{
    public void Configure(EntityTypeBuilder<StockAdjustmentLine> builder)
    {
        builder.ToTable("stock_adjustment_lines");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Adjustment).WithMany(a => a.Lines).HasForeignKey(x => x.AdjustmentId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    }
}

public class InterStoreTransferConfiguration : IEntityTypeConfiguration<InterStoreTransfer>
{
    public void Configure(EntityTypeBuilder<InterStoreTransfer> builder)
    {
        builder.ToTable("inter_store_transfers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TransferNumber).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).IsRequired();
    }
}

public class InterStoreTransferLineConfiguration : IEntityTypeConfiguration<InterStoreTransferLine>
{
    public void Configure(EntityTypeBuilder<InterStoreTransferLine> builder)
    {
        builder.ToTable("inter_store_transfer_lines");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Transfer).WithMany(t => t.Lines).HasForeignKey(x => x.TransferId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    }
}

public class ReorderRuleConfiguration : IEntityTypeConfiguration<ReorderRule>
{
    public void Configure(EntityTypeBuilder<ReorderRule> builder)
    {
        builder.ToTable("reorder_rules");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.StoreId, x.ProductId }).IsUnique();
    }
}

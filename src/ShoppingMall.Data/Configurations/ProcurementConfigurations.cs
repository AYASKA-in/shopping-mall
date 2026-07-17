using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PONumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.PONumber).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        builder.HasOne(x => x.Supplier).WithMany(s => s.PurchaseOrders).HasForeignKey(x => x.SupplierId);
    }
}

public class POLineConfiguration : IEntityTypeConfiguration<POLine>
{
    public void Configure(EntityTypeBuilder<POLine> builder)
    {
        builder.ToTable("po_lines");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.PurchaseOrder).WithMany(po => po.Lines).HasForeignKey(x => x.POId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    }
}

public class VendorInvoiceConfiguration : IEntityTypeConfiguration<VendorInvoice>
{
    public void Configure(EntityTypeBuilder<VendorInvoice> builder)
    {
        builder.ToTable("vendor_invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvoiceNo).HasMaxLength(50).IsRequired();
        builder.Property(x => x.MatchStatus).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.PaymentStatus).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.POId);
        builder.HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId);
    }
}

public class DebitNoteConfiguration : IEntityTypeConfiguration<DebitNote>
{
    public void Configure(EntityTypeBuilder<DebitNote> builder)
    {
        builder.ToTable("debit_notes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DebitNoteNo).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(15);
    }
}

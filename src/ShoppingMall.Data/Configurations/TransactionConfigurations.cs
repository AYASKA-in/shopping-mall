using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReceiptNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.ReceiptNumber).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.RowVersion).IsConcurrencyToken(false);
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        builder.HasOne(x => x.Terminal).WithMany().HasForeignKey(x => x.TerminalId);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).IsRequired(false);
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).IsRequired(false);
        builder.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("\"IdempotencyKey\" IS NOT NULL");
        builder.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAt });
    }
}

public class TransactionLineConfiguration : IEntityTypeConfiguration<TransactionLine>
{
    public void Configure(EntityTypeBuilder<TransactionLine> builder)
    {
        builder.ToTable("transaction_lines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductName).HasMaxLength(255).IsRequired();
        builder.HasOne(x => x.Transaction).WithMany(t => t.Lines).HasForeignKey(x => x.TransactionId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ReferenceNumber).HasMaxLength(100);
        builder.HasOne(x => x.Transaction).WithMany(t => t.Payments).HasForeignKey(x => x.TransactionId);
        builder.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("\"IdempotencyKey\" IS NOT NULL");
        builder.HasIndex(x => new { x.TransactionId, x.Method, x.CreatedAt });
    }
}

public class TaxBreakdownConfiguration : IEntityTypeConfiguration<TaxBreakdown>
{
    public void Configure(EntityTypeBuilder<TaxBreakdown> builder)
    {
        builder.ToTable("tax_breakdowns");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TaxType).HasConversion<string>().HasMaxLength(10);
        builder.HasOne(x => x.Transaction).WithMany(t => t.TaxBreakdowns).HasForeignKey(x => x.TransactionId);
        builder.HasOne(x => x.TransactionLine).WithMany().HasForeignKey(x => x.TransactionLineId).IsRequired(false);
    }
}

public class SuspendedTransactionConfiguration : IEntityTypeConfiguration<SuspendedTransaction>
{
    public void Configure(EntityTypeBuilder<SuspendedTransaction> builder)
    {
        builder.ToTable("suspended_transactions");
        builder.HasKey(x => x.Id);
    }
}

public class VoidLogConfiguration : IEntityTypeConfiguration<VoidLog>
{
    public void Configure(EntityTypeBuilder<VoidLog> builder)
    {
        builder.ToTable("void_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.HasOne(x => x.Transaction).WithMany().HasForeignKey(x => x.TransactionId);
    }
}

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("refunds");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RefundMethod).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.OriginalTransaction).WithMany().HasForeignKey(x => x.OriginalTransactionId);
    }
}

public class CashMovementConfiguration : IEntityTypeConfiguration<CashMovement>
{
    public void Configure(EntityTypeBuilder<CashMovement> builder)
    {
        builder.ToTable("cash_movements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MovementType).HasMaxLength(30).IsRequired();
    }
}

public class CashDeclarationConfiguration : IEntityTypeConfiguration<CashDeclaration>
{
    public void Configure(EntityTypeBuilder<CashDeclaration> builder)
    {
        builder.ToTable("cash_declarations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DeclarationType).HasMaxLength(10).IsRequired();
    }
}

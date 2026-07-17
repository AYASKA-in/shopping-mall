using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class SyncQueueConfiguration : IEntityTypeConfiguration<SyncQueue>
{
    public void Configure(EntityTypeBuilder<SyncQueue> builder)
    {
        builder.ToTable("sync_queue");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Operation).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
        builder.HasIndex(x => new { x.StoreId, x.Status, x.CreatedAt });
    }
}

public class SyncLogConfiguration : IEntityTypeConfiguration<SyncLog>
{
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.ToTable("sync_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Direction).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10);
    }
}

public class CloudBackupConfiguration : IEntityTypeConfiguration<CloudBackup>
{
    public void Configure(EntityTypeBuilder<CloudBackup> builder)
    {
        builder.ToTable("cloud_backups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
    }
}

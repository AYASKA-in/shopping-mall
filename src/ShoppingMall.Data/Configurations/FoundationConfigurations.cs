using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.GSTIN).HasMaxLength(15);
        builder.Property(x => x.PAN).HasMaxLength(10);
        builder.Property(x => x.AddressLine1).HasMaxLength(255);
        builder.Property(x => x.AddressLine2).HasMaxLength(255);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.State).HasMaxLength(100);
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(200);
    }
}

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("stores");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.GSTIN).HasMaxLength(15);
        builder.Property(x => x.AddressLine1).HasMaxLength(255);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.State).HasMaxLength(100);
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.Organization).WithMany().HasForeignKey(x => x.OrganizationId);
    }
}

public class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
{
    public void Configure(EntityTypeBuilder<Terminal> builder)
    {
        builder.ToTable("terminals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DeviceId).HasMaxLength(100);
        builder.Property(x => x.Mode).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.HasOne(x => x.Store).WithMany(s => s.Terminals).HasForeignKey(x => x.StoreId);
    }
}

public class StoreConfigConfiguration : IEntityTypeConfiguration<StoreConfig>
{
    public void Configure(EntityTypeBuilder<StoreConfig> builder)
    {
        builder.ToTable("store_configs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConfigKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ConfigValue).HasMaxLength(500).IsRequired();
        builder.HasOne(x => x.Store).WithMany(s => s.Configs).HasForeignKey(x => x.StoreId);
        builder.HasIndex(x => new { x.StoreId, x.ConfigKey }).IsUnique();
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Username).IsUnique();
        builder.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PinHash).HasMaxLength(255).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).IsRequired(false);
    }
}

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User).WithMany(u => u.Sessions).HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.Terminal).WithMany().HasForeignKey(x => x.TerminalId);
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.PermissionName).HasMaxLength(100).IsRequired();
    }
}

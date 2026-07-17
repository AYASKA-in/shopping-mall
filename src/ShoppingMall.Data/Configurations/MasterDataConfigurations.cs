using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SKU).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.SKU).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(255).IsRequired();
        builder.Property(x => x.HSNCode).HasMaxLength(10);
        builder.HasOne(x => x.Category).WithMany(c => c.Products).HasForeignKey(x => x.CategoryId);
        builder.HasOne(x => x.Brand).WithMany(b => b.Products).HasForeignKey(x => x.BrandId).IsRequired(false);
        builder.HasOne(x => x.BaseUOM).WithMany(u => u.Products).HasForeignKey(x => x.BaseUOMId);
        builder.HasOne(x => x.HSN).WithMany().HasForeignKey(x => x.HSNId).IsRequired(false);
    }
}

public class BarcodeConfiguration : IEntityTypeConfiguration<Barcode>
{
    public void Configure(EntityTypeBuilder<Barcode> builder)
    {
        builder.ToTable("barcodes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Type).HasMaxLength(20);
        builder.HasOne(x => x.Product).WithMany(p => p.Barcodes).HasForeignKey(x => x.ProductId);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasOne(x => x.ParentCategory).WithMany(c => c.SubCategories).HasForeignKey(x => x.ParentCategoryId).IsRequired(false);
    }
}

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}

public class UOMConfiguration : IEntityTypeConfiguration<UOM>
{
    public void Configure(EntityTypeBuilder<UOM> builder)
    {
        builder.ToTable("uoms");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Abbreviation).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(20);
    }
}

public class UOMConversionConfiguration : IEntityTypeConfiguration<UOMConversion>
{
    public void Configure(EntityTypeBuilder<UOMConversion> builder)
    {
        builder.ToTable("uom_conversions");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.FromUOM).WithMany().HasForeignKey(x => x.FromUOMId);
        builder.HasOne(x => x.ToUOM).WithMany().HasForeignKey(x => x.ToUOMId);
    }
}

public class ProductUOMConfiguration : IEntityTypeConfiguration<ProductUOM>
{
    public void Configure(EntityTypeBuilder<ProductUOM> builder)
    {
        builder.ToTable("product_uoms");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Product).WithMany(p => p.ProductUOMs).HasForeignKey(x => x.ProductId);
        builder.HasOne(x => x.UOM).WithMany(u => u.ProductUOMs).HasForeignKey(x => x.UOMId);
    }
}

public class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.ToTable("price_lists");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}

public class PriceListLineConfiguration : IEntityTypeConfiguration<PriceListLine>
{
    public void Configure(EntityTypeBuilder<PriceListLine> builder)
    {
        builder.ToTable("price_list_lines");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.PriceList).WithMany(p => p.Lines).HasForeignKey(x => x.PriceListId);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
    }
}

public class HSNConfiguration : IEntityTypeConfiguration<HSN>
{
    public void Configure(EntityTypeBuilder<HSN> builder)
    {
        builder.ToTable("hsn_codes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(10).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.GSTIN).HasMaxLength(15);
        builder.Property(x => x.PAN).HasMaxLength(10);
        builder.Property(x => x.Tier).HasConversion<string>().HasMaxLength(15);
    }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Phone).HasMaxLength(20);
        builder.HasIndex(x => x.Phone).IsUnique().HasFilter("\"Phone\" IS NOT NULL");
        builder.Property(x => x.Email).HasMaxLength(200);
    }
}

public class StoreProductOverrideConfiguration : IEntityTypeConfiguration<StoreProductOverride>
{
    public void Configure(EntityTypeBuilder<StoreProductOverride> builder)
    {
        builder.ToTable("store_product_overrides");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId);
        builder.HasOne(x => x.Product).WithMany(p => p.StoreOverrides).HasForeignKey(x => x.ProductId);
        builder.HasIndex(x => new { x.StoreId, x.ProductId }).IsUnique();
    }
}

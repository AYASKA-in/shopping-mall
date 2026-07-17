namespace ShoppingMall.Core.Models;

public class Product
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid BaseUOMId { get; set; }
    public Guid? HSNId { get; set; }
    public string? HSNCode { get; set; }
    public decimal TaxRate { get; set; }
    public decimal? Mrp { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? PurchasePrice { get; set; }
    public bool IsWeighable { get; set; }
    public string? PLUCode { get; set; }
    public bool IsAgeRestricted { get; set; }
    public int? MinAge { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    public UOM BaseUOM { get; set; } = null!;
    public HSN? HSN { get; set; }
    public ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
    public ICollection<ProductUOM> ProductUOMs { get; set; } = new List<ProductUOM>();
    public ICollection<StoreProductOverride> StoreOverrides { get; set; } = new List<StoreProductOverride>();
}

public class Barcode
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = "EAN-13";
    public bool IsDefault { get; set; }

    public Product Product { get; set; } = null!;
}

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Brand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class UOM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string Category { get; set; } = "Count";

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ProductUOM> ProductUOMs { get; set; } = new List<ProductUOM>();
}

public class UOMConversion
{
    public Guid Id { get; set; }
    public Guid FromUOMId { get; set; }
    public Guid ToUOMId { get; set; }
    public decimal ConversionFactor { get; set; }

    public UOM FromUOM { get; set; } = null!;
    public UOM ToUOM { get; set; } = null!;
}

public class ProductUOM
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid UOMId { get; set; }
    public bool IsBaseUOM { get; set; }
    public bool IsPurchaseUOM { get; set; }
    public bool IsSalesUOM { get; set; }
    public decimal ConversionFactor { get; set; } = 1;

    public Product Product { get; set; } = null!;
    public UOM UOM { get; set; } = null!;
}

public class PriceList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PriceListLine> Lines { get; set; } = new List<PriceListLine>();
}

public class PriceListLine
{
    public Guid Id { get; set; }
    public Guid PriceListId { get; set; }
    public Guid ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPercent { get; set; }

    public PriceList PriceList { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class HSN
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal CGSTRate { get; set; }
    public decimal SGSTRate { get; set; }
    public decimal IGSTRate { get; set; }
    public decimal CessRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Supplier
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? GSTIN { get; set; }
    public string? PAN { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? CreditDays { get; set; }
    public VendorTier Tier { get; set; } = VendorTier.Silver;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}

public class Customer
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public LoyaltyAccount? LoyaltyAccount { get; set; }
}

public class StoreProductOverride
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid ProductId { get; set; }
    public decimal? OverridePrice { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Store Store { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

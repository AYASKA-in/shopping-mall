using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ShoppingMall.Core.Models;
using ShoppingMall.Core.Enums;
using ShoppingMall.Data.DbContext;

namespace ShoppingMall.Data.Seed;

public static class DbInitializer
{
    private static string HashPin(string pin)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
        return Convert.ToHexString(bytes).ToLower();
    }

    public static async Task InitializeAsync(ShoppingMallDbContext db)
    {
        if (await db.Organizations.AnyAsync()) return;

        var orgId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();

        var org = new Organization
        {
            Id = orgId,
            Name = "Shopping Mart India",
            LegalName = "Shopping Mart India Private Limited",
            GSTIN = "27AABCU1234D1Z1",
            PAN = "AABCU1234D",
            AddressLine1 = "123, Business District",
            City = "Mumbai",
            State = "Maharashtra",
            PostalCode = "400001",
            Country = "India",
            Phone = "022-12345678",
            Email = "info@shoppingmart.in",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Organizations.Add(org);

        var store = new Store
        {
            Id = storeId,
            OrganizationId = orgId,
            Code = "MUM-001",
            Name = "Mumbai Flagship",
            GSTIN = "27AABCU1234D1Z1",
            AddressLine1 = "123, Business District",
            City = "Mumbai",
            State = "Maharashtra",
            PostalCode = "400001",
            Phone = "022-12345678",
            Email = "store.mumbai@shoppingmart.in",
            Status = StoreStatus.Active,
            ReceiptFooter = "Thank you! Visit again!",
            TimeZone = "Asia/Kolkata",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Stores.Add(store);

        var terminal = new Terminal
        {
            Id = terminalId,
            StoreId = storeId,
            Name = "POS-1",
            DeviceId = terminalId.ToString(),
            Mode = TerminalMode.Client,
            IsActive = true,
            LastHeartbeat = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        db.Terminals.Add(terminal);

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            Username = "admin",
            DisplayName = "Store Admin",
            PinHash = HashPin("1234"),
            Email = "admin@shoppingmart.in",
            Phone = "9876543210",
            Role = UserRole.StoreManager,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Users.Add(adminUser);

        var cashierUser = new User
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            Username = "cashier1",
            DisplayName = "Rohit Sharma",
            PinHash = HashPin("0000"),
            Role = UserRole.Cashier,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Users.Add(cashierUser);

        var hsn0401 = Guid.NewGuid();
        var hsn0406 = Guid.NewGuid();
        var hsn1507 = Guid.NewGuid();
        var hsn1701 = Guid.NewGuid();
        var hsn1905 = Guid.NewGuid();
        var hsn2201 = Guid.NewGuid();
        var hsn3304 = Guid.NewGuid();
        var hsn3926 = Guid.NewGuid();
        var hsn6109 = Guid.NewGuid();
        var hsn8471 = Guid.NewGuid();
        var hsn8523 = Guid.NewGuid();
        var hsn9506 = Guid.NewGuid();

        var hsns = new List<HSN>
        {
            new() { Id = hsn0401, Code = "0401", Description = "Milk & Cream", CGSTRate = 2.5m, SGSTRate = 2.5m, IGSTRate = 5m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn0406, Code = "0406", Description = "Cheese & Paneer", CGSTRate = 2.5m, SGSTRate = 2.5m, IGSTRate = 5m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn1507, Code = "1507", Description = "Edible Oils", CGSTRate = 2.5m, SGSTRate = 2.5m, IGSTRate = 5m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn1701, Code = "1701", Description = "Sugar", CGSTRate = 2.5m, SGSTRate = 2.5m, IGSTRate = 5m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn1905, Code = "1905", Description = "Bread, Pastries, Biscuits", CGSTRate = 9m, SGSTRate = 9m, IGSTRate = 18m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn2201, Code = "2201", Description = "Packaged Drinking Water", CGSTRate = 6m, SGSTRate = 6m, IGSTRate = 12m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn3304, Code = "3304", Description = "Cosmetics & Toiletries", CGSTRate = 9m, SGSTRate = 9m, IGSTRate = 18m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn3926, Code = "3926", Description = "Plastic Household Items", CGSTRate = 9m, SGSTRate = 9m, IGSTRate = 18m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn6109, Code = "6109", Description = "T-shirts & Garments", CGSTRate = 6m, SGSTRate = 6m, IGSTRate = 12m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn8471, Code = "8471", Description = "Computers & Accessories", CGSTRate = 9m, SGSTRate = 9m, IGSTRate = 18m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn8523, Code = "8523", Description = "Media & Electronics", CGSTRate = 9m, SGSTRate = 9m, IGSTRate = 18m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true },
            new() { Id = hsn9506, Code = "9506", Description = "Sports & Toys", CGSTRate = 6m, SGSTRate = 6m, IGSTRate = 12m, CessRate = 0, EffectiveFrom = new DateOnly(2017, 7, 1), IsActive = true }
        };
        db.HSNs.AddRange(hsns);

        var kgId = Guid.NewGuid();
        var gId = Guid.NewGuid();
        var lId = Guid.NewGuid();
        var mlId = Guid.NewGuid();
        var pcId = Guid.NewGuid();
        var boxId = Guid.NewGuid();
        var dzId = Guid.NewGuid();
        var pkId = Guid.NewGuid();

        var uoms = new List<UOM>
        {
            new() { Id = kgId, Name = "Kilogram", Abbreviation = "kg", Category = "Weight" },
            new() { Id = gId, Name = "Gram", Abbreviation = "g", Category = "Weight" },
            new() { Id = lId, Name = "Litre", Abbreviation = "L", Category = "Volume" },
            new() { Id = mlId, Name = "Millilitre", Abbreviation = "mL", Category = "Volume" },
            new() { Id = pcId, Name = "Piece", Abbreviation = "pc", Category = "Count" },
            new() { Id = boxId, Name = "Box", Abbreviation = "box", Category = "Count" },
            new() { Id = dzId, Name = "Dozen", Abbreviation = "dz", Category = "Count" },
            new() { Id = pkId, Name = "Pack", Abbreviation = "pk", Category = "Count" }
        };
        db.UOMs.AddRange(uoms);

        var dairyCat = Guid.NewGuid();
        var bevCat = Guid.NewGuid();
        var snackCat = Guid.NewGuid();
        var groceryCat = Guid.NewGuid();
        var personalCat = Guid.NewGuid();
        var householdCat = Guid.NewGuid();
        var electronicsCat = Guid.NewGuid();

        var categories = new List<Category>
        {
            new() { Id = dairyCat, Name = "Dairy & Eggs", Description = "Milk, cheese, paneer, eggs", SortOrder = 1, IsActive = true },
            new() { Id = bevCat, Name = "Beverages", Description = "Soft drinks, juices, water", SortOrder = 2, IsActive = true },
            new() { Id = snackCat, Name = "Snacks & Biscuits", Description = "Chips, cookies, namkeen", SortOrder = 3, IsActive = true },
            new() { Id = groceryCat, Name = "Grocery & Staples", Description = "Rice, dal, atta, oil, sugar", SortOrder = 4, IsActive = true },
            new() { Id = personalCat, Name = "Personal Care", Description = "Soap, shampoo, cosmetics", SortOrder = 5, IsActive = true },
            new() { Id = householdCat, Name = "Household", Description = "Cleaning, kitchen items", SortOrder = 6, IsActive = true },
            new() { Id = electronicsCat, Name = "Electronics", Description = "Accessories, media", SortOrder = 7, IsActive = true }
        };
        db.Categories.AddRange(categories);

        await db.SaveChangesAsync();
    }
}

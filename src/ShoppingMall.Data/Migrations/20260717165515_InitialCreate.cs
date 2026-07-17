using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingMall.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cash_declarations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeclarationType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Count2000 = table.Column<int>(type: "integer", nullable: false),
                    Count500 = table.Column<int>(type: "integer", nullable: false),
                    Count200 = table.Column<int>(type: "integer", nullable: false),
                    Count100 = table.Column<int>(type: "integer", nullable: false),
                    Count50 = table.Column<int>(type: "integer", nullable: false),
                    Count20 = table.Column<int>(type: "integer", nullable: false),
                    Count10 = table.Column<int>(type: "integer", nullable: false),
                    CountCoins = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_declarations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cash_movements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovementType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_movements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_categories_categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cloud_backups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "text", nullable: true),
                    StorageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cloud_backups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "coupon_campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaxUses = table.Column<int>(type: "integer", nullable: false),
                    MaxUsesPerCustomer = table.Column<int>(type: "integer", nullable: true),
                    MinOrderAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "debit_notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DebitNoteNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    GRNId = table.Column<Guid>(type: "uuid", nullable: false),
                    POId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturnType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAdjustment = table.Column<decimal>(type: "numeric", nullable: false),
                    NetAdjustment = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debit_notes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "hsn_codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CGSTRate = table.Column<decimal>(type: "numeric", nullable: false),
                    SGSTRate = table.Column<decimal>(type: "numeric", nullable: false),
                    IGSTRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CessRate = table.Column<decimal>(type: "numeric", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hsn_codes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "inter_store_transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    FromStoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToStoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShippedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inter_store_transfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GSTIN = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    PAN = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLine2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "price_lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_lists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "promotion_usages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CouponCodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_usages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Stackability = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Budget = table.Column<decimal>(type: "numeric", nullable: true),
                    DailyBudget = table.Column<decimal>(type: "numeric", nullable: true),
                    BudgetSpent = table.Column<decimal>(type: "numeric", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "reorder_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "numeric", nullable: false),
                    ReorderQty = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxStock = table.Column<decimal>(type: "numeric", nullable: false),
                    SafetyStock = table.Column<decimal>(type: "numeric", nullable: false),
                    PreferredSupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reorder_rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PermissionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Resource = table.Column<string>(type: "text", nullable: true),
                    CanRead = table.Column<bool>(type: "boolean", nullable: false),
                    CanWrite = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stock_adjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdjustmentNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPosted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_adjustments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LegalName = table.Column<string>(type: "text", nullable: true),
                    GSTIN = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    PAN = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric", nullable: true),
                    CreditDays = table.Column<int>(type: "integer", nullable: true),
                    Tier = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suspended_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    BasketData = table.Column<string>(type: "text", nullable: false),
                    BasketTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    ItemCount = table.Column<int>(type: "integer", nullable: false),
                    SuspendedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecalledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRecalled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suspended_transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sync_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Direction = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ItemsProcessed = table.Column<int>(type: "integer", nullable: false),
                    ItemsFailed = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sync_queue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Operation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_queue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tier_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MinimumLifetimePoints = table.Column<int>(type: "integer", nullable: false),
                    PointsMultiplier = table.Column<decimal>(type: "numeric", nullable: false),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tier_configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "uoms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uoms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "coupon_codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UseCount = table.Column<int>(type: "integer", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupon_codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_coupon_codes_coupon_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "coupon_campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardNumber = table.Column<string>(type: "text", nullable: true),
                    PointsBalance = table.Column<int>(type: "integer", nullable: false),
                    LifetimePoints = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LastEarnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastRedeemedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loyalty_accounts_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GSTIN = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReceiptFooter = table.Column<string>(type: "text", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stores_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotion_benefits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BenefitType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxDiscount = table.Column<decimal>(type: "numeric", nullable: true),
                    BuyQty = table.Column<int>(type: "integer", nullable: true),
                    GetQty = table.Column<int>(type: "integer", nullable: true),
                    FreeProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicableOn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_benefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_promotion_benefits_promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotion_rule_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PromotionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Combinator = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_rule_groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_promotion_rule_groups_promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    BaseUOMId = table.Column<Guid>(type: "uuid", nullable: false),
                    HSNId = table.Column<Guid>(type: "uuid", nullable: true),
                    HSNCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Mrp = table.Column<decimal>(type: "numeric", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    IsWeighable = table.Column<bool>(type: "boolean", nullable: false),
                    PLUCode = table.Column<string>(type: "text", nullable: true),
                    IsAgeRestricted = table.Column<bool>(type: "boolean", nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_products_brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_products_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_hsn_codes_HSNId",
                        column: x => x.HSNId,
                        principalTable: "hsn_codes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_products_uoms_BaseUOMId",
                        column: x => x.BaseUOMId,
                        principalTable: "uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "uom_conversions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUOMId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToUOMId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uom_conversions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_uom_conversions_uoms_FromUOMId",
                        column: x => x.FromUOMId,
                        principalTable: "uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_uom_conversions_uoms_ToUOMId",
                        column: x => x.ToUOMId,
                        principalTable: "uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoyaltyAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    BalanceBefore = table.Column<int>(type: "integer", nullable: false),
                    BalanceAfter = table.Column<int>(type: "integer", nullable: false),
                    ReferenceType = table.Column<string>(type: "text", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId",
                        column: x => x.LoyaltyAccountId,
                        principalTable: "loyalty_accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchase_orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PONumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PromisedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_purchase_orders_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_orders_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_configs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConfigValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_configs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_configs_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "terminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Mode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_terminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_terminals_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: true),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PinHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "promotion_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operator = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_promotion_rules_promotion_rule_groups_RuleGroupId",
                        column: x => x.RuleGroupId,
                        principalTable: "promotion_rule_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "barcodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barcodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_barcodes_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "current_stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnHand = table.Column<decimal>(type: "numeric", nullable: false),
                    Reserved = table.Column<decimal>(type: "numeric", nullable: false),
                    Available = table.Column<decimal>(type: "numeric", nullable: false),
                    OnOrder = table.Column<decimal>(type: "numeric", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_current_stock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_current_stock_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_current_stock_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inter_store_transfer_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    ShippedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceivedQty = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inter_store_transfer_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_inter_store_transfer_lines_inter_store_transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "inter_store_transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inter_store_transfer_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "price_list_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PriceListId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_list_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_price_list_lines_price_lists_PriceListId",
                        column: x => x.PriceListId,
                        principalTable: "price_lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_price_list_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_uoms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UOMId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsBaseUOM = table.Column<bool>(type: "boolean", nullable: false),
                    IsPurchaseUOM = table.Column<bool>(type: "boolean", nullable: false),
                    IsSalesUOM = table.Column<bool>(type: "boolean", nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_uoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_uoms_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_uoms_uoms_UOMId",
                        column: x => x.UOMId,
                        principalTable: "uoms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_adjustment_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdjustmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_adjustment_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_adjustment_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_adjustment_lines_stock_adjustments_AdjustmentId",
                        column: x => x.AdjustmentId,
                        principalTable: "stock_adjustments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_ledger",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovementType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReferenceType = table.Column<string>(type: "text", nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantityBefore = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantityAfter = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    LotNumber = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_ledger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_ledger_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_ledger_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_product_overrides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OverridePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_product_overrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_product_overrides_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_product_overrides_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "goods_receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GRNNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    POId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeliveryChallanNo = table.Column<string>(type: "text", nullable: true),
                    VehicleNo = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods_receipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_goods_receipts_purchase_orders_POId",
                        column: x => x.POId,
                        principalTable: "purchase_orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_goods_receipts_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "po_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    POId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNo = table.Column<int>(type: "integer", nullable: false),
                    OrderedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceivedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    AcceptedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    RejectedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RequiredDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsBackorder = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_po_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_po_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_po_lines_purchase_orders_POId",
                        column: x => x.POId,
                        principalTable: "purchase_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vendor_invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    POId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MatchStatus = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vendor_invoices_purchase_orders_POId",
                        column: x => x.POId,
                        principalTable: "purchase_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vendor_invoices_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LogoutAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sessions_terminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "terminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sessions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceiptNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    RoundingAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transactions_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_transactions_stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_terminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "terminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "grn_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GRNId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    AcceptedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    RejectedQty = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    BatchNo = table.Column<string>(type: "text", nullable: true),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MfgDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InspectionStatus = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grn_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_grn_lines_goods_receipts_GRNId",
                        column: x => x.GRNId,
                        principalTable: "goods_receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grn_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TenderedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    ChangeAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GatewayResponse = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IdempotencyKey = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payments_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefundTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    RefundMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refunds_transactions_OriginalTransactionId",
                        column: x => x.OriginalTransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transaction_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: true),
                    Barcode = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UOMId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Mrp = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    DiscountReason = table.Column<string>(type: "text", nullable: true),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    NetAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsWeighable = table.Column<bool>(type: "boolean", nullable: false),
                    WeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    ParentLineId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transaction_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_lines_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "void_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    VoidedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReasonCode = table.Column<string>(type: "text", nullable: true),
                    VoidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_void_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_void_logs_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tax_breakdowns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaxType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_breakdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tax_breakdowns_transaction_lines_TransactionLineId",
                        column: x => x.TransactionLineId,
                        principalTable: "transaction_lines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tax_breakdowns_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_barcodes_Code",
                table: "barcodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_barcodes_ProductId",
                table: "barcodes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentCategoryId",
                table: "categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_coupon_codes_CampaignId",
                table: "coupon_codes",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_coupon_codes_Code",
                table: "coupon_codes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_current_stock_ProductId",
                table: "current_stock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_current_stock_StoreId_ProductId",
                table: "current_stock",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_Phone",
                table: "customers",
                column: "Phone",
                unique: true,
                filter: "\"Phone\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_GRNNumber",
                table: "goods_receipts",
                column: "GRNNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_POId",
                table: "goods_receipts",
                column: "POId");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_SupplierId",
                table: "goods_receipts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_grn_lines_GRNId",
                table: "grn_lines",
                column: "GRNId");

            migrationBuilder.CreateIndex(
                name: "IX_grn_lines_ProductId",
                table: "grn_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_hsn_codes_Code",
                table: "hsn_codes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inter_store_transfer_lines_ProductId",
                table: "inter_store_transfer_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_inter_store_transfer_lines_TransferId",
                table: "inter_store_transfer_lines",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_accounts_CustomerId",
                table: "loyalty_accounts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_transactions_IdempotencyKey",
                table: "loyalty_transactions",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_transactions_LoyaltyAccountId",
                table: "loyalty_transactions",
                column: "LoyaltyAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_IdempotencyKey",
                table: "payments",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_payments_TransactionId",
                table: "payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_po_lines_POId",
                table: "po_lines",
                column: "POId");

            migrationBuilder.CreateIndex(
                name: "IX_po_lines_ProductId",
                table: "po_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_price_list_lines_PriceListId",
                table: "price_list_lines",
                column: "PriceListId");

            migrationBuilder.CreateIndex(
                name: "IX_price_list_lines_ProductId",
                table: "price_list_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_uoms_ProductId",
                table: "product_uoms",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_uoms_UOMId",
                table: "product_uoms",
                column: "UOMId");

            migrationBuilder.CreateIndex(
                name: "IX_products_BaseUOMId",
                table: "products",
                column: "BaseUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_products_BrandId",
                table: "products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_products_CategoryId",
                table: "products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_products_HSNId",
                table: "products",
                column: "HSNId");

            migrationBuilder.CreateIndex(
                name: "IX_products_SKU",
                table: "products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_promotion_benefits_PromotionId",
                table: "promotion_benefits",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_rule_groups_PromotionId",
                table: "promotion_rule_groups",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_rules_RuleGroupId",
                table: "promotion_rules",
                column: "RuleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_PONumber",
                table: "purchase_orders",
                column: "PONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_StoreId",
                table: "purchase_orders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_SupplierId",
                table: "purchase_orders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_OriginalTransactionId",
                table: "refunds",
                column: "OriginalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_reorder_rules_StoreId_ProductId",
                table: "reorder_rules",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessions_TerminalId",
                table: "sessions",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_UserId",
                table: "sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustment_lines_AdjustmentId",
                table: "stock_adjustment_lines",
                column: "AdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustment_lines_ProductId",
                table: "stock_adjustment_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_ledger_ProductId",
                table: "stock_ledger",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_ledger_StoreId_ProductId_CreatedAt",
                table: "stock_ledger",
                columns: new[] { "StoreId", "ProductId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_store_configs_StoreId_ConfigKey",
                table: "store_configs",
                columns: new[] { "StoreId", "ConfigKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_store_product_overrides_ProductId",
                table: "store_product_overrides",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_store_product_overrides_StoreId_ProductId",
                table: "store_product_overrides",
                columns: new[] { "StoreId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stores_Code",
                table: "stores",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stores_OrganizationId",
                table: "stores",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_Code",
                table: "suppliers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sync_queue_StoreId_Status_CreatedAt",
                table: "sync_queue",
                columns: new[] { "StoreId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tax_breakdowns_TransactionId",
                table: "tax_breakdowns",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_tax_breakdowns_TransactionLineId",
                table: "tax_breakdowns",
                column: "TransactionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_terminals_StoreId",
                table: "terminals",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_lines_ProductId",
                table: "transaction_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_lines_TransactionId",
                table: "transaction_lines",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CustomerId",
                table: "transactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_IdempotencyKey",
                table: "transactions",
                column: "IdempotencyKey",
                unique: true,
                filter: "\"IdempotencyKey\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_ReceiptNumber",
                table: "transactions",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_StoreId",
                table: "transactions",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_TerminalId",
                table: "transactions",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_UserId",
                table: "transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_uom_conversions_FromUOMId",
                table: "uom_conversions",
                column: "FromUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_uom_conversions_ToUOMId",
                table: "uom_conversions",
                column: "ToUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_users_StoreId",
                table: "users",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vendor_invoices_POId",
                table: "vendor_invoices",
                column: "POId");

            migrationBuilder.CreateIndex(
                name: "IX_vendor_invoices_SupplierId",
                table: "vendor_invoices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_void_logs_TransactionId",
                table: "void_logs",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "barcodes");

            migrationBuilder.DropTable(
                name: "cash_declarations");

            migrationBuilder.DropTable(
                name: "cash_movements");

            migrationBuilder.DropTable(
                name: "cloud_backups");

            migrationBuilder.DropTable(
                name: "coupon_codes");

            migrationBuilder.DropTable(
                name: "current_stock");

            migrationBuilder.DropTable(
                name: "debit_notes");

            migrationBuilder.DropTable(
                name: "grn_lines");

            migrationBuilder.DropTable(
                name: "inter_store_transfer_lines");

            migrationBuilder.DropTable(
                name: "loyalty_transactions");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "po_lines");

            migrationBuilder.DropTable(
                name: "price_list_lines");

            migrationBuilder.DropTable(
                name: "product_uoms");

            migrationBuilder.DropTable(
                name: "promotion_benefits");

            migrationBuilder.DropTable(
                name: "promotion_rules");

            migrationBuilder.DropTable(
                name: "promotion_usages");

            migrationBuilder.DropTable(
                name: "refunds");

            migrationBuilder.DropTable(
                name: "reorder_rules");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "stock_adjustment_lines");

            migrationBuilder.DropTable(
                name: "stock_ledger");

            migrationBuilder.DropTable(
                name: "store_configs");

            migrationBuilder.DropTable(
                name: "store_product_overrides");

            migrationBuilder.DropTable(
                name: "suspended_transactions");

            migrationBuilder.DropTable(
                name: "sync_logs");

            migrationBuilder.DropTable(
                name: "sync_queue");

            migrationBuilder.DropTable(
                name: "tax_breakdowns");

            migrationBuilder.DropTable(
                name: "tier_configs");

            migrationBuilder.DropTable(
                name: "uom_conversions");

            migrationBuilder.DropTable(
                name: "vendor_invoices");

            migrationBuilder.DropTable(
                name: "void_logs");

            migrationBuilder.DropTable(
                name: "coupon_campaigns");

            migrationBuilder.DropTable(
                name: "goods_receipts");

            migrationBuilder.DropTable(
                name: "inter_store_transfers");

            migrationBuilder.DropTable(
                name: "loyalty_accounts");

            migrationBuilder.DropTable(
                name: "price_lists");

            migrationBuilder.DropTable(
                name: "promotion_rule_groups");

            migrationBuilder.DropTable(
                name: "stock_adjustments");

            migrationBuilder.DropTable(
                name: "transaction_lines");

            migrationBuilder.DropTable(
                name: "purchase_orders");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "hsn_codes");

            migrationBuilder.DropTable(
                name: "uoms");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "terminals");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "stores");

            migrationBuilder.DropTable(
                name: "organizations");
        }
    }
}

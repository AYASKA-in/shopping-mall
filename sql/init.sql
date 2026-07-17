CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE brands (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" text,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_brands" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE cash_declarations (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "TerminalId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "SessionId" uuid,
        "DeclarationType" character varying(10) NOT NULL,
        "Count2000" integer NOT NULL,
        "Count500" integer NOT NULL,
        "Count200" integer NOT NULL,
        "Count100" integer NOT NULL,
        "Count50" integer NOT NULL,
        "Count20" integer NOT NULL,
        "Count10" integer NOT NULL,
        "CountCoins" integer NOT NULL,
        "TotalAmount" numeric NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_cash_declarations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE cash_movements (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "TerminalId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "MovementType" character varying(30) NOT NULL,
        "Amount" numeric NOT NULL,
        "Reason" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_cash_movements" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE categories (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" text,
        "ParentCategoryId" uuid,
        "SortOrder" integer NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_categories" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_categories_categories_ParentCategoryId" FOREIGN KEY ("ParentCategoryId") REFERENCES categories ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE cloud_backups (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "FileName" character varying(255) NOT NULL,
        "FileSizeBytes" bigint NOT NULL,
        "Checksum" text,
        "StorageUrl" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_cloud_backups" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE coupon_campaigns (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "PromotionId" uuid,
        "MaxUses" integer NOT NULL,
        "MaxUsesPerCustomer" integer,
        "MinOrderAmount" numeric,
        "StartDate" timestamp with time zone NOT NULL,
        "EndDate" timestamp with time zone NOT NULL,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_coupon_campaigns" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE customers (
        "Id" uuid NOT NULL,
        "FirstName" text,
        "LastName" text,
        "Phone" character varying(20),
        "Email" character varying(200),
        "DateOfBirth" date,
        "Address" text,
        "City" text,
        "State" text,
        "PostalCode" text,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_customers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE debit_notes (
        "Id" uuid NOT NULL,
        "DebitNoteNo" character varying(30) NOT NULL,
        "GRNId" uuid NOT NULL,
        "POId" uuid NOT NULL,
        "SupplierId" uuid NOT NULL,
        "ReturnType" text NOT NULL,
        "Status" character varying(15) NOT NULL,
        "TotalAmount" numeric NOT NULL,
        "TaxAdjustment" numeric NOT NULL,
        "NetAdjustment" numeric NOT NULL,
        "Notes" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_debit_notes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE hsn_codes (
        "Id" uuid NOT NULL,
        "Code" character varying(10) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "CGSTRate" numeric NOT NULL,
        "SGSTRate" numeric NOT NULL,
        "IGSTRate" numeric NOT NULL,
        "CessRate" numeric NOT NULL,
        "EffectiveFrom" date NOT NULL,
        "EffectiveTo" date,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_hsn_codes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE inter_store_transfers (
        "Id" uuid NOT NULL,
        "TransferNumber" character varying(30) NOT NULL,
        "FromStoreId" uuid NOT NULL,
        "ToStoreId" uuid NOT NULL,
        "Status" character varying(20) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "ShippedAt" timestamp with time zone,
        "ReceivedAt" timestamp with time zone,
        "CreatedByUserId" uuid NOT NULL,
        "Notes" text,
        CONSTRAINT "PK_inter_store_transfers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE organizations (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "LegalName" character varying(200) NOT NULL,
        "GSTIN" character varying(15),
        "PAN" character varying(10),
        "AddressLine1" character varying(255),
        "AddressLine2" character varying(255),
        "City" character varying(100),
        "State" character varying(100),
        "PostalCode" character varying(20),
        "Country" text,
        "Phone" character varying(20),
        "Email" character varying(200),
        "Website" text,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_organizations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE price_lists (
        "Id" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "Description" text,
        "EffectiveFrom" date NOT NULL,
        "EffectiveTo" date,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_price_lists" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE promotion_usages (
        "Id" uuid NOT NULL,
        "PromotionId" uuid NOT NULL,
        "TransactionId" uuid NOT NULL,
        "DiscountAmount" numeric NOT NULL,
        "CouponCodeId" uuid,
        "UsedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_promotion_usages" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE promotions (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" text,
        "Type" character varying(20) NOT NULL,
        "StartDate" timestamp with time zone NOT NULL,
        "EndDate" timestamp with time zone NOT NULL,
        "Priority" integer NOT NULL,
        "Stackability" character varying(15) NOT NULL,
        "Budget" numeric,
        "DailyBudget" numeric,
        "BudgetSpent" numeric,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        "CreatedByUserId" uuid NOT NULL,
        CONSTRAINT "PK_promotions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE reorder_rules (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "ReorderLevel" numeric NOT NULL,
        "ReorderQty" numeric NOT NULL,
        "MaxStock" numeric NOT NULL,
        "SafetyStock" numeric NOT NULL,
        "PreferredSupplierId" uuid,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_reorder_rules" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE role_permissions (
        "Id" uuid NOT NULL,
        "Role" character varying(20) NOT NULL,
        "PermissionName" character varying(100) NOT NULL,
        "Resource" text,
        "CanRead" boolean NOT NULL,
        "CanWrite" boolean NOT NULL,
        "CanDelete" boolean NOT NULL,
        CONSTRAINT "PK_role_permissions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE stock_adjustments (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "AdjustmentNumber" character varying(30) NOT NULL,
        "Type" character varying(20) NOT NULL,
        "Reason" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "CreatedByUserId" uuid NOT NULL,
        "IsPosted" boolean NOT NULL,
        CONSTRAINT "PK_stock_adjustments" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE suppliers (
        "Id" uuid NOT NULL,
        "Code" character varying(20) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "LegalName" text,
        "GSTIN" character varying(15),
        "PAN" character varying(10),
        "ContactPerson" text,
        "Phone" text,
        "Email" text,
        "AddressLine1" text,
        "City" text,
        "State" text,
        "PostalCode" text,
        "CreditLimit" numeric,
        "CreditDays" integer,
        "Tier" character varying(15) NOT NULL,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_suppliers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE suspended_transactions (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "TerminalId" uuid NOT NULL,
        "UserId" uuid,
        "BasketData" text NOT NULL,
        "BasketTotal" numeric NOT NULL,
        "ItemCount" integer NOT NULL,
        "SuspendedAt" timestamp with time zone NOT NULL,
        "RecalledAt" timestamp with time zone,
        "IsRecalled" boolean NOT NULL,
        CONSTRAINT "PK_suspended_transactions" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE sync_logs (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "Direction" character varying(10) NOT NULL,
        "Status" character varying(10) NOT NULL,
        "ItemsProcessed" integer NOT NULL,
        "ItemsFailed" integer NOT NULL,
        "ErrorMessage" text,
        "StartedAt" timestamp with time zone NOT NULL,
        "CompletedAt" timestamp with time zone,
        CONSTRAINT "PK_sync_logs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE sync_queue (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "EntityType" character varying(50) NOT NULL,
        "EntityId" text NOT NULL,
        "Operation" character varying(20) NOT NULL,
        "Payload" text NOT NULL,
        "Status" character varying(10) NOT NULL,
        "RetryCount" integer NOT NULL,
        "ErrorMessage" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        "ProcessedAt" timestamp with time zone,
        CONSTRAINT "PK_sync_queue" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE tier_configs (
        "Id" uuid NOT NULL,
        "Tier" character varying(10) NOT NULL,
        "MinimumLifetimePoints" integer NOT NULL,
        "PointsMultiplier" numeric NOT NULL,
        "Benefits" text,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_tier_configs" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE uoms (
        "Id" uuid NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Abbreviation" character varying(10) NOT NULL,
        "Category" character varying(20) NOT NULL,
        CONSTRAINT "PK_uoms" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE coupon_codes (
        "Id" uuid NOT NULL,
        "CampaignId" uuid NOT NULL,
        "Code" character varying(30) NOT NULL,
        "Status" character varying(10) NOT NULL,
        "UseCount" integer NOT NULL,
        "UsedAt" timestamp with time zone,
        CONSTRAINT "PK_coupon_codes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_coupon_codes_coupon_campaigns_CampaignId" FOREIGN KEY ("CampaignId") REFERENCES coupon_campaigns ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE loyalty_accounts (
        "Id" uuid NOT NULL,
        "CustomerId" uuid NOT NULL,
        "CardNumber" text,
        "PointsBalance" integer NOT NULL,
        "LifetimePoints" integer NOT NULL,
        "Tier" character varying(10) NOT NULL,
        "LastEarnedAt" timestamp with time zone,
        "LastRedeemedAt" timestamp with time zone,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_loyalty_accounts" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_loyalty_accounts_customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES customers ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE stores (
        "Id" uuid NOT NULL,
        "OrganizationId" uuid NOT NULL,
        "Code" character varying(20) NOT NULL,
        "Name" character varying(200) NOT NULL,
        "GSTIN" character varying(15),
        "AddressLine1" character varying(255),
        "AddressLine2" text,
        "City" character varying(100),
        "State" character varying(100),
        "PostalCode" character varying(20),
        "Phone" character varying(20),
        "Email" character varying(200),
        "Status" character varying(20) NOT NULL,
        "ReceiptFooter" text,
        "TimeZone" text,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_stores" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_stores_organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES organizations ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE promotion_benefits (
        "Id" uuid NOT NULL,
        "PromotionId" uuid NOT NULL,
        "BenefitType" character varying(30) NOT NULL,
        "Value" numeric NOT NULL,
        "MaxDiscount" numeric,
        "BuyQty" integer,
        "GetQty" integer,
        "FreeProductId" uuid,
        "ApplicableOn" text,
        CONSTRAINT "PK_promotion_benefits" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_promotion_benefits_promotions_PromotionId" FOREIGN KEY ("PromotionId") REFERENCES promotions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE promotion_rule_groups (
        "Id" uuid NOT NULL,
        "PromotionId" uuid NOT NULL,
        "Combinator" character varying(5) NOT NULL,
        "SortOrder" integer NOT NULL,
        CONSTRAINT "PK_promotion_rule_groups" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_promotion_rule_groups_promotions_PromotionId" FOREIGN KEY ("PromotionId") REFERENCES promotions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE products (
        "Id" uuid NOT NULL,
        "SKU" character varying(50) NOT NULL,
        "Name" character varying(255) NOT NULL,
        "Description" text,
        "CategoryId" uuid NOT NULL,
        "BrandId" uuid,
        "BaseUOMId" uuid NOT NULL,
        "HSNId" uuid,
        "HSNCode" character varying(10),
        "TaxRate" numeric NOT NULL,
        "Mrp" numeric,
        "SellingPrice" numeric,
        "PurchasePrice" numeric,
        "IsWeighable" boolean NOT NULL,
        "PLUCode" text,
        "IsAgeRestricted" boolean NOT NULL,
        "MinAge" integer,
        "IsActive" boolean NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_products" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_products_brands_BrandId" FOREIGN KEY ("BrandId") REFERENCES brands ("Id"),
        CONSTRAINT "FK_products_categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES categories ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_products_hsn_codes_HSNId" FOREIGN KEY ("HSNId") REFERENCES hsn_codes ("Id"),
        CONSTRAINT "FK_products_uoms_BaseUOMId" FOREIGN KEY ("BaseUOMId") REFERENCES uoms ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE uom_conversions (
        "Id" uuid NOT NULL,
        "FromUOMId" uuid NOT NULL,
        "ToUOMId" uuid NOT NULL,
        "ConversionFactor" numeric NOT NULL,
        CONSTRAINT "PK_uom_conversions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_uom_conversions_uoms_FromUOMId" FOREIGN KEY ("FromUOMId") REFERENCES uoms ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_uom_conversions_uoms_ToUOMId" FOREIGN KEY ("ToUOMId") REFERENCES uoms ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE loyalty_transactions (
        "Id" uuid NOT NULL,
        "LoyaltyAccountId" uuid NOT NULL,
        "Type" character varying(10) NOT NULL,
        "Points" integer NOT NULL,
        "BalanceBefore" integer NOT NULL,
        "BalanceAfter" integer NOT NULL,
        "ReferenceType" text,
        "ReferenceId" uuid,
        "Description" text,
        "IdempotencyKey" uuid,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_loyalty_transactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId" FOREIGN KEY ("LoyaltyAccountId") REFERENCES loyalty_accounts ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE purchase_orders (
        "Id" uuid NOT NULL,
        "PONumber" character varying(30) NOT NULL,
        "StoreId" uuid NOT NULL,
        "SupplierId" uuid NOT NULL,
        "Status" character varying(20) NOT NULL,
        "ExpectedDeliveryDate" date,
        "PromisedDeliveryDate" date,
        "SubTotal" numeric NOT NULL,
        "DiscountAmount" numeric NOT NULL,
        "TaxAmount" numeric NOT NULL,
        "GrandTotal" numeric NOT NULL,
        "Notes" text,
        "CreatedByUserId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_purchase_orders" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_purchase_orders_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_purchase_orders_suppliers_SupplierId" FOREIGN KEY ("SupplierId") REFERENCES suppliers ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE store_configs (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "ConfigKey" character varying(100) NOT NULL,
        "ConfigValue" character varying(500) NOT NULL,
        "Description" text,
        CONSTRAINT "PK_store_configs" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_store_configs_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE terminals (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "DeviceId" character varying(100),
        "Mode" character varying(10) NOT NULL,
        "IpAddress" character varying(45),
        "IsActive" boolean NOT NULL,
        "LastHeartbeat" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_terminals" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_terminals_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE users (
        "Id" uuid NOT NULL,
        "StoreId" uuid,
        "Username" character varying(50) NOT NULL,
        "DisplayName" character varying(100) NOT NULL,
        "PinHash" character varying(255) NOT NULL,
        "Email" text,
        "Phone" text,
        "Role" character varying(20) NOT NULL,
        "IsActive" boolean NOT NULL,
        "LastLoginAt" timestamp with time zone,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_users" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_users_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE promotion_rules (
        "Id" uuid NOT NULL,
        "RuleGroupId" uuid NOT NULL,
        "RuleType" character varying(50) NOT NULL,
        "Operator" character varying(20) NOT NULL,
        "Value" text NOT NULL,
        CONSTRAINT "PK_promotion_rules" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_promotion_rules_promotion_rule_groups_RuleGroupId" FOREIGN KEY ("RuleGroupId") REFERENCES promotion_rule_groups ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE barcodes (
        "Id" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "Code" character varying(50) NOT NULL,
        "Type" character varying(20) NOT NULL,
        "IsDefault" boolean NOT NULL,
        CONSTRAINT "PK_barcodes" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_barcodes_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE current_stock (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "OnHand" numeric NOT NULL,
        "Reserved" numeric NOT NULL,
        "Available" numeric NOT NULL,
        "OnOrder" numeric NOT NULL,
        "LastUpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_current_stock" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_current_stock_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_current_stock_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE inter_store_transfer_lines (
        "Id" uuid NOT NULL,
        "TransferId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "RequestedQty" numeric NOT NULL,
        "ShippedQty" numeric NOT NULL,
        "ReceivedQty" numeric NOT NULL,
        CONSTRAINT "PK_inter_store_transfer_lines" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_inter_store_transfer_lines_inter_store_transfers_TransferId" FOREIGN KEY ("TransferId") REFERENCES inter_store_transfers ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_inter_store_transfer_lines_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE price_list_lines (
        "Id" uuid NOT NULL,
        "PriceListId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "UnitPrice" numeric NOT NULL,
        "DiscountPercent" numeric,
        CONSTRAINT "PK_price_list_lines" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_price_list_lines_price_lists_PriceListId" FOREIGN KEY ("PriceListId") REFERENCES price_lists ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_price_list_lines_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE product_uoms (
        "Id" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "UOMId" uuid NOT NULL,
        "IsBaseUOM" boolean NOT NULL,
        "IsPurchaseUOM" boolean NOT NULL,
        "IsSalesUOM" boolean NOT NULL,
        "ConversionFactor" numeric NOT NULL,
        CONSTRAINT "PK_product_uoms" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_product_uoms_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_product_uoms_uoms_UOMId" FOREIGN KEY ("UOMId") REFERENCES uoms ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE stock_adjustment_lines (
        "Id" uuid NOT NULL,
        "AdjustmentId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "QuantityChange" numeric NOT NULL,
        "UnitCost" numeric NOT NULL,
        "Reason" text,
        CONSTRAINT "PK_stock_adjustment_lines" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_stock_adjustment_lines_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_stock_adjustment_lines_stock_adjustments_AdjustmentId" FOREIGN KEY ("AdjustmentId") REFERENCES stock_adjustments ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE stock_ledger (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "MovementType" character varying(20) NOT NULL,
        "ReferenceType" text,
        "ReferenceId" uuid,
        "Quantity" numeric NOT NULL,
        "QuantityBefore" numeric NOT NULL,
        "QuantityAfter" numeric NOT NULL,
        "UnitCost" numeric NOT NULL,
        "TotalCost" numeric NOT NULL,
        "BatchId" uuid,
        "LotNumber" text,
        "Notes" text,
        "CreatedByUserId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_stock_ledger" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_stock_ledger_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_stock_ledger_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE store_product_overrides (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "OverridePrice" numeric,
        "IsAvailable" boolean NOT NULL,
        CONSTRAINT "PK_store_product_overrides" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_store_product_overrides_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_store_product_overrides_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE goods_receipts (
        "Id" uuid NOT NULL,
        "GRNNumber" character varying(30) NOT NULL,
        "StoreId" uuid NOT NULL,
        "POId" uuid,
        "SupplierId" uuid NOT NULL,
        "DeliveryChallanNo" text,
        "VehicleNo" text,
        "Status" character varying(20) NOT NULL,
        "ReceiptDate" timestamp with time zone NOT NULL,
        "Notes" text,
        "CreatedByUserId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_goods_receipts" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_goods_receipts_purchase_orders_POId" FOREIGN KEY ("POId") REFERENCES purchase_orders ("Id"),
        CONSTRAINT "FK_goods_receipts_suppliers_SupplierId" FOREIGN KEY ("SupplierId") REFERENCES suppliers ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE po_lines (
        "Id" uuid NOT NULL,
        "POId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "LineNo" integer NOT NULL,
        "OrderedQty" numeric NOT NULL,
        "ReceivedQty" numeric NOT NULL,
        "AcceptedQty" numeric NOT NULL,
        "RejectedQty" numeric NOT NULL,
        "UnitPrice" numeric NOT NULL,
        "TaxRate" numeric NOT NULL,
        "NetAmount" numeric NOT NULL,
        "RequiredDate" date,
        "IsBackorder" boolean NOT NULL,
        CONSTRAINT "PK_po_lines" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_po_lines_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_po_lines_purchase_orders_POId" FOREIGN KEY ("POId") REFERENCES purchase_orders ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE vendor_invoices (
        "Id" uuid NOT NULL,
        "InvoiceNo" character varying(50) NOT NULL,
        "POId" uuid NOT NULL,
        "SupplierId" uuid NOT NULL,
        "InvoiceDate" date NOT NULL,
        "GrossAmount" numeric NOT NULL,
        "DiscountAmount" numeric NOT NULL,
        "TaxAmount" numeric NOT NULL,
        "NetAmount" numeric NOT NULL,
        "MatchStatus" character varying(15) NOT NULL,
        "PaymentStatus" character varying(10) NOT NULL,
        "DueDate" date,
        "Notes" text,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_vendor_invoices" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_vendor_invoices_purchase_orders_POId" FOREIGN KEY ("POId") REFERENCES purchase_orders ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_vendor_invoices_suppliers_SupplierId" FOREIGN KEY ("SupplierId") REFERENCES suppliers ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE sessions (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "TerminalId" uuid NOT NULL,
        "LoginAt" timestamp with time zone NOT NULL,
        "LogoutAt" timestamp with time zone,
        "IsActive" boolean NOT NULL,
        CONSTRAINT "PK_sessions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_sessions_terminals_TerminalId" FOREIGN KEY ("TerminalId") REFERENCES terminals ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_sessions_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE transactions (
        "Id" uuid NOT NULL,
        "StoreId" uuid NOT NULL,
        "TerminalId" uuid NOT NULL,
        "UserId" uuid,
        "CustomerId" uuid,
        "ReceiptNumber" character varying(30) NOT NULL,
        "Status" character varying(20) NOT NULL,
        "SubTotal" numeric NOT NULL,
        "DiscountTotal" numeric NOT NULL,
        "TaxTotal" numeric NOT NULL,
        "GrandTotal" numeric NOT NULL,
        "RoundingAmount" numeric NOT NULL,
        "Notes" text,
        "IdempotencyKey" uuid,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_transactions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_transactions_customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES customers ("Id"),
        CONSTRAINT "FK_transactions_stores_StoreId" FOREIGN KEY ("StoreId") REFERENCES stores ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_transactions_terminals_TerminalId" FOREIGN KEY ("TerminalId") REFERENCES terminals ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_transactions_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE grn_lines (
        "Id" uuid NOT NULL,
        "GRNId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "ReceivedQty" numeric NOT NULL,
        "AcceptedQty" numeric NOT NULL,
        "RejectedQty" numeric NOT NULL,
        "UnitPrice" numeric NOT NULL,
        "BatchNo" text,
        "ExpiryDate" date,
        "MfgDate" date,
        "InspectionStatus" character varying(15) NOT NULL,
        "RejectionReason" text,
        CONSTRAINT "PK_grn_lines" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_grn_lines_goods_receipts_GRNId" FOREIGN KEY ("GRNId") REFERENCES goods_receipts ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_grn_lines_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE payments (
        "Id" uuid NOT NULL,
        "TransactionId" uuid NOT NULL,
        "Method" character varying(20) NOT NULL,
        "Amount" numeric NOT NULL,
        "TenderedAmount" numeric,
        "ChangeAmount" numeric,
        "ReferenceNumber" character varying(100),
        "GatewayResponse" text,
        "Status" character varying(20) NOT NULL,
        "IdempotencyKey" uuid,
        "CreatedAt" timestamp with time zone NOT NULL,
        "SettledAt" timestamp with time zone,
        CONSTRAINT "PK_payments" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_payments_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES transactions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE refunds (
        "Id" uuid NOT NULL,
        "OriginalTransactionId" uuid NOT NULL,
        "RefundTransactionId" uuid,
        "RefundAmount" numeric NOT NULL,
        "Reason" text NOT NULL,
        "RefundMethod" character varying(20) NOT NULL,
        "ApprovedByUserId" uuid,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_refunds" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_refunds_transactions_OriginalTransactionId" FOREIGN KEY ("OriginalTransactionId") REFERENCES transactions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE transaction_lines (
        "Id" uuid NOT NULL,
        "TransactionId" uuid NOT NULL,
        "ProductId" uuid NOT NULL,
        "LineNumber" integer NOT NULL,
        "ProductName" character varying(255) NOT NULL,
        "SKU" text,
        "Barcode" text,
        "Quantity" numeric NOT NULL,
        "UOMId" uuid NOT NULL,
        "UnitPrice" numeric NOT NULL,
        "Mrp" numeric NOT NULL,
        "DiscountAmount" numeric NOT NULL,
        "DiscountPercent" numeric,
        "DiscountReason" text,
        "TaxRate" numeric NOT NULL,
        "TaxAmount" numeric NOT NULL,
        "NetAmount" numeric NOT NULL,
        "IsWeighable" boolean NOT NULL,
        "WeightKg" numeric,
        "ParentLineId" uuid,
        CONSTRAINT "PK_transaction_lines" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_transaction_lines_products_ProductId" FOREIGN KEY ("ProductId") REFERENCES products ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_transaction_lines_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES transactions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE void_logs (
        "Id" uuid NOT NULL,
        "TransactionId" uuid NOT NULL,
        "VoidedByUserId" uuid,
        "ApprovedByUserId" uuid,
        "Reason" character varying(500) NOT NULL,
        "ReasonCode" text,
        "VoidedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_void_logs" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_void_logs_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES transactions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE TABLE tax_breakdowns (
        "Id" uuid NOT NULL,
        "TransactionId" uuid NOT NULL,
        "TransactionLineId" uuid,
        "TaxType" character varying(10) NOT NULL,
        "TaxableAmount" numeric NOT NULL,
        "TaxRate" numeric NOT NULL,
        "TaxAmount" numeric NOT NULL,
        CONSTRAINT "PK_tax_breakdowns" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_tax_breakdowns_transaction_lines_TransactionLineId" FOREIGN KEY ("TransactionLineId") REFERENCES transaction_lines ("Id"),
        CONSTRAINT "FK_tax_breakdowns_transactions_TransactionId" FOREIGN KEY ("TransactionId") REFERENCES transactions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_barcodes_Code" ON barcodes ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_barcodes_ProductId" ON barcodes ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_categories_ParentCategoryId" ON categories ("ParentCategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_coupon_codes_CampaignId" ON coupon_codes ("CampaignId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_coupon_codes_Code" ON coupon_codes ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_current_stock_ProductId" ON current_stock ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_current_stock_StoreId_ProductId" ON current_stock ("StoreId", "ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_customers_Phone" ON customers ("Phone") WHERE "Phone" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_goods_receipts_GRNNumber" ON goods_receipts ("GRNNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_goods_receipts_POId" ON goods_receipts ("POId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_goods_receipts_SupplierId" ON goods_receipts ("SupplierId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_grn_lines_GRNId" ON grn_lines ("GRNId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_grn_lines_ProductId" ON grn_lines ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_hsn_codes_Code" ON hsn_codes ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_inter_store_transfer_lines_ProductId" ON inter_store_transfer_lines ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_inter_store_transfer_lines_TransferId" ON inter_store_transfer_lines ("TransferId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_loyalty_accounts_CustomerId" ON loyalty_accounts ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_loyalty_transactions_IdempotencyKey" ON loyalty_transactions ("IdempotencyKey") WHERE "IdempotencyKey" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_loyalty_transactions_LoyaltyAccountId" ON loyalty_transactions ("LoyaltyAccountId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_payments_IdempotencyKey" ON payments ("IdempotencyKey") WHERE "IdempotencyKey" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_payments_TransactionId" ON payments ("TransactionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_po_lines_POId" ON po_lines ("POId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_po_lines_ProductId" ON po_lines ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_price_list_lines_PriceListId" ON price_list_lines ("PriceListId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_price_list_lines_ProductId" ON price_list_lines ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_product_uoms_ProductId" ON product_uoms ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_product_uoms_UOMId" ON product_uoms ("UOMId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_products_BaseUOMId" ON products ("BaseUOMId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_products_BrandId" ON products ("BrandId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_products_CategoryId" ON products ("CategoryId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_products_HSNId" ON products ("HSNId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_products_SKU" ON products ("SKU");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_promotion_benefits_PromotionId" ON promotion_benefits ("PromotionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_promotion_rule_groups_PromotionId" ON promotion_rule_groups ("PromotionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_promotion_rules_RuleGroupId" ON promotion_rules ("RuleGroupId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_purchase_orders_PONumber" ON purchase_orders ("PONumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_purchase_orders_StoreId" ON purchase_orders ("StoreId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_purchase_orders_SupplierId" ON purchase_orders ("SupplierId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_refunds_OriginalTransactionId" ON refunds ("OriginalTransactionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_reorder_rules_StoreId_ProductId" ON reorder_rules ("StoreId", "ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_sessions_TerminalId" ON sessions ("TerminalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_sessions_UserId" ON sessions ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_stock_adjustment_lines_AdjustmentId" ON stock_adjustment_lines ("AdjustmentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_stock_adjustment_lines_ProductId" ON stock_adjustment_lines ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_stock_ledger_ProductId" ON stock_ledger ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_stock_ledger_StoreId_ProductId_CreatedAt" ON stock_ledger ("StoreId", "ProductId", "CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_store_configs_StoreId_ConfigKey" ON store_configs ("StoreId", "ConfigKey");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_store_product_overrides_ProductId" ON store_product_overrides ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_store_product_overrides_StoreId_ProductId" ON store_product_overrides ("StoreId", "ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_stores_Code" ON stores ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_stores_OrganizationId" ON stores ("OrganizationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_suppliers_Code" ON suppliers ("Code");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_sync_queue_StoreId_Status_CreatedAt" ON sync_queue ("StoreId", "Status", "CreatedAt");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_tax_breakdowns_TransactionId" ON tax_breakdowns ("TransactionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_tax_breakdowns_TransactionLineId" ON tax_breakdowns ("TransactionLineId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_terminals_StoreId" ON terminals ("StoreId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_transaction_lines_ProductId" ON transaction_lines ("ProductId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_transaction_lines_TransactionId" ON transaction_lines ("TransactionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_transactions_CustomerId" ON transactions ("CustomerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_transactions_IdempotencyKey" ON transactions ("IdempotencyKey") WHERE "IdempotencyKey" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_transactions_ReceiptNumber" ON transactions ("ReceiptNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_transactions_StoreId" ON transactions ("StoreId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_transactions_TerminalId" ON transactions ("TerminalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_transactions_UserId" ON transactions ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_uom_conversions_FromUOMId" ON uom_conversions ("FromUOMId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_uom_conversions_ToUOMId" ON uom_conversions ("ToUOMId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_users_StoreId" ON users ("StoreId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_users_Username" ON users ("Username");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_vendor_invoices_POId" ON vendor_invoices ("POId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_vendor_invoices_SupplierId" ON vendor_invoices ("SupplierId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    CREATE INDEX "IX_void_logs_TransactionId" ON void_logs ("TransactionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260717165515_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260717165515_InitialCreate', '8.0.0');
    END IF;
END $EF$;
COMMIT;


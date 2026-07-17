using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingMall.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAuditGaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_barcodes_products_ProductId",
                table: "barcodes");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_coupon_codes_coupon_campaigns_CampaignId",
                table: "coupon_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_current_stock_products_ProductId",
                table: "current_stock");

            migrationBuilder.DropForeignKey(
                name: "FK_current_stock_stores_StoreId",
                table: "current_stock");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_purchase_orders_POId",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_suppliers_SupplierId",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_grn_lines_goods_receipts_GRNId",
                table: "grn_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_grn_lines_products_ProductId",
                table: "grn_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_inter_store_transfer_lines_inter_store_transfers_TransferId",
                table: "inter_store_transfer_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_inter_store_transfer_lines_products_ProductId",
                table: "inter_store_transfer_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_loyalty_accounts_customers_CustomerId",
                table: "loyalty_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId",
                table: "loyalty_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_transactions_TransactionId",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "FK_po_lines_products_ProductId",
                table: "po_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_po_lines_purchase_orders_POId",
                table: "po_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_price_list_lines_price_lists_PriceListId",
                table: "price_list_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_price_list_lines_products_ProductId",
                table: "price_list_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_product_uoms_products_ProductId",
                table: "product_uoms");

            migrationBuilder.DropForeignKey(
                name: "FK_product_uoms_uoms_UOMId",
                table: "product_uoms");

            migrationBuilder.DropForeignKey(
                name: "FK_products_brands_BrandId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_hsn_codes_HSNId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_uoms_BaseUOMId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_promotion_benefits_promotions_PromotionId",
                table: "promotion_benefits");

            migrationBuilder.DropForeignKey(
                name: "FK_promotion_rule_groups_promotions_PromotionId",
                table: "promotion_rule_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_promotion_rules_promotion_rule_groups_RuleGroupId",
                table: "promotion_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_stores_StoreId",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_suppliers_SupplierId",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_refunds_transactions_OriginalTransactionId",
                table: "refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_terminals_TerminalId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustment_lines_products_ProductId",
                table: "stock_adjustment_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustment_lines_stock_adjustments_AdjustmentId",
                table: "stock_adjustment_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_ledger_products_ProductId",
                table: "stock_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_ledger_stores_StoreId",
                table: "stock_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_store_configs_stores_StoreId",
                table: "store_configs");

            migrationBuilder.DropForeignKey(
                name: "FK_store_product_overrides_products_ProductId",
                table: "store_product_overrides");

            migrationBuilder.DropForeignKey(
                name: "FK_store_product_overrides_stores_StoreId",
                table: "store_product_overrides");

            migrationBuilder.DropForeignKey(
                name: "FK_stores_organizations_OrganizationId",
                table: "stores");

            migrationBuilder.DropForeignKey(
                name: "FK_tax_breakdowns_transaction_lines_TransactionLineId",
                table: "tax_breakdowns");

            migrationBuilder.DropForeignKey(
                name: "FK_tax_breakdowns_transactions_TransactionId",
                table: "tax_breakdowns");

            migrationBuilder.DropForeignKey(
                name: "FK_terminals_stores_StoreId",
                table: "terminals");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_lines_products_ProductId",
                table: "transaction_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_lines_transactions_TransactionId",
                table: "transaction_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_customers_CustomerId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_stores_StoreId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_terminals_TerminalId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_uom_conversions_uoms_FromUOMId",
                table: "uom_conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_uom_conversions_uoms_ToUOMId",
                table: "uom_conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_users_stores_StoreId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_invoices_purchase_orders_POId",
                table: "vendor_invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_invoices_suppliers_SupplierId",
                table: "vendor_invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_void_logs_transactions_TransactionId",
                table: "void_logs");

            migrationBuilder.DropIndex(
                name: "IX_transactions_StoreId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_payments_TransactionId",
                table: "payments");

            migrationBuilder.AddColumn<string>(
                name: "PinSalt",
                table: "users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "current_stock",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_StoreId_Status_CreatedAt",
                table: "transactions",
                columns: new[] { "StoreId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_payments_TransactionId_Method_CreatedAt",
                table: "payments",
                columns: new[] { "TransactionId", "Method", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_barcodes_products_ProductId",
                table: "barcodes",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                table: "categories",
                column: "ParentCategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_coupon_codes_coupon_campaigns_CampaignId",
                table: "coupon_codes",
                column: "CampaignId",
                principalTable: "coupon_campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_current_stock_products_ProductId",
                table: "current_stock",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_current_stock_stores_StoreId",
                table: "current_stock",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_purchase_orders_POId",
                table: "goods_receipts",
                column: "POId",
                principalTable: "purchase_orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_suppliers_SupplierId",
                table: "goods_receipts",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grn_lines_goods_receipts_GRNId",
                table: "grn_lines",
                column: "GRNId",
                principalTable: "goods_receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_grn_lines_products_ProductId",
                table: "grn_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inter_store_transfer_lines_inter_store_transfers_TransferId",
                table: "inter_store_transfer_lines",
                column: "TransferId",
                principalTable: "inter_store_transfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inter_store_transfer_lines_products_ProductId",
                table: "inter_store_transfer_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_loyalty_accounts_customers_CustomerId",
                table: "loyalty_accounts",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId",
                table: "loyalty_transactions",
                column: "LoyaltyAccountId",
                principalTable: "loyalty_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_transactions_TransactionId",
                table: "payments",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_po_lines_products_ProductId",
                table: "po_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_po_lines_purchase_orders_POId",
                table: "po_lines",
                column: "POId",
                principalTable: "purchase_orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_price_list_lines_price_lists_PriceListId",
                table: "price_list_lines",
                column: "PriceListId",
                principalTable: "price_lists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_price_list_lines_products_ProductId",
                table: "price_list_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_uoms_products_ProductId",
                table: "product_uoms",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_uoms_uoms_UOMId",
                table: "product_uoms",
                column: "UOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_brands_BrandId",
                table: "products",
                column: "BrandId",
                principalTable: "brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_hsn_codes_HSNId",
                table: "products",
                column: "HSNId",
                principalTable: "hsn_codes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_uoms_BaseUOMId",
                table: "products",
                column: "BaseUOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_promotion_benefits_promotions_PromotionId",
                table: "promotion_benefits",
                column: "PromotionId",
                principalTable: "promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_promotion_rule_groups_promotions_PromotionId",
                table: "promotion_rule_groups",
                column: "PromotionId",
                principalTable: "promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_promotion_rules_promotion_rule_groups_RuleGroupId",
                table: "promotion_rules",
                column: "RuleGroupId",
                principalTable: "promotion_rule_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_stores_StoreId",
                table: "purchase_orders",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_suppliers_SupplierId",
                table: "purchase_orders",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_refunds_transactions_OriginalTransactionId",
                table: "refunds",
                column: "OriginalTransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_terminals_TerminalId",
                table: "sessions",
                column: "TerminalId",
                principalTable: "terminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustment_lines_products_ProductId",
                table: "stock_adjustment_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustment_lines_stock_adjustments_AdjustmentId",
                table: "stock_adjustment_lines",
                column: "AdjustmentId",
                principalTable: "stock_adjustments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_ledger_products_ProductId",
                table: "stock_ledger",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_ledger_stores_StoreId",
                table: "stock_ledger",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_store_configs_stores_StoreId",
                table: "store_configs",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_store_product_overrides_products_ProductId",
                table: "store_product_overrides",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_store_product_overrides_stores_StoreId",
                table: "store_product_overrides",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stores_organizations_OrganizationId",
                table: "stores",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tax_breakdowns_transaction_lines_TransactionLineId",
                table: "tax_breakdowns",
                column: "TransactionLineId",
                principalTable: "transaction_lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tax_breakdowns_transactions_TransactionId",
                table: "tax_breakdowns",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_terminals_stores_StoreId",
                table: "terminals",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_lines_products_ProductId",
                table: "transaction_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_lines_transactions_TransactionId",
                table: "transaction_lines",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_customers_CustomerId",
                table: "transactions",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_stores_StoreId",
                table: "transactions",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_terminals_TerminalId",
                table: "transactions",
                column: "TerminalId",
                principalTable: "terminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_uom_conversions_uoms_FromUOMId",
                table: "uom_conversions",
                column: "FromUOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_uom_conversions_uoms_ToUOMId",
                table: "uom_conversions",
                column: "ToUOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_stores_StoreId",
                table: "users",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_invoices_purchase_orders_POId",
                table: "vendor_invoices",
                column: "POId",
                principalTable: "purchase_orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_invoices_suppliers_SupplierId",
                table: "vendor_invoices",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_void_logs_transactions_TransactionId",
                table: "void_logs",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_barcodes_products_ProductId",
                table: "barcodes");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_coupon_codes_coupon_campaigns_CampaignId",
                table: "coupon_codes");

            migrationBuilder.DropForeignKey(
                name: "FK_current_stock_products_ProductId",
                table: "current_stock");

            migrationBuilder.DropForeignKey(
                name: "FK_current_stock_stores_StoreId",
                table: "current_stock");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_purchase_orders_POId",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_suppliers_SupplierId",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_grn_lines_goods_receipts_GRNId",
                table: "grn_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_grn_lines_products_ProductId",
                table: "grn_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_inter_store_transfer_lines_inter_store_transfers_TransferId",
                table: "inter_store_transfer_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_inter_store_transfer_lines_products_ProductId",
                table: "inter_store_transfer_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_loyalty_accounts_customers_CustomerId",
                table: "loyalty_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId",
                table: "loyalty_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_transactions_TransactionId",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "FK_po_lines_products_ProductId",
                table: "po_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_po_lines_purchase_orders_POId",
                table: "po_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_price_list_lines_price_lists_PriceListId",
                table: "price_list_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_price_list_lines_products_ProductId",
                table: "price_list_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_product_uoms_products_ProductId",
                table: "product_uoms");

            migrationBuilder.DropForeignKey(
                name: "FK_product_uoms_uoms_UOMId",
                table: "product_uoms");

            migrationBuilder.DropForeignKey(
                name: "FK_products_brands_BrandId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_hsn_codes_HSNId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_uoms_BaseUOMId",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_promotion_benefits_promotions_PromotionId",
                table: "promotion_benefits");

            migrationBuilder.DropForeignKey(
                name: "FK_promotion_rule_groups_promotions_PromotionId",
                table: "promotion_rule_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_promotion_rules_promotion_rule_groups_RuleGroupId",
                table: "promotion_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_stores_StoreId",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_suppliers_SupplierId",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_refunds_transactions_OriginalTransactionId",
                table: "refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_terminals_TerminalId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustment_lines_products_ProductId",
                table: "stock_adjustment_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustment_lines_stock_adjustments_AdjustmentId",
                table: "stock_adjustment_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_ledger_products_ProductId",
                table: "stock_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_ledger_stores_StoreId",
                table: "stock_ledger");

            migrationBuilder.DropForeignKey(
                name: "FK_store_configs_stores_StoreId",
                table: "store_configs");

            migrationBuilder.DropForeignKey(
                name: "FK_store_product_overrides_products_ProductId",
                table: "store_product_overrides");

            migrationBuilder.DropForeignKey(
                name: "FK_store_product_overrides_stores_StoreId",
                table: "store_product_overrides");

            migrationBuilder.DropForeignKey(
                name: "FK_stores_organizations_OrganizationId",
                table: "stores");

            migrationBuilder.DropForeignKey(
                name: "FK_tax_breakdowns_transaction_lines_TransactionLineId",
                table: "tax_breakdowns");

            migrationBuilder.DropForeignKey(
                name: "FK_tax_breakdowns_transactions_TransactionId",
                table: "tax_breakdowns");

            migrationBuilder.DropForeignKey(
                name: "FK_terminals_stores_StoreId",
                table: "terminals");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_lines_products_ProductId",
                table: "transaction_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_lines_transactions_TransactionId",
                table: "transaction_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_customers_CustomerId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_stores_StoreId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_terminals_TerminalId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_uom_conversions_uoms_FromUOMId",
                table: "uom_conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_uom_conversions_uoms_ToUOMId",
                table: "uom_conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_users_stores_StoreId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_invoices_purchase_orders_POId",
                table: "vendor_invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_vendor_invoices_suppliers_SupplierId",
                table: "vendor_invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_void_logs_transactions_TransactionId",
                table: "void_logs");

            migrationBuilder.DropIndex(
                name: "IX_transactions_StoreId_Status_CreatedAt",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_payments_TransactionId_Method_CreatedAt",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "PinSalt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "current_stock");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_StoreId",
                table: "transactions",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_TransactionId",
                table: "payments",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_barcodes_products_ProductId",
                table: "barcodes",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentCategoryId",
                table: "categories",
                column: "ParentCategoryId",
                principalTable: "categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_coupon_codes_coupon_campaigns_CampaignId",
                table: "coupon_codes",
                column: "CampaignId",
                principalTable: "coupon_campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_current_stock_products_ProductId",
                table: "current_stock",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_current_stock_stores_StoreId",
                table: "current_stock",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_purchase_orders_POId",
                table: "goods_receipts",
                column: "POId",
                principalTable: "purchase_orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_suppliers_SupplierId",
                table: "goods_receipts",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_grn_lines_goods_receipts_GRNId",
                table: "grn_lines",
                column: "GRNId",
                principalTable: "goods_receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_grn_lines_products_ProductId",
                table: "grn_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inter_store_transfer_lines_inter_store_transfers_TransferId",
                table: "inter_store_transfer_lines",
                column: "TransferId",
                principalTable: "inter_store_transfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_inter_store_transfer_lines_products_ProductId",
                table: "inter_store_transfer_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loyalty_accounts_customers_CustomerId",
                table: "loyalty_accounts",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loyalty_transactions_loyalty_accounts_LoyaltyAccountId",
                table: "loyalty_transactions",
                column: "LoyaltyAccountId",
                principalTable: "loyalty_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_transactions_TransactionId",
                table: "payments",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_po_lines_products_ProductId",
                table: "po_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_po_lines_purchase_orders_POId",
                table: "po_lines",
                column: "POId",
                principalTable: "purchase_orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_price_list_lines_price_lists_PriceListId",
                table: "price_list_lines",
                column: "PriceListId",
                principalTable: "price_lists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_price_list_lines_products_ProductId",
                table: "price_list_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_uoms_products_ProductId",
                table: "product_uoms",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_uoms_uoms_UOMId",
                table: "product_uoms",
                column: "UOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_brands_BrandId",
                table: "products",
                column: "BrandId",
                principalTable: "brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_CategoryId",
                table: "products",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_hsn_codes_HSNId",
                table: "products",
                column: "HSNId",
                principalTable: "hsn_codes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_uoms_BaseUOMId",
                table: "products",
                column: "BaseUOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_promotion_benefits_promotions_PromotionId",
                table: "promotion_benefits",
                column: "PromotionId",
                principalTable: "promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_promotion_rule_groups_promotions_PromotionId",
                table: "promotion_rule_groups",
                column: "PromotionId",
                principalTable: "promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_promotion_rules_promotion_rule_groups_RuleGroupId",
                table: "promotion_rules",
                column: "RuleGroupId",
                principalTable: "promotion_rule_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_stores_StoreId",
                table: "purchase_orders",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_suppliers_SupplierId",
                table: "purchase_orders",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refunds_transactions_OriginalTransactionId",
                table: "refunds",
                column: "OriginalTransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_terminals_TerminalId",
                table: "sessions",
                column: "TerminalId",
                principalTable: "terminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_users_UserId",
                table: "sessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustment_lines_products_ProductId",
                table: "stock_adjustment_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustment_lines_stock_adjustments_AdjustmentId",
                table: "stock_adjustment_lines",
                column: "AdjustmentId",
                principalTable: "stock_adjustments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_ledger_products_ProductId",
                table: "stock_ledger",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_ledger_stores_StoreId",
                table: "stock_ledger",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_store_configs_stores_StoreId",
                table: "store_configs",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_store_product_overrides_products_ProductId",
                table: "store_product_overrides",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_store_product_overrides_stores_StoreId",
                table: "store_product_overrides",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stores_organizations_OrganizationId",
                table: "stores",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tax_breakdowns_transaction_lines_TransactionLineId",
                table: "tax_breakdowns",
                column: "TransactionLineId",
                principalTable: "transaction_lines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tax_breakdowns_transactions_TransactionId",
                table: "tax_breakdowns",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_terminals_stores_StoreId",
                table: "terminals",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_lines_products_ProductId",
                table: "transaction_lines",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_lines_transactions_TransactionId",
                table: "transaction_lines",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_customers_CustomerId",
                table: "transactions",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_stores_StoreId",
                table: "transactions",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_terminals_TerminalId",
                table: "transactions",
                column: "TerminalId",
                principalTable: "terminals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_UserId",
                table: "transactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_uom_conversions_uoms_FromUOMId",
                table: "uom_conversions",
                column: "FromUOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_uom_conversions_uoms_ToUOMId",
                table: "uom_conversions",
                column: "ToUOMId",
                principalTable: "uoms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_stores_StoreId",
                table: "users",
                column: "StoreId",
                principalTable: "stores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_invoices_purchase_orders_POId",
                table: "vendor_invoices",
                column: "POId",
                principalTable: "purchase_orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_vendor_invoices_suppliers_SupplierId",
                table: "vendor_invoices",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_void_logs_transactions_TransactionId",
                table: "void_logs",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

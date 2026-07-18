using Microsoft.EntityFrameworkCore;
using ShoppingMall.Business.Services;
using ShoppingMall.Data.DbContext;
using ShoppingMall.Core.Enums;

namespace ShoppingMall.Server.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/sales/{storeId}/today", async (Guid storeId, ITransactionRepository txnRepo) =>
        {
            var txns = await txnRepo.GetByStoreAndDateAsync(storeId, DateTime.UtcNow);
            var completed = txns.Where(t => t.Status == Core.Enums.TransactionStatus.Completed).ToList();

            return Results.Ok(new
            {
                TransactionCount = completed.Count,
                TotalSales = completed.Sum(t => t.GrandTotal),
                TotalTax = completed.Sum(t => t.TaxTotal),
                TotalDiscount = completed.Sum(t => t.DiscountTotal),
                AvgTransactionValue = completed.Count > 0 ? completed.Average(t => t.GrandTotal) : 0,
                CashSales = completed.Where(t => t.Payments.Any(p => p.Method == Core.Enums.PaymentMethod.Cash)).Sum(t => t.GrandTotal),
                CardSales = completed.Where(t => t.Payments.Any(p => p.Method == Core.Enums.PaymentMethod.Card)).Sum(t => t.GrandTotal),
                UPISales = completed.Where(t => t.Payments.Any(p => p.Method == Core.Enums.PaymentMethod.UPI)).Sum(t => t.GrandTotal)
            });
        });

        group.MapGet("/sales/{storeId}/range", async (Guid storeId, DateTime from, DateTime to, ITransactionRepository txnRepo) =>
        {
            var txns = await txnRepo.FindAsync(t =>
                t.StoreId == storeId && t.CreatedAt >= from && t.CreatedAt <= to
                && t.Status == Core.Enums.TransactionStatus.Completed);

            var list = txns.ToList();
            return Results.Ok(new
            {
                From = from,
                To = to,
                TransactionCount = list.Count,
                TotalSales = list.Sum(t => t.GrandTotal),
                TotalTax = list.Sum(t => t.TaxTotal),
                TotalDiscount = list.Sum(t => t.DiscountTotal)
            });
        });

        group.MapGet("/sales/{storeId}/by-terminal", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db) =>
        {
            var data = await db.Transactions
                .Where(t => t.StoreId == storeId && t.Status == Core.Enums.TransactionStatus.Completed
                    && t.CreatedAt >= from && t.CreatedAt <= to)
                .GroupBy(t => t.TerminalId)
                .Select(g => new
                {
                    TerminalId = g.Key,
                    TransactionCount = g.Count(),
                    TotalSales = g.Sum(t => t.GrandTotal),
                    TotalTax = g.Sum(t => t.TaxTotal)
                })
                .ToListAsync();

            var terminals = await db.Terminals.Where(te => te.StoreId == storeId).ToDictionaryAsync(te => te.Id, te => te.Name);
            var result = data.Select(d => new
            {
                d.TerminalId,
                TerminalName = terminals.TryGetValue(d.TerminalId, out var name) ? name : "Unknown",
                d.TransactionCount,
                d.TotalSales,
                d.TotalTax
            });

            return Results.Ok(result);
        });

        group.MapGet("/sales/{storeId}/by-cashier", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db) =>
        {
            var data = await db.Transactions
                .Where(t => t.StoreId == storeId && t.Status == Core.Enums.TransactionStatus.Completed
                    && t.UserId != null && t.CreatedAt >= from && t.CreatedAt <= to)
                .GroupBy(t => t.UserId!.Value)
                .Select(g => new
                {
                    UserId = g.Key,
                    TransactionCount = g.Count(),
                    TotalSales = g.Sum(t => t.GrandTotal),
                    TotalVoids = db.Transactions.Count(tx => tx.StoreId == storeId
                        && tx.Status == Core.Enums.TransactionStatus.Voided
                        && tx.UserId == g.Key
                        && tx.CreatedAt >= from && tx.CreatedAt <= to)
                })
                .ToListAsync();

            var users = await db.Users.Where(u => u.IsActive).ToDictionaryAsync(u => u.Id, u => u.DisplayName);
            var result = data.Select(d => new
            {
                d.UserId,
                CashierName = users.TryGetValue(d.UserId, out var uname) ? uname : "Unknown",
                d.TransactionCount,
                d.TotalSales,
                d.TotalVoids
            });

            return Results.Ok(result);
        });

        group.MapGet("/products/{storeId}/top-sellers", async (Guid storeId, DateTime from, DateTime to, int count = 20, ShoppingMallDbContext db = null!) =>
        {
            var data = await db.TransactionLines
                .Where(l => l.Transaction.StoreId == storeId
                    && l.Transaction.Status == Core.Enums.TransactionStatus.Completed
                    && l.Transaction.CreatedAt >= from && l.Transaction.CreatedAt <= to)
                .GroupBy(l => new { l.ProductId, l.ProductName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    TotalQuantity = g.Sum(l => l.Quantity),
                    TotalSales = g.Sum(l => l.NetAmount),
                    TransactionCount = g.Select(l => l.TransactionId).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(count)
                .ToListAsync();

            return Results.Ok(data);
        });

        group.MapGet("/products/{storeId}/by-category", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db) =>
        {
            var data = await db.TransactionLines
                .Where(l => l.Transaction.StoreId == storeId
                    && l.Transaction.Status == Core.Enums.TransactionStatus.Completed
                    && l.Transaction.CreatedAt >= from && l.Transaction.CreatedAt <= to)
                .GroupBy(l => l.Product.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    TotalQuantity = g.Sum(l => l.Quantity),
                    TotalSales = g.Sum(l => l.NetAmount),
                    TransactionCount = g.Select(l => l.TransactionId).Distinct().Count()
                })
                .ToListAsync();

            var categories = await db.Categories.Where(c => c.IsActive).ToDictionaryAsync(c => c.Id, c => c.Name);
            var result = data.Select(d => new
            {
                d.CategoryId,
                CategoryName = categories.TryGetValue(d.CategoryId, out var cname) ? cname : "Unknown",
                d.TotalQuantity,
                d.TotalSales,
                d.TransactionCount
            }).OrderByDescending(x => x.TotalSales);

            return Results.Ok(result);
        });

        group.MapGet("/gst/{storeId}/summary", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db) =>
        {
            var data = await db.TaxBreakdowns
                .Where(tb => tb.Transaction.StoreId == storeId
                    && tb.Transaction.Status == Core.Enums.TransactionStatus.Completed
                    && tb.Transaction.CreatedAt >= from && tb.Transaction.CreatedAt <= to)
                .GroupBy(tb => new { tb.TaxType, tb.TaxRate })
                .Select(g => new
                {
                    g.Key.TaxType,
                    g.Key.TaxRate,
                    TotalTaxableAmount = g.Sum(tb => tb.TaxableAmount),
                    TotalTaxAmount = g.Sum(tb => tb.TaxAmount),
                    TransactionCount = g.Select(tb => tb.TransactionId).Distinct().Count()
                })
                .OrderBy(x => x.TaxRate)
                .ToListAsync();

            var summary = new
            {
                CGST = data.Where(d => d.TaxType == Core.Enums.TaxType.CGST).Sum(d => d.TotalTaxAmount),
                SGST = data.Where(d => d.TaxType == Core.Enums.TaxType.SGST).Sum(d => d.TotalTaxAmount),
                IGST = data.Where(d => d.TaxType == Core.Enums.TaxType.IGST).Sum(d => d.TotalTaxAmount),
                TotalGST = data.Sum(d => d.TotalTaxAmount),
                Slabs = data
            };

            return Results.Ok(summary);
        });

        group.MapGet("/stock/{storeId}/summary", async (Guid storeId, ShoppingMallDbContext db) =>
        {
            var stocks = await db.CurrentStocks
                .Include(s => s.Product)
                .Where(s => s.StoreId == storeId)
                .Select(s => new
                {
                    s.Product.Name,
                    s.Product.SKU,
                    s.OnHand,
                    s.Reserved,
                    s.Available
                })
                .ToListAsync();

            return Results.Ok(stocks);
        });

        group.MapGet("/dashboard/{storeId}", async (Guid storeId, ITransactionRepository txnRepo, ICurrentStockRepository stockRepo) =>
        {
            var todayTxns = await txnRepo.GetByStoreAndDateAsync(storeId, DateTime.UtcNow);
            var completedToday = todayTxns.Where(t => t.Status == Core.Enums.TransactionStatus.Completed);
            var lowStock = await stockRepo.GetLowStockItemsAsync(storeId);

            return Results.Ok(new
            {
                Today = new
                {
                    Sales = completedToday.Sum(t => t.GrandTotal),
                    Transactions = completedToday.Count(),
                    Tax = completedToday.Sum(t => t.TaxTotal),
                    Discounts = completedToday.Sum(t => t.DiscountTotal)
                },
                Alerts = new
                {
                    LowStockCount = lowStock.Count(),
                    LowStockItems = lowStock.Select(s => new { s.ProductId, s.Available })
                }
            });
        });

        group.MapGet("/export/sales/{storeId}/pdf", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db, PdfExportService pdf) =>
        {
            var txns = await db.Transactions
                .Where(t => t.StoreId == storeId && t.Status == TransactionStatus.Completed
                    && t.CreatedAt >= from && t.CreatedAt <= to)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            var store = await db.Stores.FindAsync(storeId);
            var users = await db.Users.Where(u => u.IsActive).ToDictionaryAsync(u => u.Id, u => u.DisplayName);
            var rows = txns.Select(t => new SalesReportRow(
                t.ReceiptNumber,
                t.UserId.HasValue && users.TryGetValue(t.UserId.Value, out var n) ? n : "Unknown",
                t.GrandTotal,
                t.CreatedAt
            ));

            var pdfBytes = pdf.GenerateSalesReport(rows, from, to, store?.Name ?? "Store");
            return Results.File(pdfBytes, "application/pdf", $"sales_report_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
        });

        group.MapGet("/export/sales/{storeId}/excel", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db, ExcelExportService excel) =>
        {
            var txns = await db.Transactions
                .Where(t => t.StoreId == storeId && t.Status == TransactionStatus.Completed
                    && t.CreatedAt >= from && t.CreatedAt <= to)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            var store = await db.Stores.FindAsync(storeId);
            var users = await db.Users.Where(u => u.IsActive).ToDictionaryAsync(u => u.Id, u => u.DisplayName);
            var rows = txns.Select(t => new SalesReportRow(
                t.ReceiptNumber,
                t.UserId.HasValue && users.TryGetValue(t.UserId.Value, out var n) ? n : "Unknown",
                t.GrandTotal,
                t.CreatedAt
            ));

            var excelBytes = excel.GenerateSalesReport(rows, from, to, store?.Name ?? "Store");
            return Results.File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"sales_report_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        });

        group.MapGet("/export/gst/{storeId}/pdf", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db, PdfExportService pdf) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            var data = await db.TaxBreakdowns
                .Where(tb => tb.Transaction.StoreId == storeId
                    && tb.Transaction.Status == TransactionStatus.Completed
                    && tb.Transaction.CreatedAt >= from && tb.Transaction.CreatedAt <= to)
                .GroupBy(tb => new { tb.TaxType, tb.TaxRate })
                .Select(g => new
                {
                    g.Key.TaxType,
                    g.Key.TaxRate,
                    TotalTaxable = g.Sum(tb => tb.TaxableAmount),
                    TotalTax = g.Sum(tb => tb.TaxAmount),
                    TxnCount = g.Select(tb => tb.TransactionId).Distinct().Count()
                })
                .ToListAsync();

            var slabs = data.Select(d => new GstReportRow(
                d.TaxType.ToString(), d.TaxRate, d.TotalTaxable, d.TotalTax, d.TxnCount
            ));
            var cgst = data.Where(d => d.TaxType == TaxType.CGST).Sum(d => d.TotalTax);
            var sgst = data.Where(d => d.TaxType == TaxType.SGST).Sum(d => d.TotalTax);
            var igst = data.Where(d => d.TaxType == TaxType.IGST).Sum(d => d.TotalTax);
            var total = cgst + sgst + igst;

            var pdfBytes = pdf.GenerateGstReport(slabs, from, to, store?.Name ?? "Store", cgst, sgst, igst, total);
            return Results.File(pdfBytes, "application/pdf", $"gst_report_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
        });

        group.MapGet("/export/gst/{storeId}/excel", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db, ExcelExportService excel) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            var data = await db.TaxBreakdowns
                .Where(tb => tb.Transaction.StoreId == storeId
                    && tb.Transaction.Status == TransactionStatus.Completed
                    && tb.Transaction.CreatedAt >= from && tb.Transaction.CreatedAt <= to)
                .GroupBy(tb => new { tb.TaxType, tb.TaxRate })
                .Select(g => new
                {
                    g.Key.TaxType,
                    g.Key.TaxRate,
                    TotalTaxable = g.Sum(tb => tb.TaxableAmount),
                    TotalTax = g.Sum(tb => tb.TaxAmount),
                    TxnCount = g.Select(tb => tb.TransactionId).Distinct().Count()
                })
                .ToListAsync();

            var slabs = data.Select(d => new GstReportRow(
                d.TaxType.ToString(), d.TaxRate, d.TotalTaxable, d.TotalTax, d.TxnCount
            ));
            var cgst = data.Where(d => d.TaxType == TaxType.CGST).Sum(d => d.TotalTax);
            var sgst = data.Where(d => d.TaxType == TaxType.SGST).Sum(d => d.TotalTax);
            var igst = data.Where(d => d.TaxType == TaxType.IGST).Sum(d => d.TotalTax);
            var total = cgst + sgst + igst;

            var excelBytes = excel.GenerateGstReport(slabs, from, to, store?.Name ?? "Store", cgst, sgst, igst, total);
            return Results.File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"gst_report_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        });

        group.MapGet("/export/products/{storeId}/pdf", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db, PdfExportService pdf) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            var data = await db.TransactionLines
                .Where(l => l.Transaction.StoreId == storeId
                    && l.Transaction.Status == TransactionStatus.Completed
                    && l.Transaction.CreatedAt >= from && l.Transaction.CreatedAt <= to)
                .GroupBy(l => l.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(l => l.Quantity),
                    Sales = g.Sum(l => l.NetAmount),
                    TxnCount = g.Select(l => l.TransactionId).Distinct().Count()
                })
                .OrderByDescending(x => x.Sales)
                .ToListAsync();

            var rows = data.Select(d => new ProductReportRow(d.ProductName, d.Quantity, d.Sales, d.TxnCount));
            var pdfBytes = pdf.GenerateProductReport(rows, from, to, store?.Name ?? "Store");
            return Results.File(pdfBytes, "application/pdf", $"product_report_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
        });

        group.MapGet("/export/products/{storeId}/excel", async (Guid storeId, DateTime from, DateTime to, ShoppingMallDbContext db, ExcelExportService excel) =>
        {
            var store = await db.Stores.FindAsync(storeId);
            var data = await db.TransactionLines
                .Where(l => l.Transaction.StoreId == storeId
                    && l.Transaction.Status == TransactionStatus.Completed
                    && l.Transaction.CreatedAt >= from && l.Transaction.CreatedAt <= to)
                .GroupBy(l => l.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(l => l.Quantity),
                    Sales = g.Sum(l => l.NetAmount),
                    TxnCount = g.Select(l => l.TransactionId).Distinct().Count()
                })
                .OrderByDescending(x => x.Sales)
                .ToListAsync();

            var rows = data.Select(d => new ProductReportRow(d.ProductName, d.Quantity, d.Sales, d.TxnCount));
            var excelBytes = excel.GenerateProductReport(rows, from, to, store?.Name ?? "Store");
            return Results.File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"product_report_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        });
    }
}

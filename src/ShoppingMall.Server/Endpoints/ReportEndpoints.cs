namespace ShoppingMall.Server.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/sales/{storeId}/today", async (Guid storeId, ITransactionRepository txnRepo) =>
        {
            var txns = await txnRepo.GetByStoreAndDateAsync(storeId, DateTime.UtcNow);
            var completed = txns.Where(t => t.Status == Core.Enums.TransactionStatus.Completed);

            return Results.Ok(new
            {
                TransactionCount = completed.Count(),
                TotalSales = completed.Sum(t => t.GrandTotal),
                TotalTax = completed.Sum(t => t.TaxTotal),
                TotalDiscount = completed.Sum(t => t.DiscountTotal),
                AvgTransactionValue = completed.Any() ? completed.Average(t => t.GrandTotal) : 0,
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

            return Results.Ok(new
            {
                From = from,
                To = to,
                TransactionCount = txns.Count(),
                TotalSales = txns.Sum(t => t.GrandTotal),
                TotalTax = txns.Sum(t => t.TaxTotal),
                TotalDiscount = txns.Sum(t => t.DiscountTotal)
            });
        });

        group.MapGet("/stock/{storeId}/summary", async (Guid storeId, ICurrentStockRepository stockRepo) =>
        {
            var stocks = await stockRepo.FindAsync(s => s.StoreId == storeId);
            return Results.Ok(new
            {
                TotalProducts = stocks.Count(),
                TotalStockValue = stocks.Sum(s => s.OnHand),
                LowStockItems = stocks.Count(s => s.Available <= 10),
                OutOfStock = stocks.Count(s => s.Available <= 0)
            });
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
    }
}

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ShoppingMall.Business.Services;

public class PdfExportService
{
    public PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateSalesReport(IEnumerable<SalesReportRow> sales, DateTime from, DateTime to, string storeName)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(c =>
                {
                    ComposeHeader(c, "Sales Report", storeName, from, to);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(60);
                        c.RelativeColumn();
                        c.ConstantColumn(80);
                        c.ConstantColumn(100);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Receipt").Bold();
                        h.Cell().Text("Cashier").Bold();
                        h.Cell().Text("Amount").Bold().AlignRight();
                        h.Cell().Text("Date").Bold().AlignRight();
                    });

                    decimal total = 0;
                    foreach (var s in sales)
                    {
                        table.Cell().Text(s.ReceiptNumber);
                        table.Cell().Text(s.CashierName);
                        table.Cell().Text(s.Amount.ToString("N2")).AlignRight();
                        table.Cell().Text(s.Date.ToString("dd-MMM-yy HH:mm")).AlignRight();
                        total += s.Amount;
                    }

                    table.Cell().ColumnSpan(2).Background(Colors.Grey.Lighten3);
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text("Total:").Bold();
                    table.Cell().Background(Colors.Grey.Lighten3).AlignRight().Text(total.ToString("N2")).Bold();
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateGstReport(IEnumerable<GstReportRow> slabs, DateTime from, DateTime to, string storeName,
        decimal cgstTotal, decimal sgstTotal, decimal igstTotal, decimal grandTotal)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(c =>
                {
                    ComposeHeader(c, "GST Report", storeName, from, to);
                });

                page.Content().Column(column =>
                {
                    column.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(r =>
                    {
                        r.RelativeItem().Text($"CGST: \u20B9{cgstTotal:N2}").Bold();
                        r.RelativeItem().Text($"SGST: \u20B9{sgstTotal:N2}").Bold();
                        r.RelativeItem().Text($"IGST: \u20B9{igstTotal:N2}").Bold();
                        r.RelativeItem().Text($"Total GST: \u20B9{grandTotal:N2}").Bold();
                    });

                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(80);
                            c.ConstantColumn(80);
                            c.ConstantColumn(120);
                            c.ConstantColumn(120);
                            c.ConstantColumn(80);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Type").Bold();
                            h.Cell().Text("Rate").Bold().AlignRight();
                            h.Cell().Text("Taxable Amount").Bold().AlignRight();
                            h.Cell().Text("Tax Amount").Bold().AlignRight();
                            h.Cell().Text("Transactions").Bold().AlignRight();
                        });

                        foreach (var s in slabs)
                        {
                            table.Cell().Text(s.TaxType);
                            table.Cell().Text($"{s.TaxRate}%").AlignRight();
                            table.Cell().Text(s.TaxableAmount.ToString("N2")).AlignRight();
                            table.Cell().Text(s.TaxAmount.ToString("N2")).AlignRight();
                            table.Cell().Text(s.TransactionCount.ToString()).AlignRight();
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    public byte[] GenerateProductReport(IEnumerable<ProductReportRow> products, DateTime from, DateTime to, string storeName)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(c =>
                {
                    ComposeHeader(c, "Product Performance Report", storeName, from, to);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.ConstantColumn(80);
                        c.ConstantColumn(100);
                        c.ConstantColumn(100);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Product").Bold();
                        h.Cell().Text("Qty Sold").Bold().AlignRight();
                        h.Cell().Text("Total Sales").Bold().AlignRight();
                        h.Cell().Text("Transactions").Bold().AlignRight();
                    });

                    foreach (var p in products)
                    {
                        table.Cell().Text(p.ProductName);
                        table.Cell().Text(p.Quantity.ToString("N0")).AlignRight();
                        table.Cell().Text(p.Sales.ToString("N2")).AlignRight();
                        table.Cell().Text(p.TransactionCount.ToString()).AlignRight();
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    private void ComposeHeader(ColumnDescriptor column, string title, string storeName, DateTime from, DateTime to)
    {
        column.Item().Text(storeName).FontSize(16).Bold();
        column.Item().Text(title).FontSize(14);
        column.Item().Text($"Period: {from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}").FontSize(10).FontColor(Colors.Grey.Darken1);
        column.Item().PaddingTop(5).PaddingBottom(10).LineHorizontal(1);
    }
}

public record SalesReportRow(string ReceiptNumber, string CashierName, decimal Amount, DateTime Date);
public record GstReportRow(string TaxType, decimal TaxRate, decimal TaxableAmount, decimal TaxAmount, int TransactionCount);
public record ProductReportRow(string ProductName, decimal Quantity, decimal Sales, int TransactionCount);

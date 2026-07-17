using ClosedXML.Excel;

namespace ShoppingMall.Business.Services;

public class ExcelExportService
{
    public byte[] GenerateSalesReport(IEnumerable<SalesReportRow> sales, DateTime from, DateTime to, string storeName)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Sales");

        ws.Cell(1, 1).Value = storeName;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Cell(2, 1).Value = $"Sales Report: {from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}";
        ws.Cell(2, 1).Style.Font.FontSize = 12;

        ws.Cell(4, 1).Value = "Receipt #";
        ws.Cell(4, 2).Value = "Cashier";
        ws.Cell(4, 3).Value = "Amount";
        ws.Cell(4, 4).Value = "Date";

        var headerRange = ws.Range(4, 1, 4, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1A56DB");
        headerRange.Style.Font.FontColor = XLColor.White;

        int row = 5;
        decimal total = 0;
        foreach (var s in sales)
        {
            ws.Cell(row, 1).Value = s.ReceiptNumber;
            ws.Cell(row, 2).Value = s.CashierName;
            ws.Cell(row, 3).Value = s.Amount;
            ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 4).Value = s.Date.ToString("dd-MMM-yyyy HH:mm");
            total += s.Amount;
            row++;
        }

        ws.Cell(row, 1).Value = "";
        ws.Cell(row, 2).Value = "";
        ws.Cell(row, 3).Value = total;
        ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(row, 3).Style.Font.Bold = true;
        ws.Cell(row, 4).Value = "TOTAL";
        ws.Cell(row, 4).Style.Font.Bold = true;

        ws.Columns(1, 4).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateGstReport(IEnumerable<GstReportRow> slabs, DateTime from, DateTime to, string storeName,
        decimal cgstTotal, decimal sgstTotal, decimal igstTotal, decimal grandTotal)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("GST");

        ws.Cell(1, 1).Value = storeName;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Cell(2, 1).Value = $"GST Report: {from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}";
        ws.Cell(2, 1).Style.Font.FontSize = 12;

        ws.Cell(4, 1).Value = "CGST";
        ws.Cell(4, 2).Value = cgstTotal;
        ws.Cell(4, 2).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(5, 1).Value = "SGST";
        ws.Cell(5, 2).Value = sgstTotal;
        ws.Cell(5, 2).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(6, 1).Value = "IGST";
        ws.Cell(6, 2).Value = igstTotal;
        ws.Cell(6, 2).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(7, 1).Value = "Total GST";
        ws.Cell(7, 1).Style.Font.Bold = true;
        ws.Cell(7, 2).Value = grandTotal;
        ws.Cell(7, 2).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(7, 2).Style.Font.Bold = true;

        ws.Cell(9, 1).Value = "Type";
        ws.Cell(9, 2).Value = "Rate";
        ws.Cell(9, 3).Value = "Taxable Amount";
        ws.Cell(9, 4).Value = "Tax Amount";
        ws.Cell(9, 5).Value = "Transactions";

        var headerRange = ws.Range(9, 1, 9, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#059669");
        headerRange.Style.Font.FontColor = XLColor.White;

        int row = 10;
        foreach (var s in slabs)
        {
            ws.Cell(row, 1).Value = s.TaxType;
            ws.Cell(row, 2).Value = s.TaxRate;
            ws.Cell(row, 2).Style.NumberFormat.Format = "0.0%";
            ws.Cell(row, 3).Value = s.TaxableAmount;
            ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 4).Value = s.TaxAmount;
            ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 5).Value = s.TransactionCount;
            row++;
        }

        ws.Columns(1, 5).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateProductReport(IEnumerable<ProductReportRow> products, DateTime from, DateTime to, string storeName)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Products");

        ws.Cell(1, 1).Value = storeName;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Cell(2, 1).Value = $"Product Performance: {from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}";
        ws.Cell(2, 1).Style.Font.FontSize = 12;

        ws.Cell(4, 1).Value = "Product";
        ws.Cell(4, 2).Value = "Qty Sold";
        ws.Cell(4, 3).Value = "Total Sales";
        ws.Cell(4, 4).Value = "Transactions";

        var headerRange = ws.Range(4, 1, 4, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#DC2626");
        headerRange.Style.Font.FontColor = XLColor.White;

        int row = 5;
        foreach (var p in products)
        {
            ws.Cell(row, 1).Value = p.ProductName;
            ws.Cell(row, 2).Value = (double)p.Quantity;
            ws.Cell(row, 3).Value = p.Sales;
            ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 4).Value = p.TransactionCount;
            row++;
        }

        ws.Columns(1, 4).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

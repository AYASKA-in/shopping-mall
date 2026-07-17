using System.IO;
using System.Text;
using ShoppingMall.Client.Printers;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Client.Services;

public class ThermalPrinterService
{
    private readonly string _printerName;

    public ThermalPrinterService()
    {
        _printerName = GetDefaultPrinter();
    }

    public string PrinterName => _printerName;

    public async Task PrintReceiptAsync(Transaction transaction, Store store, string cashierName)
    {
        var escpos = BuildReceipt(transaction, store, cashierName);
        await PrintRawAsync(escpos);
    }

    public bool TestConnection()
    {
        return RawPrinterHelper.Print(_printerName, Encoding.UTF8.GetBytes("\n\nTest Print - Shopping Mall POS\n\n"));
    }

    private byte[] BuildReceipt(Transaction txn, Store store, string cashierName)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8);

        Write(writer, EscPos.Init);
        Write(writer, EscPos.AlignCenter);
        Write(writer, EscPos.BoldOn);
        WriteLine(writer, store.Name);
        Write(writer, EscPos.BoldOff);
        WriteLine(writer, store.AddressLine1 ?? "");
        WriteLine(writer, $"GSTIN: {store.GSTIN}");
        WriteLine(writer, $"Tel: {store.Phone}");
        WriteLine(writer, new string('-', 32));

        Write(writer, EscPos.AlignLeft);
        WriteLine(writer, $"Invoice: {txn.ReceiptNumber}");
        WriteLine(writer, $"Date: {txn.CreatedAt:dd-MMM-yyyy HH:mm}");
        WriteLine(writer, $"Cashier: {cashierName}");
        if (txn.Customer != null)
            WriteLine(writer, $"Customer: {txn.Customer.FirstName} {txn.Customer.LastName}");
        WriteLine(writer, new string('-', 32));

        WriteLine(writer, $"{"Item",-20} {"Qty",4} {"Amt",8}");
        WriteLine(writer, new string('-', 32));

        foreach (var line in txn.Lines.OrderBy(l => l.LineNumber))
        {
            var name = line.ProductName.Length > 20 ? line.ProductName[..17] + "..." : line.ProductName;
            WriteLine(writer, $"{name,-20} {line.Quantity,4:F0} {line.NetAmount,8:F2}");
            if (line.TaxRate > 0)
            {
                var cgst = line.TaxAmount / 2;
                WriteLine(writer, $"  CGST @{line.TaxRate / 2:F1}%  {cgst,8:F2}");
                WriteLine(writer, $"  SGST @{line.TaxRate / 2:F1}%  {cgst,8:F2}");
            }
        }

        WriteLine(writer, new string('-', 32));
        Write(writer, EscPos.BoldOn);
        WriteLine(writer, $"{"Subtotal:",-24} {txn.SubTotal,8:F2}");
        if (txn.DiscountTotal > 0)
            WriteLine(writer, $"{"Discount:",-24} -{txn.DiscountTotal,8:F2}");
        WriteLine(writer, $"{"Tax Total:",-24} {txn.TaxTotal,8:F2}");
        WriteLine(writer, $"{"GRAND TOTAL:",-24} {txn.GrandTotal,8:F2}");
        Write(writer, EscPos.BoldOff);

        WriteLine(writer, new string('-', 32));
        foreach (var payment in txn.Payments)
        {
            WriteLine(writer, $"{payment.Method,-16} {payment.Amount,8:F2}");
            if (payment.TenderedAmount > payment.Amount)
                WriteLine(writer, $"{"Change:",-16} {payment.ChangeAmount,8:F2}");
        }

        Write(writer, EscPos.AlignCenter);
        WriteLine(writer, new string('-', 32));
        WriteLine(writer, store.ReceiptFooter ?? "Thank you! Visit again!");
        WriteLine(writer, $"Items: {txn.Lines.Sum(l => l.Quantity):F0}");
        WriteLine(writer, "");

        for (int i = 0; i < 4; i++)
            WriteLine(writer, "");

        Write(writer, EscPos.CutPaper);
        Write(writer, EscPos.OpenDrawer);

        return ms.ToArray();
    }

    private static void Write(BinaryWriter w, byte[] data) => w.Write(data);
    private static void WriteLine(BinaryWriter w, string text) => w.Write(Encoding.UTF8.GetBytes(text + "\n"));

    private Task PrintRawAsync(byte[] data)
    {
        return Task.Run(() =>
        {
            if (!RawPrinterHelper.Print(_printerName, data, "Receipt"))
            {
                System.Diagnostics.Debug.WriteLine($"Print failed on {_printerName}");
            }
        });
    }

    private static string GetDefaultPrinter()
    {
        try
        {
            using var ps = new System.Diagnostics.Process();
            ps.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-Command \"Get-CimInstance Win32_Printer | Where-Object {$_.Default -eq $true} | Select-Object -ExpandProperty Name\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            ps.Start();
            var output = ps.StandardOutput.ReadToEnd()?.Trim();
            ps.WaitForExit(3000);
            return string.IsNullOrEmpty(output) ? "EPSON TM-T88V" : output;
        }
        catch
        {
            return "EPSON TM-T88V";
        }
    }
}

internal static class EscPos
{
    public static readonly byte[] Init = [0x1B, 0x40];
    public static readonly byte[] BoldOn = [0x1B, 0x45, 0x01];
    public static readonly byte[] BoldOff = [0x1B, 0x45, 0x00];
    public static readonly byte[] AlignLeft = [0x1B, 0x61, 0x00];
    public static readonly byte[] AlignCenter = [0x1B, 0x61, 0x01];
    public static readonly byte[] AlignRight = [0x1B, 0x61, 0x02];
    public static readonly byte[] CutPaper = [0x1D, 0x56, 0x00];
    public static readonly byte[] OpenDrawer = [0x1B, 0x70, 0x00, 0x19, 0xFA];
    public static readonly byte[] QrCode = [0x1D, 0x6B, 0x04];
}

using System.Runtime.InteropServices;

namespace ShoppingMall.Client.Printers;

internal static class RawPrinterHelper
{
    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, nint pDefault);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOC_INFO_1 pDocInfo);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, byte[] pBytes, int dwCount, out int dwWritten);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DOC_INFO_1
    {
        public string DocName;
        public string OutputFile;
        public string Datatype;
    }

    public static bool Print(string printerName, byte[] data, string docName = "Receipt")
    {
        if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
            return false;

        try
        {
            var docInfo = new DOC_INFO_1
            {
                DocName = docName,
                OutputFile = null!,
                Datatype = "RAW"
            };

            if (!StartDocPrinter(hPrinter, 1, ref docInfo))
                return false;

            try
            {
                if (!StartPagePrinter(hPrinter))
                    return false;

                try
                {
                    if (!WritePrinter(hPrinter, data, data.Length, out var written))
                        return false;

                    return written == data.Length;
                }
                finally
                {
                    EndPagePrinter(hPrinter);
                }
            }
            finally
            {
                EndDocPrinter(hPrinter);
            }
        }
        finally
        {
            ClosePrinter(hPrinter);
        }
    }
}

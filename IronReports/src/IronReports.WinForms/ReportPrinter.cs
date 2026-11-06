using System.Drawing;
using System.Drawing.Printing;
using IronReports.Layout;

namespace IronReports.WinForms;

/// <summary>What was sent to the printer.</summary>
/// <param name="OutputFile">The file written, when printing to Microsoft Print to PDF.</param>
public sealed record PrintResult(string PrinterName, int PagesSent, string? OutputFile = null);

/// <summary>Prints a rendered document through the Windows print system.</summary>
public static class ReportPrinter
{
    /// <summary>Shows the print dialog, then prints. Returns null when the user cancels.</summary>
    public static PrintResult? Print(RenderedDocument document, IWin32Window? owner = null, bool showDialog = true)
    {
        using var printDoc = CreatePrintDocument(document);
        if (showDialog)
        {
            using var dialog = new PrintDialog
            {
                Document = printDoc,
                AllowSomePages = true,
                AllowCurrentPage = false,
                UseEXDialog = true
            };
            dialog.PrinterSettings.MinimumPage = 1;
            dialog.PrinterSettings.MaximumPage = document.Pages.Count;
            dialog.PrinterSettings.FromPage = 1;
            dialog.PrinterSettings.ToPage = document.Pages.Count;
            if (dialog.ShowDialog(owner) != DialogResult.OK) return null;
        }

        // Microsoft Print to PDF asks for a file name with an empty box. Ask ourselves instead,
        // with the report name filled in, and hand the path to the driver.
        string? outputFile = null;
        if (IsPdfPrinter(printDoc.PrinterSettings.PrinterName))
        {
            using var save = new SaveFileDialog
            {
                Filter = "PDF document (*.pdf)|*.pdf",
                FileName = SafeFileName(document.Title) + ".pdf",
                Title = "Print to PDF"
            };
            if (save.ShowDialog(owner) != DialogResult.OK) return null;
            outputFile = save.FileName;
            if (File.Exists(outputFile)) File.Delete(outputFile); // the user confirmed the overwrite
            printDoc.PrinterSettings.PrintToFile = true;
            printDoc.PrinterSettings.PrintFileName = outputFile;
        }

        var sent = 0;
        printDoc.PrintPage += (_, _) => sent++;
        printDoc.Print();
        return new PrintResult(printDoc.PrinterSettings.PrinterName, sent, outputFile);
    }

    public static bool IsPdfPrinter(string printerName) =>
        string.Equals(printerName, "Microsoft Print to PDF", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// The spooler writes print-to-file output after Print() returns. Waits until the file exists
    /// and is no longer locked, or the timeout passes. Returns true when the file is ready.
    /// </summary>
    public static bool WaitForFile(string path, TimeSpan timeout)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        while (watch.Elapsed < timeout)
        {
            if (File.Exists(path))
            {
                try
                {
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    if (stream.Length > 0) return true;
                }
                catch (IOException)
                {
                    // still being written
                }
            }
            Thread.Sleep(150);
        }
        return false;
    }

    private static string SafeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string((name ?? "Report").Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim();
        return cleaned.Length == 0 ? "Report" : cleaned;
    }

    /// <summary>A PrintDocument sized to the report pages. Dispose it when done.</summary>
    public static PrintDocument CreatePrintDocument(RenderedDocument document)
    {
        var printDoc = new PrintDocument { DocumentName = document.Title };
        var landscape = document.PageWidth > document.PageHeight;
        // PaperSize is in hundredths of an inch, portrait orientation.
        var portraitW = (int)Math.Round(Math.Min(document.PageWidth, document.PageHeight) * 100f / 72f);
        var portraitH = (int)Math.Round(Math.Max(document.PageWidth, document.PageHeight) * 100f / 72f);

        printDoc.DefaultPageSettings.Landscape = landscape;
        printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        printDoc.DefaultPageSettings.PaperSize = FindPaper(printDoc.PrinterSettings, portraitW, portraitH)
                                                ?? new PaperSize("Report", portraitW, portraitH);

        var index = 0;
        var last = document.Pages.Count - 1;

        printDoc.BeginPrint += (_, _) =>
        {
            var settings = printDoc.PrinterSettings;
            if (settings.PrintRange == PrintRange.SomePages)
            {
                index = Math.Clamp(settings.FromPage - 1, 0, document.Pages.Count - 1);
                last = Math.Clamp(settings.ToPage - 1, index, document.Pages.Count - 1);
            }
            else
            {
                index = 0;
                last = document.Pages.Count - 1;
            }
        };

        printDoc.PrintPage += (_, e) =>
        {
            if (document.Pages.Count == 0 || e.Graphics is null)
            {
                e.HasMorePages = false;
                return;
            }
            var g = e.Graphics;
            g.PageUnit = GraphicsUnit.Display; // 1/100 inch
            // The printer's origin is the printable area; shift back to the paper edge.
            g.TranslateTransform(-e.PageSettings.HardMarginX, -e.PageSettings.HardMarginY);
            g.ScaleTransform(100f / 72f, 100f / 72f);
            GdiRenderer.DrawPage(g, document.Pages[index], minLineWidth: 0.1f);
            index++;
            e.HasMorePages = index <= last;
        };
        return printDoc;
    }

    private static PaperSize? FindPaper(PrinterSettings settings, int width, int height)
    {
        try
        {
            foreach (PaperSize size in settings.PaperSizes)
                if (Math.Abs(size.Width - width) <= 8 && Math.Abs(size.Height - height) <= 8)
                    return size;
        }
        catch (InvalidPrinterException)
        {
        }
        return null;
    }
}

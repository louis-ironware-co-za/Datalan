using System.Drawing;
using System.Drawing.Printing;
using IronReports.Layout;

namespace IronReports.WinForms;

/// <summary>What was sent to the printer.</summary>
public sealed record PrintResult(string PrinterName, int PagesSent);

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
        var sent = 0;
        printDoc.PrintPage += (_, _) => sent++;
        printDoc.Print();
        return new PrintResult(printDoc.PrinterSettings.PrinterName, sent);
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

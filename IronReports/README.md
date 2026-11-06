# IronReports

A banded report designer and viewer for Windows, written in C# on .NET 8.
It uses no third-party components. Everything comes from the .NET base library and WinForms.

## Projects

| Project | Target | Purpose |
|---|---|---|
| `IronReports.Core` | `net8.0` | Report model, expression engine, layout engine, PDF / CSV / XLSX export, `.irpt` file format. No Windows dependency. |
| `IronReports.WinForms` | `net8.0-windows` | GDI+ renderer, `ReportViewerControl`, printing, `ReportDesignerControl`, and the ready-made `ReportDesignerForm` and `ReportViewerForm`. |
| `IronReports.Studio` | `net8.0-windows` | A stand-alone designer exe. Opens the sample report with sample data, or a `.irpt` file passed on the command line. |
| `IronReports.Core.Tests` | `net8.0` | A plain console test runner (no test framework). Writes sample PDF, CSV, XLSX and `.irpt` files. |

Open `IronReports.sln` in Visual Studio and set **IronReports.Studio** as the startup project.

```
dotnet run --project tests/IronReports.Core.Tests        # all tests, exit code 0 on success
```

## Using it from an application

```csharp
using IronReports.Data;
using IronReports.Layout;
using IronReports.Export;
using IronReports.Serialization;
using IronReports.WinForms;

var report = ReportSerializer.Load(@"C:\Reports\Invoice.irpt");
var data   = ReportDataSource.FromDataTable(table);          // or FromObjects(list), FromDictionaries(rows)
var args   = new Dictionary<string, object?> { ["CustomerId"] = 42 };

// Preview window (print and export are on its toolbar)
ReportViewerForm.ShowReport(report, data, args, owner: this);

// Headless PDF
var doc = new ReportEngine().Run(report, data, args);
PdfExporter.Export(doc, @"C:\Out\Invoice.pdf");

// Designer window, with live data for the field list and preview
using var designer = new ReportDesignerForm { DataSource = data, PreviewParameters = args };
designer.OpenFile(@"C:\Reports\Invoice.irpt");
designer.ShowDialog(this);
```

`ReportViewerControl` and `ReportDesignerControl` can also be placed on your own forms.

## Report structure

Bands, in print order: **Report Header** (page 1), **Page Header**, **Group Headers** (outer to inner),
**Detail** (once per row), **Group Footers**, **Page Footer** (bottom of every page), **Report Footer**.

Elements: **Text**, **Line**, **Shape** (rectangle or ellipse) and **Picture** (PNG or JPEG; other formats are converted on import).
All positions and sizes are in points (1 pt = 1/72 inch). The page defaults to A4 with 0.5 inch margins.

## Text and expressions

A text element holds a template. Expressions go in braces, with an optional .NET format after a colon:

```
Invoice {[InvoiceNo]}
Total: R {Sum([Quantity] * [UnitPrice]):N2}
Printed {Now():dd MMM yyyy HH:mm}
Page {PageNumber()} of {PageCount()}
{Iif(IsNullOrEmpty([Note]), '-', [Note])}
```

- Fields: `[Name]`, `[Customer.Name]` for nested objects. Parameters: `?Name`.
- Operators: `+ - * / %`, `= <> < <= > >=`, `and or not` (also `&& || !`). `+` joins text.
- Numbers are decimal, so `0.1 + 0.2 = 0.3`. Division by zero gives empty, not an error.
- Aggregates (`Sum`, `Count`, `Avg`, `Min`, `Max`, `First`, `Last`, `CountDistinct`) run over the current group in group bands
  and over the whole report in report bands. `SumAll` always covers the whole report, for percentages.
- The full function list, with descriptions, is in the expression editor (double-click a text element).

**Report → Check Expressions** (F7) validates every expression and flags unknown fields before you run the report.

## Behaviour worth knowing

- **CanGrow** on a text element lets it grow, and its band grows with it. **StretchToBand** on a shape or line
  lets it follow the band down (row stripes, vertical rules).
- **VisibleWhen** hides an element per row. **PageBreakBefore** on a band starts a new page.
- An expression error never stops a report. The element prints `#ERR` and the message.
  Filter, sort and group expressions are the exception: they stop the run with a clear message.
- Excel and CSV export write the **detail band**: one column per text element, left to right, headed by `ExportCaption`.

## Fonts and fidelity

Fonts are limited to three families: **Sans** (Arial / Helvetica), **Serif** (Times New Roman / Times) and **Mono** (Courier New / Courier).
These pairs share character widths, so line breaks are identical on screen, on paper and in the PDF.
The PDF uses the standard base fonts without embedding them, which keeps files small.
Text is limited to the Windows-1252 character set in PDF; other characters print as `?`.

## Current limits (version 1)

- A band never splits across pages. A single band taller than the page runs past the page footer.
- No sub-reports, cross-tabs, charts or barcodes yet.
- No page-scope totals in the page footer (group and report totals work).
- Expression syntax is checked, but not data types, until the report runs.

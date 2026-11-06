using System.Data;
using System.Drawing;
using IronReports.Data;
using IronReports.Model;

namespace IronReports.Samples;

/// <summary>A sample report and data set for the Studio and the tests.</summary>
public static class SampleReports
{
    public static DataTable CreateSalesTable(int rows = 120)
    {
        var table = new DataTable("Sales");
        table.Columns.Add("InvoiceNo", typeof(string));
        table.Columns.Add("InvoiceDate", typeof(DateTime));
        table.Columns.Add("Region", typeof(string));
        table.Columns.Add("Customer", typeof(string));
        table.Columns.Add("Product", typeof(string));
        table.Columns.Add("Quantity", typeof(int));
        table.Columns.Add("UnitPrice", typeof(decimal));
        table.Columns.Add("Note", typeof(string));

        string[] regions = { "Gauteng", "Western Cape", "KwaZulu-Natal" };
        string[] customers = { "Steyn Projects", "Mokoena Architects", "Coastal Fit-outs", "Van der Merwe Bou", "Naidoo Interiors", "Table Bay Joinery" };
        string[] products = { "Lever handle, satin stainless", "Mortice lock, 3-lever", "Door closer, size 3", "Butt hinge 100mm (pair)", "Pull handle 600mm", "Floor door stop" };
        decimal[] prices = { 485.00m, 362.50m, 1290.00m, 145.75m, 899.90m, 78.20m };

        var rnd = new Random(42);
        var start = new DateTime(2026, 7, 1);
        for (var i = 0; i < rows; i++)
        {
            var p = rnd.Next(products.Length);
            table.Rows.Add(
                $"INV-{10001 + i}",
                start.AddDays(rnd.Next(120)),
                regions[rnd.Next(regions.Length)],
                customers[rnd.Next(customers.Length)],
                products[p],
                rnd.Next(1, 40),
                prices[p],
                rnd.Next(5) == 0 ? "Deliver to site office before 10:00, ask for the foreman at gate 2." : (rnd.Next(3) == 0 ? DBNull.Value : ""));
        }
        return table;
    }

    public static ReportDataSource CreateSalesData(int rows = 120) => ReportDataSource.FromDataTable(CreateSalesTable(rows));

    /// <summary>Sales grouped by region, with group and grand totals, page numbers and a growing note column.</summary>
    public static ReportDefinition CreateSalesReport()
    {
        var r = new ReportDefinition { Name = "Sales by Region", Culture = "en-ZA", SortExpression = "[InvoiceDate]" };
        r.Parameters.Add(new ReportParameter { Name = "Title", Type = ParameterType.String, DefaultValue = "Sales by Region" });
        foreach (var f in CreateSalesData(1).Fields) r.Fields.Add(f);

        var width = r.Page.PrintableWidth; // 523 pt on A4 portrait with 36 pt margins
        var navy = Color.FromArgb(31, 56, 100);
        var light = Color.FromArgb(232, 238, 247);

        // Report header
        r.ReportHeader.Height = 56;
        r.ReportHeader.Elements.Add(new TextElement { Name = "title", Text = "{?Title}", X = 0, Y = 0, Width = width, Height = 26, Font = new FontSpec(FontFamilyKind.Sans, 18, bold: true), ForeColor = navy });
        r.ReportHeader.Elements.Add(new TextElement { Name = "subtitle", Text = "Printed {Now():dd MMMM yyyy HH:mm}  ·  {Count()} invoices", X = 0, Y = 28, Width = width, Height = 16, Font = new FontSpec(FontFamilyKind.Sans, 9), ForeColor = Color.DimGray });
        r.ReportHeader.Elements.Add(new LineElement { Name = "titleRule", X = 0, Y = 48, Width = width, Height = 4, LineColor = navy, LineWidth = 1.5f });

        // Page header: column captions
        r.PageHeader.Height = 22;
        r.PageHeader.BackColor = navy;
        AddCaption(r.PageHeader, "capNo", "Invoice", 0, 62);
        AddCaption(r.PageHeader, "capDate", "Date", 62, 58);
        AddCaption(r.PageHeader, "capCustomer", "Customer", 120, 100);
        AddCaption(r.PageHeader, "capProduct", "Product", 220, 128);
        AddCaption(r.PageHeader, "capQty", "Qty", 348, 30, HorizontalAlign.Right);
        AddCaption(r.PageHeader, "capAmount", "Amount", 378, 70, HorizontalAlign.Right);
        AddCaption(r.PageHeader, "capNote", "Note", 452, width - 452);

        // Group by region
        var g = r.AddGroup("[Region]");
        g.Header.Height = 24;
        g.Header.Elements.Add(new TextElement { Name = "regionName", Text = "{[Region]}", X = 0, Y = 4, Width = 300, Height = 18, Font = new FontSpec(FontFamilyKind.Sans, 11, bold: true), ForeColor = navy });
        g.Footer.Height = 24;
        g.Footer.Elements.Add(new TextElement { Name = "regionCount", Text = "{Count()} invoices in {[Region]}", X = 120, Y = 3, Width = 228, Height = 16, Font = new FontSpec(FontFamilyKind.Sans, 8, italic: true), ForeColor = Color.DimGray });
        g.Footer.Elements.Add(new TextElement { Name = "regionTotal", Text = "{Sum([Quantity] * [UnitPrice]):N2}", X = 348, Y = 3, Width = 100, Height = 16, HorizontalAlign = HorizontalAlign.Right, Font = new FontSpec(FontFamilyKind.Sans, 9, bold: true), Borders = BorderSides.Top });
        g.Footer.Elements.Add(new TextElement { Name = "regionShare", Text = "{Sum([Quantity] * [UnitPrice]) / SumAll([Quantity] * [UnitPrice]):P1} of total", X = 452, Y = 3, Width = width - 452, Height = 16, Font = new FontSpec(FontFamilyKind.Sans, 8), ForeColor = Color.DimGray });

        // Detail
        r.Detail.Height = 16;
        var f9 = new FontSpec(FontFamilyKind.Sans, 8.5f);
        r.Detail.Elements.Add(new ShapeElement { Name = "stripe", X = 0, Y = 0, Width = width, Height = 16, FillColor = light, LineWidth = 0, StretchToBand = true, VisibleWhen = "RowNumber() % 2 = 0" });
        r.Detail.Elements.Add(new TextElement { Name = "invoiceNo", ExportCaption = "Invoice", Text = "{[InvoiceNo]}", X = 0, Width = 62, Height = 16, Font = f9.Clone() });
        r.Detail.Elements.Add(new TextElement { Name = "invoiceDate", ExportCaption = "Date", Text = "{[InvoiceDate]:dd MMM yyyy}", X = 62, Width = 58, Height = 16, Font = f9.Clone() });
        r.Detail.Elements.Add(new TextElement { Name = "customer", ExportCaption = "Customer", Text = "{[Customer]}", X = 120, Width = 100, Height = 16, Font = f9.Clone() });
        r.Detail.Elements.Add(new TextElement { Name = "product", ExportCaption = "Product", Text = "{[Product]}", X = 220, Width = 128, Height = 16, Font = f9.Clone() });
        r.Detail.Elements.Add(new TextElement { Name = "quantity", ExportCaption = "Qty", Text = "{[Quantity]}", X = 348, Width = 30, Height = 16, HorizontalAlign = HorizontalAlign.Right, Font = f9.Clone() });
        r.Detail.Elements.Add(new TextElement { Name = "amount", ExportCaption = "Amount", Text = "{[Quantity] * [UnitPrice]:N2}", X = 378, Width = 70, Height = 16, HorizontalAlign = HorizontalAlign.Right, Font = f9.Clone() });
        r.Detail.Elements.Add(new TextElement { Name = "note", ExportCaption = "Note", Text = "{Iif(IsNullOrEmpty([Note]), '-', [Note])}", X = 452, Width = width - 452, Height = 16, CanGrow = true, Font = new FontSpec(FontFamilyKind.Sans, 7.5f), ForeColor = Color.DimGray });

        // Page footer
        r.PageFooter.Height = 20;
        r.PageFooter.Elements.Add(new LineElement { Name = "footRule", X = 0, Y = 0, Width = width, Height = 2, LineColor = Color.Silver, LineWidth = 0.5f });
        r.PageFooter.Elements.Add(new TextElement { Name = "footLeft", Text = "{?Title}", X = 0, Y = 4, Width = 300, Height = 14, Font = new FontSpec(FontFamilyKind.Sans, 8), ForeColor = Color.Gray });
        r.PageFooter.Elements.Add(new TextElement { Name = "pageNo", Text = "Page {PageNumber()} of {PageCount()}", X = width - 150, Y = 4, Width = 150, Height = 14, HorizontalAlign = HorizontalAlign.Right, Font = new FontSpec(FontFamilyKind.Sans, 8), ForeColor = Color.Gray });

        // Report footer: grand total
        r.ReportFooter.Height = 30;
        r.ReportFooter.Elements.Add(new TextElement { Name = "grandCaption", Text = "Grand total", X = 220, Y = 8, Width = 128, Height = 18, Font = new FontSpec(FontFamilyKind.Sans, 10, bold: true), ForeColor = navy });
        r.ReportFooter.Elements.Add(new TextElement { Name = "grandTotal", Text = "R {Sum([Quantity] * [UnitPrice]):N2}", X = 348, Y = 8, Width = 100, Height = 18, HorizontalAlign = HorizontalAlign.Right, Font = new FontSpec(FontFamilyKind.Sans, 10, bold: true), ForeColor = navy, Borders = BorderSides.Top | BorderSides.Bottom, BorderColor = navy, BorderWidth = 1 });
        return r;
    }

    private static void AddCaption(Band band, string name, string text, float x, float width, HorizontalAlign align = HorizontalAlign.Left) =>
        band.Elements.Add(new TextElement
        {
            Name = name, Text = text, X = x, Y = 0, Width = width, Height = 22, HorizontalAlign = align, VerticalAlign = VerticalAlign.Middle,
            Font = new FontSpec(FontFamilyKind.Sans, 8.5f, bold: true), ForeColor = Color.White
        });
}

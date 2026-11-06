using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using IronReports.Data;
using IronReports.Export;
using IronReports.Expressions;
using IronReports.Imaging;
using IronReports.Layout;
using IronReports.Model;
using IronReports.Samples;
using IronReports.Serialization;

// A dependency-free test runner. Exit code 0 means every test passed.
// Usage: dotnet run --project tests/IronReports.Core.Tests [outputFolder]

var outDir = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "test-output");
Directory.CreateDirectory(outDir);

var tests = new List<(string Name, Action Body)>();
void Test(string name, Action body) => tests.Add((name, body));

var inv = CultureInfo.InvariantCulture;
EvalContext Ctx(IReportRow? row = null, IReadOnlyList<IReportRow>? scope = null)
{
    var c = new EvalContext(inv) { Row = row };
    if (scope is not null) { c.ScopeRows = scope; c.AllRows = scope; }
    c.Parameters["Rate"] = 0.15m;
    return c;
}

object? Eval(string expr, IReportRow? row = null, IReadOnlyList<IReportRow>? scope = null) =>
    ReportExpression.Compile(expr).Evaluate(Ctx(row, scope));

var sample = ReportDataSource.FromDictionaries(new[]
{
    new Dictionary<string, object?> { ["Name"] = "Anna", ["Amount"] = 10.5m, ["Qty"] = 2, ["Note"] = null, ["When"] = new DateTime(2026, 1, 15) },
    new Dictionary<string, object?> { ["Name"] = "Ben", ["Amount"] = 20m, ["Qty"] = 3, ["Note"] = "", ["When"] = new DateTime(2026, 2, 1) },
    new Dictionary<string, object?> { ["Name"] = "Cara", ["Amount"] = 30m, ["Qty"] = null, ["Note"] = "x", ["When"] = new DateTime(2026, 3, 1) },
});
var first = sample.Rows[0];

// ---------- Expressions ----------
Test("arithmetic precedence", () => Equal(7m, Eval("1 + 2 * 3")));
Test("parentheses", () => Equal(9m, Eval("(1 + 2) * 3")));
Test("decimal exactness", () => Equal(0.3m, Eval("0.1 + 0.2")));
Test("unary minus", () => Equal(-5m, Eval("-(2 + 3)")));
Test("division by zero gives null", () => Equal(null, Eval("1 / 0")));
Test("string concat", () => Equal("ab1", Eval("'a' + \"b\" + 1")));
Test("escaped quote", () => Equal("it's", Eval("'it''s'")));
Test("field access", () => Equal("Anna", Eval("[Name]", first)));
Test("field case-insensitive", () => Equal("Anna", Eval("[name]", first)));
Test("missing field is null", () => Equal(null, Eval("[Nope]", first)));
Test("parameter", () => Equal(1.575m, Eval("[Amount] * ?Rate", first)));
Test("comparison", () => Equal(true, Eval("[Amount] > 10 and [Name] = 'Anna'", first)));
Test("or / not", () => Equal(true, Eval("not ([Amount] > 100) || false", first)));
Test("<> operator", () => Equal(true, Eval("[Name] <> 'Ben'", first)));
Test("null comparisons", () => { Equal(false, Eval("[Note] > 1", first)); Equal(true, Eval("[Note] = null", first)); });
Test("Iif lazy", () => Equal("ok", Eval("Iif(true, 'ok', 1/0)")));
Test("IsNullOrEmpty", () => { Equal(true, Eval("IsNullOrEmpty([Note])", first)); Equal(true, Eval("IsNullOrEmpty([Note])", sample.Rows[1])); Equal(false, Eval("IsNullOrEmpty([Note])", sample.Rows[2])); });
Test("IsNull", () => Equal("none", Eval("IsNull([Note], 'none')", first)));
Test("text functions", () =>
{
    Equal("ANNA", Eval("Upper([Name])", first));
    Equal(4, Eval("Len([Name])", first));
    Equal("nn", Eval("Substring([Name], 1, 2)", first));
    Equal("An", Eval("Left([Name], 2)", first));
    Equal("Bnna", Eval("Replace('Anna', 'A', 'B')"));
    Equal("007", Eval("PadLeft('7', 3, '0')"));
});
Test("Round half away from zero", () => { Equal(3m, Eval("Round(2.5)")); Equal(2.35m, Eval("Round(2.345, 2)")); });
Test("date functions", () =>
{
    Equal(2026, Eval("Year([When])", first));
    Equal(17L, Eval("DateDiffDays([When], ToDate('2026-02-01'))", first));
    Equal(new DateTime(2026, 1, 16), Eval("[When] + 1", first));
});
Test("ToStr with format", () => Equal("15 Jan 2026", Eval("ToStr([When], 'dd MMM yyyy')", first)));
Test("Format()", () => Equal("10.50 for Anna", Eval("Format('{0:N2} for {1}', [Amount], [Name])", first)));
Test("aggregates", () =>
{
    var rows = sample.Rows;
    Equal(60.5m, Eval("Sum([Amount])", null, rows));
    Equal(3, Eval("Count()", null, rows));
    Equal(2, Eval("Count([Qty])", null, rows));
    Equal(10.5m, Eval("Min([Amount])", null, rows));
    Equal("Cara", Eval("Max([Name])", null, rows));
    Equal("Anna", Eval("First([Name])", null, rows));
    Equal(81m, Eval("Sum([Amount] * IsNull([Qty], 0))", null, rows));
});
Test("scalar Min/Max", () => { Equal(2m, Eval("Min(2, 5)")); Equal(5m, Eval("Max(2, 5)")); });
Test("syntax errors are reported", () =>
{
    NotNull(ReportExpression.Validate("1 +"));
    NotNull(ReportExpression.Validate("Foo(1)"));
    NotNull(ReportExpression.Validate("[Name"));
    NotNull(ReportExpression.Validate("Name"));
    NotNull(ReportExpression.Validate("Iif(1, 2)"));
    Equal(null, ReportExpression.Validate("Iif([A] > 1, 'x', 'y')"));
});
Test("page info detection", () =>
{
    True(ReportExpression.Compile("PageNumber() + 1").UsesPageInfo);
    True(!ReportExpression.Compile("RowNumber()").UsesPageInfo);
});

// ---------- Templates ----------
Test("template literal", () => Equal("Hello", TextTemplate.Parse("Hello").Evaluate(Ctx())));
Test("template holes and format", () => Equal("Anna owes 10.50", TextTemplate.Parse("{[Name]} owes {[Amount]:N2}").Evaluate(Ctx(first))));
Test("template format with colons", () => Equal("15 Jan 2026 00:00", TextTemplate.Parse("{[When]:dd MMM yyyy HH:mm}").Evaluate(Ctx(first))));
Test("template braces escape", () => Equal("{x} Anna", TextTemplate.Parse("{{x}} {[Name]}").Evaluate(Ctx(first))));
Test("template default format", () => Equal("10.500", TextTemplate.Parse("{[Amount]}").Evaluate(Ctx(first), "N3")));
Test("template string with brace inside quotes", () => Equal("a}b", TextTemplate.Parse("{'a}b'}").Evaluate(Ctx())));
Test("template raw value", () => Equal(10.5m, TextTemplate.Parse("{[Amount]}").EvaluateValue(Ctx(first))));
Test("template error", () => NotNull(TextTemplate.Validate("{[Name]")));

// ---------- Text layout ----------
Test("font tables are complete", () =>
{
    foreach (var f in new[] { new FontSpec(FontFamilyKind.Sans, 10), new FontSpec(FontFamilyKind.Sans, 10, true), new FontSpec(FontFamilyKind.Serif, 10), new FontSpec(FontFamilyKind.Serif, 10, true), new FontSpec(FontFamilyKind.Serif, 10, false, true) })
    {
        var all = new string(Enumerable.Range(32, 95).Select(i => (char)i).ToArray());
        True(FontMetrics.MeasureWidth(all, f) > 0);
    }
});
Test("Helvetica widths", () => Near(22.78f, FontMetrics.MeasureWidth("Hello", new FontSpec(FontFamilyKind.Sans, 10)), 0.01f));
Test("Courier widths", () => Near(30f, FontMetrics.MeasureWidth("Hello", new FontSpec(FontFamilyKind.Mono, 10)), 0.01f));
Test("word wrap", () =>
{
    var font = new FontSpec(FontFamilyKind.Sans, 10);
    var lines = FontMetrics.WrapText("The quick brown fox jumps over the lazy dog", font, 60, true);
    True(lines.Count > 2);
    foreach (var l in lines) True(FontMetrics.MeasureWidth(l, font) <= 60.01f, $"line too wide: '{l}'");
    Equal("The quick brown fox jumps over the lazy dog", string.Join(" ", lines));
});
Test("long word breaks", () =>
{
    var font = new FontSpec(FontFamilyKind.Sans, 10);
    var lines = FontMetrics.WrapText("Supercalifragilisticexpialidocious", font, 40, true);
    True(lines.Count > 1);
    Equal("Supercalifragilisticexpialidocious", string.Concat(lines));
});
Test("explicit line breaks", () => Equal(3, FontMetrics.WrapText("a\nb\r\nc", new FontSpec(), 500, true).Count));

// ---------- Engine ----------
Test("no data prints detail once", () =>
{
    var r = new ReportDefinition();
    r.Detail.Elements.Add(new TextElement { Text = "Hello" });
    var doc = new ReportEngine().Run(r);
    Equal(1, doc.Pages.Count);
    True(doc.Pages[0].Items.OfType<TextItem>().Any(t => t.Text == "Hello"));
});
Test("pagination and page count", () =>
{
    var r = new ReportDefinition();
    r.Detail.Height = 20;
    r.Detail.Elements.Add(new TextElement { Text = "{[InvoiceNo]}" });
    r.PageFooter.Elements.Add(new TextElement { Text = "Page {PageNumber()} of {PageCount()}", Width = 200 });
    var data = SampleReports.CreateSalesData(100);
    var doc = new ReportEngine().Run(r, data);
    // body: 841.89 - 72 - 24 (footer) = 745.89; report header 40 on page 1.
    True(doc.Pages.Count >= 3, $"pages: {doc.Pages.Count}");
    var last = doc.Pages[^1];
    True(last.Items.OfType<TextItem>().Any(t => t.Text == $"Page {doc.Pages.Count} of {doc.Pages.Count}"));
    var detailCount = doc.Pages.SelectMany(p => p.Items.OfType<TextItem>()).Count(t => t.Text.StartsWith("INV-"));
    Equal(100, detailCount);
    foreach (var p in doc.Pages)
    foreach (var t in p.Items.OfType<TextItem>().Where(t => t.Text.StartsWith("INV-")))
        True(t.Baseline < doc.PageHeight - 36 - 24, "detail overlaps footer");
});
Test("groups, sorting and totals", () =>
{
    var report = SampleReports.CreateSalesReport();
    var table = SampleReports.CreateSalesTable(60);
    var doc = new ReportEngine().Run(report, ReportDataSource.FromDataTable(table));
    var texts = doc.Pages.SelectMany(p => p.Items.OfType<TextItem>()).Select(t => t.Text).ToList();
    var regionOrder = texts.Where(t => t is "Gauteng" or "KwaZulu-Natal" or "Western Cape").ToList();
    Equal("Gauteng,KwaZulu-Natal,Western Cape", string.Join(",", regionOrder));
    var expected = table.Rows.Cast<DataRow>().Sum(r => (int)r["Quantity"] * (decimal)r["UnitPrice"]);
    var culture = CultureInfo.GetCultureInfo(report.Culture);
    True(texts.Contains("R " + expected.ToString("N2", culture)), "grand total missing: R " + expected.ToString("N2", culture));
    var gauteng = table.Rows.Cast<DataRow>().Where(r => (string)r["Region"] == "Gauteng").Sum(r => (int)r["Quantity"] * (decimal)r["UnitPrice"]);
    True(texts.Contains(gauteng.ToString("N2", culture)), "group total missing");
});
Test("CanGrow grows the band", () =>
{
    var r = new ReportDefinition();
    r.ReportHeader.Visible = false; r.PageHeader.Visible = false;
    r.Detail.Height = 16;
    r.Detail.Elements.Add(new TextElement { Text = "{[T]}", Width = 60, Height = 16, CanGrow = true });
    r.Detail.Elements.Add(new TextElement { Text = "after", X = 100, Width = 60, Height = 16 });
    var data = ReportDataSource.FromDictionaries(new[]
    {
        new Dictionary<string, object?> { ["T"] = "one two three four five six seven eight nine ten" },
        new Dictionary<string, object?> { ["T"] = "x" }
    });
    var doc = new ReportEngine().Run(r, data);
    var afters = doc.Pages[0].Items.OfType<TextItem>().Where(t => t.Text == "after").ToList();
    Equal(2, afters.Count);
    True(afters[1].Baseline - afters[0].Baseline > 30, "second row should move down");
});
Test("VisibleWhen and filter", () =>
{
    var r = new ReportDefinition { FilterExpression = "[Amount] >= 20" };
    r.Detail.Elements.Add(new TextElement { Text = "{[Name]}" });
    r.Detail.Elements.Add(new TextElement { Text = "BIG", X = 120, VisibleWhen = "[Amount] > 25" });
    var doc = new ReportEngine().Run(r, sample);
    var texts = doc.Pages[0].Items.OfType<TextItem>().Select(t => t.Text).ToList();
    True(!texts.Contains("Anna") && texts.Contains("Ben") && texts.Contains("Cara"));
    Equal(1, texts.Count(t => t == "BIG"));
});
Test("expression errors render as #ERR", () =>
{
    var r = new ReportDefinition();
    r.Detail.Elements.Add(new TextElement { Text = "{Bogus(}", Width = 300 });
    var doc = new ReportEngine().Run(r);
    True(doc.Pages[0].Items.OfType<TextItem>().Any(t => t.Text.StartsWith("#ERR")));
});
Test("page break before group", () =>
{
    var r = new ReportDefinition();
    r.ReportHeader.Visible = false;
    var g = r.AddGroup("[Name]");
    g.Header.PageBreakBefore = true;
    g.Header.Elements.Add(new TextElement { Text = "{[Name]}" });
    var doc = new ReportEngine().Run(r, sample);
    Equal(3, doc.Pages.Count);
});
Test("objects as data", () =>
{
    var items = new[] { new Order("A1", new Customer("Zed"), 5m), new Order("A2", new Customer("Amy"), 7m) };
    var data = ReportDataSource.FromObjects(items);
    True(data.Fields.Any(f => f.Name == "Customer.Name"));
    var r = new ReportDefinition { SortExpression = "[Customer.Name]" };
    r.Detail.Elements.Add(new TextElement { Text = "{[Customer.Name]}: {[Total]}" });
    var doc = new ReportEngine().Run(r, data);
    var texts = doc.Pages[0].Items.OfType<TextItem>().Select(t => t.Text).ToList();
    Equal("Amy: 7,Zed: 5", string.Join(",", texts));
});

// ---------- Serialization ----------
Test("round trip", () =>
{
    var report = SampleReports.CreateSalesReport();
    report.ReportHeader.Elements.Add(new ImageElement { Name = "logo", ImageData = TinyPng(4, 3), SizeMode = ImageSizeMode.Stretch });
    var xml = ReportSerializer.SaveToString(report);
    var back = ReportSerializer.LoadFromString(xml);
    Equal(xml, ReportSerializer.SaveToString(back));
    Equal(1, back.Groups.Count);
    Equal("[Region]", back.Groups[0].Expression);
    Equal(report.Detail.Elements.Count, back.Detail.Elements.Count);
    var amount = back.Detail.Elements.OfType<TextElement>().Single(e => e.Name == "amount");
    Equal(HorizontalAlign.Right, amount.HorizontalAlign);
    Equal(8.5f, amount.Font.Size);
    Equal(Color.FromArgb(31, 56, 100).ToArgb(), back.PageHeader.BackColor.ToArgb());
    Equal(report.Parameters.Count, back.Parameters.Count);
    True(back.ReportHeader.Elements.OfType<ImageElement>().Single().ImageData!.SequenceEqual(TinyPng(4, 3)));
    File.WriteAllText(Path.Combine(outDir, "SalesByRegion.irpt"), xml);
});
Test("elements clipboard", () =>
{
    var list = ReportSerializer.LoadElements(ReportSerializer.SaveElements(new ReportElement[] { new TextElement { Name = "a", Text = "x" }, new LineElement { Name = "b" } }));
    Equal(2, list.Count);
    True(list[0] is TextElement { Text: "x" } && list[1] is LineElement);
});

// ---------- Imaging ----------
Test("PNG info and decode", () =>
{
    var png = TinyPng(5, 4);
    var info = ImageInfo.TryRead(png)!;
    Equal(5, info.PixelWidth);
    Equal(4, info.PixelHeight);
    var decoded = PngDecoder.Decode(png);
    Equal(5 * 4 * 3, decoded.Rgb.Length);
    // pixel (1,0) is red-ish per TinyPng
    Equal((byte)(1 * 50), decoded.Rgb[3]);
    NotNull(decoded.Alpha);
});

// ---------- Export ----------
Test("PDF export", () =>
{
    var report = SampleReports.CreateSalesReport();
    report.ReportHeader.Elements.Add(new ImageElement { Name = "logo", ImageData = TinyPng(40, 20), X = report.Page.PrintableWidth - 60, Y = 0, Width = 60, Height = 30, SizeMode = ImageSizeMode.Zoom });
    report.ReportHeader.Elements.Add(new ShapeElement { Name = "dot", Shape = ShapeKind.Ellipse, X = report.Page.PrintableWidth - 90, Y = 4, Width = 20, Height = 20, FillColor = Color.Orange });
    var doc = new ReportEngine().Run(report, SampleReports.CreateSalesData(120));
    var path = Path.Combine(outDir, "SalesByRegion.pdf");
    PdfExporter.Export(doc, path);
    var bytes = File.ReadAllBytes(path);
    True(Encoding.ASCII.GetString(bytes, 0, 8) == "%PDF-1.4");
    True(Encoding.ASCII.GetString(bytes[^6..]).Contains("%%EOF"));
    Console.WriteLine($"    {doc.Pages.Count} pages, {bytes.Length:N0} bytes -> {path}");
});
Test("default culture prints 23,397.40", () =>
{
    var r = new ReportDefinition();
    r.Detail.Elements.Add(new TextElement { Text = "{23397.4:N2}" });
    var doc = new ReportEngine().Run(r);
    True(doc.Pages[0].Items.OfType<TextItem>().Any(t => t.Text == "23,397.40"), "expected 23,397.40");
});
Test("CSV export", () =>
{
    var table = ExportTable.Build(SampleReports.CreateSalesReport(), SampleReports.CreateSalesData(10));
    Equal("Invoice,Date,Customer,Product,Qty,Amount,Note", string.Join(",", table.Headers));
    Equal(10, table.Rows.Count);
    var path = Path.Combine(outDir, "SalesByRegion.csv");
    CsvExporter.Export(table, path);
    var lines = File.ReadAllLines(path);
    Equal(11, lines.Length);
});
Test("XLSX export", () =>
{
    var table = ExportTable.Build(SampleReports.CreateSalesReport(), SampleReports.CreateSalesData(25));
    var path = Path.Combine(outDir, "SalesByRegion.xlsx");
    XlsxExporter.Export(table, path, "Sales by Region");
    using var zip = ZipFile.OpenRead(path);
    var sheet = new StreamReader(zip.GetEntry("xl/worksheets/sheet1.xml")!.Open()).ReadToEnd();
    True(sheet.Contains("<row r=\"26\">"));
    True(sheet.Contains("<cols>"), "column widths missing");
    System.Xml.Linq.XDocument.Parse(sheet); // well-formed
    var styles = new StreamReader(zip.GetEntry("xl/styles.xml")!.Open()).ReadToEnd();
    System.Xml.Linq.XDocument.Parse(styles);
    True(styles.Contains("formatCode=\"#,##0.00\""), "N2 format missing");
    True(styles.Contains("formatCode=\"dd mmm yyyy\""), "date format missing");
    System.Xml.Linq.XDocument.Parse(new StreamReader(zip.GetEntry("xl/workbook.xml")!.Open()).ReadToEnd());
    Equal("Date", table.Headers[1]);
    Equal("dd MMM yyyy", table.Formats[1]);
});
Test("Excel number formats", () =>
{
    Equal("#,##0.00", ExcelNumberFormat.FromDotNet("N2"));
    Equal("#,##0", ExcelNumberFormat.FromDotNet("N0"));
    Equal("0.0%", ExcelNumberFormat.FromDotNet("P1"));
    Equal("dd mmm yyyy", ExcelNumberFormat.FromDotNet("dd MMM yyyy"));
    Equal("yyyy-mm-dd hh:mm", ExcelNumberFormat.FromDotNet("yyyy-MM-dd HH:mm"));
    Equal("#,##0.00", ExcelNumberFormat.FromDotNet("#,##0.00"));
    Equal(null, ExcelNumberFormat.FromDotNet(""));
});

// ---------- Run ----------
var failed = 0;
foreach (var (name, body) in tests)
{
    try
    {
        body();
        Console.WriteLine($"  pass  {name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"  FAIL  {name}: {ex.Message}");
    }
}
Console.WriteLine();
Console.WriteLine(failed == 0 ? $"All {tests.Count} tests passed." : $"{failed} of {tests.Count} tests failed.");
return failed == 0 ? 0 : 1;

// ---------- Helpers ----------
static void Equal(object? expected, object? actual)
{
    var same = expected is null ? actual is null : actual is not null &&
        (expected.Equals(actual) || (Operators.IsNumeric(expected) && Operators.IsNumeric(actual) && expected.GetType() == actual.GetType() && expected.Equals(actual)));
    if (!same) throw new Exception($"expected <{Show(expected)}> but got <{Show(actual)}>");
}

static string Show(object? v) => v is null ? "null" : $"{v} ({v.GetType().Name})";

static void True(bool condition, string message = "condition was false")
{
    if (!condition) throw new Exception(message);
}

static void NotNull(object? v)
{
    if (v is null) throw new Exception("expected a value");
}

static void Near(float expected, float actual, float tolerance)
{
    if (Math.Abs(expected - actual) > tolerance) throw new Exception($"expected {expected} but got {actual}");
}

/// <summary>Builds an RGBA PNG with a gradient, using Paeth filters to exercise the decoder.</summary>
static byte[] TinyPng(int w, int h)
{
    using var raw = new MemoryStream();
    for (var y = 0; y < h; y++)
    {
        raw.WriteByte(0); // filter: none
        for (var x = 0; x < w; x++)
        {
            raw.WriteByte((byte)(x * 50 % 256));
            raw.WriteByte((byte)(y * 60 % 256));
            raw.WriteByte(128);
            raw.WriteByte((byte)(x == 0 ? 0 : 255));
        }
    }
    using var z = new MemoryStream();
    using (var zs = new ZLibStream(z, CompressionLevel.Optimal, true)) raw.WriteTo(zs);

    using var png = new MemoryStream();
    png.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
    var ihdr = new byte[13];
    WriteBE(ihdr, 0, w); WriteBE(ihdr, 4, h);
    ihdr[8] = 8; ihdr[9] = 6;
    Chunk(png, "IHDR", ihdr);
    Chunk(png, "IDAT", z.ToArray());
    Chunk(png, "IEND", Array.Empty<byte>());
    return png.ToArray();

    static void Chunk(Stream s, string type, byte[] data)
    {
        var len = new byte[4]; WriteBE(len, 0, data.Length); s.Write(len);
        var t = Encoding.ASCII.GetBytes(type); s.Write(t); s.Write(data);
        s.Write(new byte[4]); // CRC not checked by the decoder
    }

    static void WriteBE(byte[] b, int o, int v) { b[o] = (byte)(v >> 24); b[o + 1] = (byte)(v >> 16); b[o + 2] = (byte)(v >> 8); b[o + 3] = (byte)v; }
}

record Customer(string Name);
record Order(string Number, Customer Customer, decimal Total);

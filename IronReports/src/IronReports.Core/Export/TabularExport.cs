using System.Globalization;
using System.IO.Compression;
using System.Security;
using System.Text;
using IronReports.Data;
using IronReports.Expressions;
using IronReports.Layout;
using IronReports.Model;

namespace IronReports.Export;

/// <summary>Detail-band data as a plain table: one column per text element in the detail band, left to right.</summary>
public sealed class ExportTable
{
    public ExportTable(IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows)
    {
        Headers = headers;
        Rows = rows;
    }

    public IReadOnlyList<string> Headers { get; }
    public IReadOnlyList<object?[]> Rows { get; }

    public static ExportTable Build(ReportDefinition report, ReportDataSource? data, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        var columns = report.Detail.Elements.OfType<TextElement>()
            .OrderBy(e => e.X).ThenBy(e => e.Y).ToList();
        var headers = columns.Select(c => !string.IsNullOrWhiteSpace(c.ExportCaption) ? c.ExportCaption : c.Name).ToList();

        var ctx = ReportEngine.CreateContext(report, parameters);
        var rows = ReportEngine.PrepareRows(report, data, ctx);
        ctx.AllRows = rows;
        ctx.ScopeRows = rows;

        var output = new List<object?[]>(rows.Count);
        for (var r = 0; r < rows.Count; r++)
        {
            ctx.Row = rows[r];
            ctx.RowNumber = r + 1;
            ctx.RecordNumber = r + 1;
            var values = new object?[columns.Count];
            for (var c = 0; c < columns.Count; c++)
            {
                try { values[c] = TextTemplate.Parse(columns[c].Text).EvaluateValue(ctx, columns[c].Format); }
                catch (Exception ex) when (ex is ExpressionException or InvalidCastException or FormatException or OverflowException)
                {
                    values[c] = "#ERR " + ex.Message;
                }
            }
            output.Add(values);
        }
        return new ExportTable(headers, output);
    }
}

public static class CsvExporter
{
    /// <summary>Writes RFC 4180 CSV in UTF-8 with a BOM so Excel detects the encoding.</summary>
    public static void Export(ExportTable table, Stream output, char delimiter = ',', CultureInfo? culture = null)
    {
        culture ??= CultureInfo.InvariantCulture;
        using var w = new StreamWriter(output, new UTF8Encoding(true), leaveOpen: true);
        w.Write(string.Join(delimiter, table.Headers.Select(h => Quote(h, delimiter))));
        w.Write("\r\n");
        foreach (var row in table.Rows)
        {
            w.Write(string.Join(delimiter, row.Select(v => Quote(Format(v, culture), delimiter))));
            w.Write("\r\n");
        }
    }

    public static void Export(ExportTable table, string path, char delimiter = ',', CultureInfo? culture = null)
    {
        using var fs = File.Create(path);
        Export(table, fs, delimiter, culture);
    }

    private static string Format(object? v, CultureInfo culture) => v switch
    {
        null => "",
        DateTime d => d.TimeOfDay == TimeSpan.Zero ? d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : d.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
        IFormattable f => f.ToString(null, culture),
        _ => v.ToString() ?? ""
    };

    private static string Quote(string s, char delimiter) =>
        s.IndexOfAny(new[] { delimiter, '"', '\r', '\n' }) >= 0 || s.StartsWith(' ') || s.EndsWith(' ')
            ? "\"" + s.Replace("\"", "\"\"") + "\""
            : s;
}

/// <summary>Writes a minimal Office Open XML workbook (.xlsx) with one sheet, using only System.IO.Compression.</summary>
public static class XlsxExporter
{
    public static void Export(ExportTable table, string path, string sheetName = "Report")
    {
        using var fs = File.Create(path);
        Export(table, fs, sheetName);
    }

    public static void Export(ExportTable table, Stream output, string sheetName = "Report")
    {
        using var zip = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true);
        Add(zip, "[Content_Types].xml",
            """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
            <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
            <Default Extension="xml" ContentType="application/xml"/>
            <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
            <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
            <Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
            </Types>
            """);
        Add(zip, "_rels/.rels",
            """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
            <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
            </Relationships>
            """);
        Add(zip, "xl/workbook.xml",
            $"""
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
            <sheets><sheet name="{Xml(SafeSheetName(sheetName))}" sheetId="1" r:id="rId1"/></sheets>
            </workbook>
            """);
        Add(zip, "xl/_rels/workbook.xml.rels",
            """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
            <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
            <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
            </Relationships>
            """);
        // Styles: 0 normal, 1 bold header, 2 date, 3 date and time.
        Add(zip, "xl/styles.xml",
            """
            <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            <styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">
            <fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><name val="Calibri"/></font></fonts>
            <fills count="2"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill></fills>
            <borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders>
            <cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>
            <cellXfs count="4">
            <xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/>
            <xf numFmtId="0" fontId="1" fillId="0" borderId="0" xfId="0" applyFont="1"/>
            <xf numFmtId="14" fontId="0" fillId="0" borderId="0" xfId="0" applyNumberFormat="1"/>
            <xf numFmtId="22" fontId="0" fillId="0" borderId="0" xfId="0" applyNumberFormat="1"/>
            </cellXfs>
            </styleSheet>
            """);

        var sheet = new StringBuilder();
        sheet.Append("""<?xml version="1.0" encoding="UTF-8" standalone="yes"?>""").Append('\n');
        sheet.Append("""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>""");
        AppendRow(sheet, 1, table.Headers.Cast<object?>().ToArray(), header: true);
        for (var i = 0; i < table.Rows.Count; i++) AppendRow(sheet, i + 2, table.Rows[i], header: false);
        sheet.Append("</sheetData></worksheet>");
        Add(zip, "xl/worksheets/sheet1.xml", sheet.ToString());
    }

    private static void AppendRow(StringBuilder sb, int rowNumber, object?[] values, bool header)
    {
        sb.Append($"<row r=\"{rowNumber}\">");
        for (var c = 0; c < values.Length; c++)
        {
            var reference = ColumnName(c) + rowNumber;
            var v = values[c];
            switch (v)
            {
                case null:
                    break;
                case bool b:
                    sb.Append($"<c r=\"{reference}\" t=\"b\"><v>{(b ? 1 : 0)}</v></c>");
                    break;
                case DateTime d:
                    var serial = d.ToOADate();
                    var style = d.TimeOfDay == TimeSpan.Zero ? 2 : 3;
                    sb.Append($"<c r=\"{reference}\" s=\"{style}\"><v>{serial.ToString("R", CultureInfo.InvariantCulture)}</v></c>");
                    break;
                case byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal:
                    sb.Append($"<c r=\"{reference}\"><v>{Convert.ToString(v, CultureInfo.InvariantCulture)}</v></c>");
                    break;
                default:
                    var text = Convert.ToString(v, CultureInfo.InvariantCulture) ?? "";
                    sb.Append($"<c r=\"{reference}\" t=\"inlineStr\"{(header ? " s=\"1\"" : "")}><is><t xml:space=\"preserve\">{Xml(text)}</t></is></c>");
                    break;
            }
        }
        sb.Append("</row>");
    }

    private static string ColumnName(int index)
    {
        var name = "";
        index++;
        while (index > 0)
        {
            var m = (index - 1) % 26;
            name = (char)('A' + m) + name;
            index = (index - 1) / 26;
        }
        return name;
    }

    private static string SafeSheetName(string name)
    {
        var cleaned = new string((name ?? "").Where(ch => "[]:*?/\\".IndexOf(ch) < 0).ToArray()).Trim();
        if (cleaned.Length == 0) cleaned = "Report";
        return cleaned.Length > 31 ? cleaned[..31] : cleaned;
    }

    private static string Xml(string s)
    {
        var filtered = new StringBuilder(s.Length);
        foreach (var ch in s)
            if (ch == '\t' || ch == '\n' || ch == '\r' || ch >= 0x20 && ch != 0xFFFE && ch != 0xFFFF) filtered.Append(ch);
        return SecurityElement.Escape(filtered.ToString()) ?? "";
    }

    private static void Add(ZipArchive zip, string name, string content)
    {
        var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
        using var s = entry.Open();
        var bytes = new UTF8Encoding(false).GetBytes(content);
        s.Write(bytes, 0, bytes.Length);
    }
}

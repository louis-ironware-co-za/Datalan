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
    public ExportTable(IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows, IReadOnlyList<string?>? formats = null)
    {
        Headers = headers;
        Rows = rows;
        Formats = formats ?? headers.Select(_ => (string?)null).ToList();
    }

    public IReadOnlyList<string> Headers { get; }
    public IReadOnlyList<object?[]> Rows { get; }

    /// <summary>The .NET format of each column (for example "N2" or "dd MMM yyyy"), or null. Excel export turns it into a number format.</summary>
    public IReadOnlyList<string?> Formats { get; }

    public static ExportTable Build(ReportDefinition report, ReportDataSource? data, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        var columns = report.Detail.Elements.OfType<TextElement>()
            .OrderBy(e => e.X).ThenBy(e => e.Y).ToList();
        var headers = columns.Select(c => !string.IsNullOrWhiteSpace(c.ExportCaption) ? c.ExportCaption : c.Name).ToList();
        var formats = columns.Select(c =>
        {
            try { return TextTemplate.Parse(c.Text).SingleExpressionFormat ?? (string.IsNullOrWhiteSpace(c.Format) ? null : c.Format); }
            catch (ExpressionException) { return null; }
        }).ToList();

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
        return new ExportTable(headers, output, formats);
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
            {FilterName(table, SafeSheetName(sheetName))}
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
        // Styles: 0 normal, 1 bold header, 2 date (yyyy-mm-dd), 3 date and time, then one per column number format.
        var codes = new List<string>();
        var columnStyle = new int?[table.Headers.Count];
        for (var c = 0; c < table.Headers.Count; c++)
        {
            var code = ExcelNumberFormat.FromDotNet(c < table.Formats.Count ? table.Formats[c] : null);
            if (code is null) continue;
            var index = codes.IndexOf(code);
            if (index < 0) { codes.Add(code); index = codes.Count - 1; }
            columnStyle[c] = 4 + index;
        }
        var numFmts = new StringBuilder();
        numFmts.Append($"<numFmts count=\"{2 + codes.Count}\"><numFmt numFmtId=\"164\" formatCode=\"yyyy-mm-dd\"/><numFmt numFmtId=\"165\" formatCode=\"yyyy-mm-dd hh:mm\"/>");
        for (var i = 0; i < codes.Count; i++) numFmts.Append($"<numFmt numFmtId=\"{166 + i}\" formatCode=\"{Xml(codes[i])}\"/>");
        numFmts.Append("</numFmts>");
        var xfs = new StringBuilder();
        xfs.Append($"<cellXfs count=\"{4 + codes.Count}\">");
        xfs.Append("<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>");
        xfs.Append("<xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/>");
        xfs.Append("<xf numFmtId=\"164\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\"/>");
        xfs.Append("<xf numFmtId=\"165\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\"/>");
        for (var i = 0; i < codes.Count; i++)
            xfs.Append($"<xf numFmtId=\"{166 + i}\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyNumberFormat=\"1\"/>");
        xfs.Append("</cellXfs>");
        xfs.Append("""<cellStyles count="1"><cellStyle name="Normal" xfId="0" builtinId="0"/></cellStyles>""");

        Add(zip, "xl/styles.xml",
            """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>""" + "\n" +
            """<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">""" +
            numFmts +
            """<fonts count="2"><font><sz val="11"/><name val="Calibri"/></font><font><b/><sz val="11"/><name val="Calibri"/></font></fonts>""" +
            """<fills count="2"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill></fills>""" +
            """<borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders>""" +
            """<cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>""" +
            xfs +
            "</styleSheet>");

        var sheet = new StringBuilder();
        sheet.Append("""<?xml version="1.0" encoding="UTF-8" standalone="yes"?>""").Append('\n');
        sheet.Append("""<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">""");
        // Freeze the header row and size each column to its content.
        sheet.Append("""<sheetViews><sheetView workbookViewId="0"><pane ySplit="1" topLeftCell="A2" activePane="bottomLeft" state="frozen"/></sheetView></sheetViews>""");
        if (table.Headers.Count > 0)
        {
            sheet.Append("<cols>");
            for (var c = 0; c < table.Headers.Count; c++)
                sheet.Append($"<col min=\"{c + 1}\" max=\"{c + 1}\" width=\"{ColumnWidth(table, c).ToString("0.##", CultureInfo.InvariantCulture)}\" customWidth=\"1\"/>");
            sheet.Append("</cols>");
        }
        sheet.Append("<sheetData>");
        AppendRow(sheet, 1, table.Headers.Cast<object?>().ToArray(), header: true, columnStyle);
        for (var i = 0; i < table.Rows.Count; i++) AppendRow(sheet, i + 2, table.Rows[i], header: false, columnStyle);
        sheet.Append("</sheetData>");
        if (table.Headers.Count > 0)
            sheet.Append($"<autoFilter ref=\"A1:{ColumnName(table.Headers.Count - 1)}{table.Rows.Count + 1}\"/>");
        sheet.Append("</worksheet>");
        Add(zip, "xl/worksheets/sheet1.xml", sheet.ToString());
    }

    /// <summary>Approximate Excel column width in characters, from the header and the formatted values.</summary>
    private static double ColumnWidth(ExportTable table, int column)
    {
        var format = column < table.Formats.Count ? table.Formats[column] : null;
        var longest = table.Headers[column].Length + 3; // room for the filter button
        foreach (var row in table.Rows.Take(2000))
        {
            if (column >= row.Length || row[column] is not { } v) continue;
            var text = v switch
            {
                DateTime d when format is not null => d.ToString(format, CultureInfo.InvariantCulture),
                DateTime d => d.TimeOfDay == TimeSpan.Zero ? "yyyy-mm-dd" : "yyyy-mm-dd hh:mm",
                IFormattable f when format is not null => f.ToString(format, CultureInfo.InvariantCulture),
                _ => Convert.ToString(v, CultureInfo.InvariantCulture) ?? ""
            };
            longest = Math.Max(longest, text.Length);
        }
        return Math.Clamp(longest + 2, 6, 60);
    }

    private static void AppendRow(StringBuilder sb, int rowNumber, object?[] values, bool header, int?[] columnStyle)
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
                    var style = columnStyle[c] ?? (d.TimeOfDay == TimeSpan.Zero ? 2 : 3);
                    sb.Append($"<c r=\"{reference}\" s=\"{style}\"><v>{serial.ToString("R", CultureInfo.InvariantCulture)}</v></c>");
                    break;
                case byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal:
                    var numberStyle = columnStyle[c] is { } ns ? $" s=\"{ns}\"" : "";
                    sb.Append($"<c r=\"{reference}\"{numberStyle}><v>{Convert.ToString(v, CultureInfo.InvariantCulture)}</v></c>");
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

    private static string FilterName(ExportTable table, string sheet)
    {
        if (table.Headers.Count == 0) return "";
        var range = $"'{sheet.Replace("'", "''")}'!$A$1:${ColumnName(table.Headers.Count - 1)}${table.Rows.Count + 1}";
        return $"<definedNames><definedName name=\"_xlnm._FilterDatabase\" localSheetId=\"0\" hidden=\"1\">{Xml(range)}</definedName></definedNames>";
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

/// <summary>Translates .NET format strings into Excel number format codes.</summary>
public static class ExcelNumberFormat
{
    /// <summary>Returns an Excel format code, or null when the format has no sensible Excel equivalent.</summary>
    public static string? FromDotNet(string? format)
    {
        if (string.IsNullOrWhiteSpace(format)) return null;
        format = format.Trim();

        // Standard numeric formats: N2, F0, C2, P1, D5, E3.
        if (format.Length <= 3 && char.IsLetter(format[0]) && (format.Length == 1 || format[1..].All(char.IsDigit)))
        {
            var letter = char.ToUpperInvariant(format[0]);
            var digits = format.Length > 1 ? int.Parse(format[1..], CultureInfo.InvariantCulture) : -1;
            string Decimals(int d) => d > 0 ? "." + new string('0', Math.Min(d, 15)) : "";
            switch (letter)
            {
                case 'N': return "#,##0" + Decimals(digits < 0 ? 2 : digits);
                case 'F': return "0" + Decimals(digits < 0 ? 2 : digits);
                case 'C': return "#,##0" + Decimals(digits < 0 ? 2 : digits);
                case 'P': return "0" + Decimals(digits < 0 ? 2 : digits) + "%";
                case 'D': return digits > 0 ? new string('0', Math.Min(digits, 15)) : "0";
                case 'E': return "0" + Decimals(digits < 0 ? 6 : digits) + "E+00";
                case 'G': return null;
            }
            // Standard date formats.
            return format switch
            {
                "d" => "yyyy-mm-dd",
                "D" => "dddd, d mmmm yyyy",
                "g" => "yyyy-mm-dd hh:mm",
                "G" => "yyyy-mm-dd hh:mm:ss",
                "t" => "hh:mm",
                "T" => "hh:mm:ss",
                _ => null
            };
        }

        // Custom numeric formats such as #,##0.00 or 0.0% are the same in Excel.
        if (format.All(ch => "#0,.%;-+ ()E".IndexOf(ch) >= 0)) return format;

        // Custom date formats: map .NET tokens to Excel tokens.
        if (format.IndexOfAny(new[] { 'd', 'M', 'y', 'H', 'h', 'm', 's' }) >= 0) return DateFormat(format);
        return null;
    }

    private static string DateFormat(string f)
    {
        var sb = new StringBuilder();
        var i = 0;
        while (i < f.Length)
        {
            var ch = f[i];
            var run = 1;
            while (i + run < f.Length && f[i + run] == ch) run++;
            var token = f.Substring(i, run);
            switch (ch)
            {
                case 'y': sb.Append(run <= 2 ? "yy" : "yyyy"); break;
                case 'M': sb.Append(new string('m', Math.Min(run, 4))); break;   // month
                case 'd': sb.Append(new string('d', Math.Min(run, 4))); break;
                case 'H': sb.Append(run >= 2 ? "hh" : "h"); break;
                case 'h': sb.Append(run >= 2 ? "hh" : "h"); break;
                case 'm': sb.Append(run >= 2 ? "mm" : "m"); break;             // minute; Excel reads it as minutes after h
                case 's': sb.Append(run >= 2 ? "ss" : "s"); break;
                case 't': sb.Append("AM/PM"); break;
                case '\'':
                {
                    var end = f.IndexOf('\'', i + 1);
                    if (end < 0) end = f.Length;
                    sb.Append('"').Append(f[(i + 1)..end]).Append('"');
                    i = Math.Min(f.Length, end + 1);
                    continue;
                }
                default:
                    if (char.IsLetter(ch)) sb.Append('"').Append(token).Append('"');
                    else sb.Append(token == " " || token == "-" || token == "/" || token == ":" || token == "." || token == "," ? token : "\\" + token);
                    break;
            }
            i += run;
        }
        return sb.ToString();
    }
}

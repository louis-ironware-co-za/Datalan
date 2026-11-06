using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using IronReports.Model;

namespace IronReports.Serialization;

/// <summary>
/// Reads and writes report definitions as readable XML (.irpt).
/// Simple properties become attributes; pictures are Base64 child elements.
/// </summary>
public static class ReportSerializer
{
    public const string FileExtension = ".irpt";
    public const int FormatVersion = 1;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static readonly Dictionary<string, Type> ElementTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Text"] = typeof(TextElement),
        ["Line"] = typeof(LineElement),
        ["Shape"] = typeof(ShapeElement),
        ["Image"] = typeof(ImageElement)
    };

    // ---------- Save ----------

    public static void Save(ReportDefinition report, string path) => ToXDocument(report).Save(path);

    public static string SaveToString(ReportDefinition report) => ToXDocument(report).ToString();

    public static XDocument ToXDocument(ReportDefinition report)
    {
        var root = new XElement("IronReport", new XAttribute("Version", FormatVersion));
        WriteProperties(root, report);
        root.Add(WriteObject("Page", report.Page));

        if (report.Parameters.Count > 0)
            root.Add(new XElement("Parameters", report.Parameters.Select(p => WriteObject("Parameter", p))));
        if (report.Fields.Count > 0)
            root.Add(new XElement("Fields", report.Fields.Select(f => WriteObject("Field", f))));

        root.Add(WriteBand(report.ReportHeader));
        root.Add(WriteBand(report.PageHeader));
        foreach (var g in report.Groups)
        {
            var ge = WriteObject("Group", g);
            ge.Add(WriteBand(g.Header));
            ge.Add(WriteBand(g.Footer));
            root.Add(ge);
        }
        root.Add(WriteBand(report.Detail));
        root.Add(WriteBand(report.PageFooter));
        root.Add(WriteBand(report.ReportFooter));
        return new XDocument(new XDeclaration("1.0", "utf-8", null), root);
    }

    private static XElement WriteBand(Band band)
    {
        var e = WriteObject("Band", band);
        foreach (var el in band.Elements) e.Add(WriteObject(el.TypeName, el));
        return e;
    }

    /// <summary>Serializes elements on their own, for the designer clipboard.</summary>
    public static string SaveElements(IEnumerable<ReportElement> elements) =>
        new XElement("IronReportElements", elements.Select(e => WriteObject(e.TypeName, e))).ToString();

    public static List<ReportElement> LoadElements(string xml)
    {
        var root = XElement.Parse(xml);
        if (root.Name.LocalName != "IronReportElements") throw new FormatException("Not IronReports elements");
        return root.Elements().Select(ReadElement).Where(e => e is not null).Cast<ReportElement>().ToList();
    }

    private static XElement WriteObject(string name, object obj)
    {
        var e = new XElement(name);
        WriteProperties(e, obj);
        return e;
    }

    private static void WriteProperties(XElement target, object obj)
    {
        foreach (var p in SerializableProperties(obj.GetType()))
        {
            var value = p.GetValue(obj);
            if (value is null) continue;
            if (value is byte[] bytes)
            {
                target.Add(new XElement(p.Name, Convert.ToBase64String(bytes)));
                continue;
            }
            var text = ToText(value);
            if (text is not null) target.Add(new XAttribute(p.Name, text));
        }
    }

    private static IEnumerable<PropertyInfo> SerializableProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.GetSetMethod() is not null &&
                        p.GetIndexParameters().Length == 0 &&
                        p.GetCustomAttribute<NotSerializedAttribute>() is null)
            .OrderBy(p => p.MetadataToken);

    private static string? ToText(object value) => value switch
    {
        string s => s,
        bool b => b ? "true" : "false",
        float f => f.ToString("R", Inv),
        double d => d.ToString("R", Inv),
        decimal m => m.ToString(Inv),
        int i => i.ToString(Inv),
        Enum en => en.ToString(),
        Color c => ColorToText(c),
        FontSpec font => font.ToString(),
        _ => null
    };

    private static string ColorToText(Color c)
    {
        if (c.IsNamedColor) return c.Name;
        return $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
    }

    // ---------- Load ----------

    public static ReportDefinition Load(string path) => FromXDocument(XDocument.Load(path));

    public static ReportDefinition LoadFromString(string xml) => FromXDocument(XDocument.Parse(xml));

    public static ReportDefinition FromXDocument(XDocument doc)
    {
        var root = doc.Root ?? throw new FormatException("Empty report file");
        if (root.Name.LocalName != "IronReport") throw new FormatException("This is not an IronReports file");

        var report = new ReportDefinition();
        ReadProperties(root, report);

        if (root.Element("Page") is { } page) ReadProperties(page, report.Page);
        foreach (var p in root.Element("Parameters")?.Elements("Parameter") ?? Enumerable.Empty<XElement>())
        {
            var param = new ReportParameter();
            ReadProperties(p, param);
            report.Parameters.Add(param);
        }
        foreach (var f in root.Element("Fields")?.Elements("Field") ?? Enumerable.Empty<XElement>())
        {
            var field = new ReportField();
            ReadProperties(f, field);
            report.Fields.Add(field);
        }

        foreach (var child in root.Elements())
        {
            switch (child.Name.LocalName)
            {
                case "Band":
                    var kind = ParseEnum<BandKind>(child.Attribute("Kind")?.Value);
                    var band = kind switch
                    {
                        BandKind.ReportHeader => report.ReportHeader,
                        BandKind.PageHeader => report.PageHeader,
                        BandKind.Detail => report.Detail,
                        BandKind.PageFooter => report.PageFooter,
                        BandKind.ReportFooter => report.ReportFooter,
                        _ => null
                    };
                    if (band is not null) ReadBand(child, band);
                    break;
                case "Group":
                    var group = report.AddGroup("");
                    ReadProperties(child, group);
                    foreach (var gb in child.Elements("Band"))
                    {
                        var gk = ParseEnum<BandKind>(gb.Attribute("Kind")?.Value);
                        if (gk == BandKind.GroupHeader) ReadBand(gb, group.Header);
                        else if (gk == BandKind.GroupFooter) ReadBand(gb, group.Footer);
                    }
                    break;
            }
        }
        return report;
    }

    private static void ReadBand(XElement source, Band band)
    {
        var kind = band.Kind;
        ReadProperties(source, band);
        band.Kind = kind; // the slot decides the kind
        band.Elements.Clear();
        foreach (var child in source.Elements())
            if (ReadElement(child) is { } el) band.Elements.Add(el);
    }

    private static ReportElement? ReadElement(XElement source)
    {
        if (!ElementTypes.TryGetValue(source.Name.LocalName, out var type)) return null;
        var element = (ReportElement)Activator.CreateInstance(type)!;
        ReadProperties(source, element);
        return element;
    }

    private static void ReadProperties(XElement source, object target)
    {
        foreach (var p in SerializableProperties(target.GetType()))
        {
            if (p.PropertyType == typeof(byte[]))
            {
                var child = source.Element(p.Name);
                if (child is not null && child.Value.Length > 0)
                {
                    try { p.SetValue(target, Convert.FromBase64String(child.Value.Trim())); }
                    catch (FormatException) { }
                }
                continue;
            }
            var attr = source.Attribute(p.Name);
            if (attr is null) continue;
            if (TryParse(attr.Value, p.PropertyType, out var value)) p.SetValue(target, value);
        }
    }

    private static bool TryParse(string text, Type type, out object? value)
    {
        value = null;
        if (type == typeof(string)) { value = text; return true; }
        if (type == typeof(bool)) { value = text.Equals("true", StringComparison.OrdinalIgnoreCase) || text == "1"; return true; }
        if (type == typeof(float)) { if (float.TryParse(text, NumberStyles.Float, Inv, out var f)) { value = f; return true; } return false; }
        if (type == typeof(double)) { if (double.TryParse(text, NumberStyles.Float, Inv, out var d)) { value = d; return true; } return false; }
        if (type == typeof(decimal)) { if (decimal.TryParse(text, NumberStyles.Number, Inv, out var m)) { value = m; return true; } return false; }
        if (type == typeof(int)) { if (int.TryParse(text, NumberStyles.Integer, Inv, out var i)) { value = i; return true; } return false; }
        if (type.IsEnum) { if (Enum.TryParse(type, text, true, out var en)) { value = en; return true; } return false; }
        if (type == typeof(Color)) { value = ParseColor(text); return true; }
        if (type == typeof(FontSpec)) { value = FontSpec.Parse(text); return true; }
        return false;
    }

    private static Color ParseColor(string text)
    {
        text = text.Trim();
        if (text.StartsWith('#'))
        {
            var hex = text[1..];
            if (hex.Length == 6) hex = "FF" + hex;
            if (hex.Length == 8 && uint.TryParse(hex, NumberStyles.HexNumber, Inv, out var argb))
                return Color.FromArgb(unchecked((int)argb));
            return Color.Black;
        }
        var named = Color.FromName(text);
        return named.IsKnownColor ? named : Color.Black;
    }

    private static T ParseEnum<T>(string? text) where T : struct, Enum =>
        Enum.TryParse<T>(text, true, out var v) ? v : default;
}

using System.ComponentModel;
using System.Globalization;

namespace IronReports.Model;

/// <summary>Font description used by text elements. Sizes are in points.</summary>
[TypeConverter(typeof(FontSpecConverter))]
public sealed class FontSpec : IEquatable<FontSpec>
{
    public FontSpec() { }

    public FontSpec(FontFamilyKind family, float size, bool bold = false, bool italic = false, bool underline = false)
    {
        Family = family;
        Size = size;
        Bold = bold;
        Italic = italic;
        Underline = underline;
    }

    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint)]
    public FontFamilyKind Family { get; set; } = FontFamilyKind.Sans;

    private float _size = 10f;

    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint)]
    public float Size
    {
        get => _size;
        set => _size = Math.Clamp(value, 1f, 400f);
    }

    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint)]
    public bool Bold { get; set; }

    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint)]
    public bool Italic { get; set; }

    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.Repaint)]
    public bool Underline { get; set; }

    public FontSpec Clone() => new(Family, Size, Bold, Italic, Underline);

    public override string ToString()
    {
        var parts = new List<string>
        {
            Family.ToString(),
            Size.ToString("0.##", CultureInfo.InvariantCulture) + "pt"
        };
        if (Bold) parts.Add("Bold");
        if (Italic) parts.Add("Italic");
        if (Underline) parts.Add("Underline");
        return string.Join(", ", parts);
    }

    /// <summary>Parses the format written by <see cref="ToString"/>, for example "Sans, 10pt, Bold".</summary>
    public static FontSpec Parse(string text)
    {
        var font = new FontSpec();
        foreach (var raw in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Enum.TryParse<FontFamilyKind>(raw, true, out var family)) { font.Family = family; continue; }
            var sizeText = raw.EndsWith("pt", StringComparison.OrdinalIgnoreCase) ? raw[..^2] : raw;
            if (float.TryParse(sizeText, NumberStyles.Float, CultureInfo.InvariantCulture, out var size)) { font.Size = size; continue; }
            switch (raw.ToLowerInvariant())
            {
                case "bold": font.Bold = true; break;
                case "italic": font.Italic = true; break;
                case "underline": font.Underline = true; break;
            }
        }
        return font;
    }

    public bool Equals(FontSpec? other) =>
        other is not null && Family == other.Family && Size.Equals(other.Size) &&
        Bold == other.Bold && Italic == other.Italic && Underline == other.Underline;

    public override bool Equals(object? obj) => Equals(obj as FontSpec);

    public override int GetHashCode() => HashCode.Combine(Family, Size, Bold, Italic, Underline);
}

/// <summary>Lets the PropertyGrid show a font as one line and expand it into its parts.</summary>
public sealed class FontSpecConverter : ExpandableObjectConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
        value is string s ? FontSpec.Parse(s) : base.ConvertFrom(context, culture, value);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
        destinationType == typeof(string) && value is FontSpec f ? f.ToString() : base.ConvertTo(context, culture, value, destinationType);
}

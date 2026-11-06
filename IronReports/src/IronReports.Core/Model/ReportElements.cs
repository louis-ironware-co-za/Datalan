using System.ComponentModel;
using System.Drawing;

namespace IronReports.Model;

/// <summary>Marks a property that the XML serializer must skip.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotSerializedAttribute : Attribute { }

/// <summary>Base class for everything placed on a band. Coordinates are points relative to the band.</summary>
public abstract class ReportElement
{
    [Category("Design"), Description("Unique name of the element.")]
    public string Name { get; set; } = "";

    [Category("Layout")] public float X { get; set; }
    [Category("Layout")] public float Y { get; set; }

    private float _width = 100f;
    private float _height = 18f;

    [Category("Layout")]
    public float Width { get => _width; set => _width = Math.Max(0f, value); }

    [Category("Layout")]
    public float Height { get => _height; set => _height = Math.Max(0f, value); }

    [Category("Layout"), Description("Stretch the element down to the bottom of its band when the band grows. Useful for row stripes and vertical lines.")]
    public bool StretchToBand { get; set; }

    [Category("Behavior"), Description("Optional expression. When it evaluates to false the element is not printed. Example: [Amount] > 0")]
    [Editor("IronReports.WinForms.Designer.ExpressionEditor, IronReports.WinForms", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    public string VisibleWhen { get; set; } = "";

    [Category("Appearance"), Description("Fill colour behind the element. Transparent means no fill.")]
    public Color BackColor { get; set; } = Color.Transparent;

    [Category("Appearance")] public BorderSides Borders { get; set; } = BorderSides.None;
    [Category("Appearance")] public Color BorderColor { get; set; } = Color.Black;

    private float _borderWidth = 0.5f;

    [Category("Appearance")]
    public float BorderWidth { get => _borderWidth; set => _borderWidth = Math.Max(0f, value); }

    [Browsable(false), NotSerialized]
    public RectangleF Bounds
    {
        get => new(X, Y, Width, Height);
        set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height; }
    }

    /// <summary>Short type name used in the file format and in generated names.</summary>
    [Browsable(false), NotSerialized]
    public abstract string TypeName { get; }

    public virtual ReportElement Clone() => (ReportElement)MemberwiseClone();

    public override string ToString() => $"{Name} ({TypeName})";
}

/// <summary>
/// Text element. <see cref="Text"/> is a template: literal text with expression holes in braces.
/// Examples: "Invoice", "{[CustomerName]}", "Total: {Sum([Amount]):N2}", "Page {PageNumber()} of {PageCount()}".
/// Use {{ and }} for literal braces.
/// </summary>
public sealed class TextElement : ReportElement
{
    public override string TypeName => "Text";

    [Category("Data"), Description("Template text. Expressions go in braces, with an optional format after a colon: {[Amount]:N2}")]
    [Editor("IronReports.WinForms.Designer.TextTemplateEditor, IronReports.WinForms", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    public string Text { get; set; } = "";

    [Category("Data"), Description("Default .NET format string for expression holes without their own format. Example: N2, dd MMM yyyy")]
    public string Format { get; set; } = "";

    [Category("Data"), Description("Column heading used by CSV and Excel export. Defaults to the element name.")]
    public string ExportCaption { get; set; } = "";

    [Category("Appearance")]
    public FontSpec Font { get; set; } = new();

    [Category("Appearance")] public Color ForeColor { get; set; } = Color.Black;
    [Category("Appearance")] public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.Left;
    [Category("Appearance")] public VerticalAlign VerticalAlign { get; set; } = VerticalAlign.Top;

    private float _padding = 2f;

    [Category("Appearance")]
    public float Padding { get => _padding; set => _padding = Math.Max(0f, value); }

    [Category("Behavior")] public bool WordWrap { get; set; } = true;

    [Category("Behavior"), Description("Grow the element and its band when the text needs more height.")]
    public bool CanGrow { get; set; }

    public override ReportElement Clone()
    {
        var c = (TextElement)base.Clone();
        c.Font = Font.Clone();
        return c;
    }
}

public sealed class LineElement : ReportElement
{
    public LineElement() { Height = 4f; }

    public override string TypeName => "Line";

    [Category("Appearance")] public LineDirection Direction { get; set; } = LineDirection.Horizontal;
    [Category("Appearance")] public Color LineColor { get; set; } = Color.Black;

    private float _lineWidth = 1f;

    [Category("Appearance")]
    public float LineWidth { get => _lineWidth; set => _lineWidth = Math.Max(0f, value); }
}

public sealed class ShapeElement : ReportElement
{
    public ShapeElement() { Height = 40f; Width = 60f; }

    public override string TypeName => "Shape";

    [Category("Appearance")] public ShapeKind Shape { get; set; } = ShapeKind.Rectangle;
    [Category("Appearance")] public Color FillColor { get; set; } = Color.Transparent;
    [Category("Appearance")] public Color LineColor { get; set; } = Color.Black;

    private float _lineWidth = 1f;

    [Category("Appearance")]
    public float LineWidth { get => _lineWidth; set => _lineWidth = Math.Max(0f, value); }
}

/// <summary>Picture element. PNG and JPEG are supported. The designer converts other formats to PNG on import.</summary>
public sealed class ImageElement : ReportElement
{
    public ImageElement() { Width = 80f; Height = 60f; }

    public override string TypeName => "Image";

    [Category("Data"), Description("Embedded picture (PNG or JPEG).")]
    [Editor("IronReports.WinForms.Designer.ImageDataEditor, IronReports.WinForms", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    [TypeConverter("IronReports.WinForms.Designer.ImageDataConverter, IronReports.WinForms")]
    public byte[]? ImageData { get; set; }

    [Category("Data"), Description("Optional expression returning picture bytes (or Base64 text) per row. Overrides ImageData. Example: [Photo]")]
    [Editor("IronReports.WinForms.Designer.ExpressionEditor, IronReports.WinForms", "System.Drawing.Design.UITypeEditor, System.Drawing")]
    public string ImageExpression { get; set; } = "";

    [Category("Appearance")] public ImageSizeMode SizeMode { get; set; } = ImageSizeMode.Zoom;
}

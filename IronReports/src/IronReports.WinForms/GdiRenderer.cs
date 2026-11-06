using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using IronReports.Layout;
using IronReports.Model;

namespace IronReports.WinForms;

/// <summary>
/// Draws page items with GDI+. The caller sets the Graphics transform so that one world unit is one point.
/// Fonts map to Arial, Times New Roman and Courier New, which share metrics with the PDF base fonts.
/// </summary>
public static class GdiRenderer
{
    private static readonly Dictionary<(FontFamilyKind, float, FontStyle), Font> Fonts = new();
    private static readonly ConditionalWeakTable<byte[], Image> Images = new();
    private static readonly StringFormat TextFormat = CreateFormat();

    private static StringFormat CreateFormat()
    {
        var f = (StringFormat)StringFormat.GenericTypographic.Clone();
        f.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
        f.Trimming = StringTrimming.None;
        return f;
    }

    public static string FamilyName(FontFamilyKind kind) => kind switch
    {
        FontFamilyKind.Serif => "Times New Roman",
        FontFamilyKind.Mono => "Courier New",
        _ => "Arial"
    };

    /// <summary>A cached GDI+ font sized in world units (points under the report transform).</summary>
    public static Font GetFont(FontSpec spec)
    {
        var style = FontStyle.Regular;
        if (spec.Bold) style |= FontStyle.Bold;
        if (spec.Italic) style |= FontStyle.Italic;
        var key = (spec.Family, spec.Size, style);
        lock (Fonts)
        {
            if (!Fonts.TryGetValue(key, out var font))
            {
                font = new Font(FamilyName(spec.Family), spec.Size, style, GraphicsUnit.World);
                Fonts[key] = font;
            }
            return font;
        }
    }

    /// <summary>Draws all items of a page.</summary>
    /// <param name="minLineWidth">Thinnest line in world units, usually one device pixel.</param>
    public static void DrawPage(Graphics g, RenderedPage page, float minLineWidth = 0f) => DrawItems(g, page.Items, minLineWidth);

    public static void DrawItems(Graphics g, IEnumerable<PageItem> items, float minLineWidth = 0f)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        foreach (var item in items)
        {
            switch (item)
            {
                case RectItem r:
                    if (r.Fill.A > 0)
                    {
                        using var b = new SolidBrush(r.Fill);
                        g.FillRectangle(b, r.Bounds);
                    }
                    if (r.Stroke.A > 0 && r.StrokeWidth > 0)
                    {
                        using var p = CreatePen(r.Stroke, r.StrokeWidth, minLineWidth);
                        g.DrawRectangle(p, r.Bounds.X, r.Bounds.Y, r.Bounds.Width, r.Bounds.Height);
                    }
                    break;

                case EllipseItem e:
                    if (e.Fill.A > 0)
                    {
                        using var b = new SolidBrush(e.Fill);
                        g.FillEllipse(b, e.Bounds);
                    }
                    if (e.Stroke.A > 0 && e.StrokeWidth > 0)
                    {
                        using var p = CreatePen(e.Stroke, e.StrokeWidth, minLineWidth);
                        g.DrawEllipse(p, e.Bounds);
                    }
                    break;

                case LineItem l:
                    if (l.Width > 0 && l.Color.A > 0)
                    {
                        using var p = CreatePen(l.Color, l.Width, minLineWidth);
                        g.DrawLine(p, l.X1, l.Y1, l.X2, l.Y2);
                    }
                    break;

                case TextItem t:
                {
                    var font = GetFont(t.Font);
                    var family = font.FontFamily;
                    var ascent = font.Size * family.GetCellAscent(font.Style) / family.GetEmHeight(font.Style);
                    using var brush = new SolidBrush(t.Color);
                    g.DrawString(t.Text, font, brush, t.X, t.Baseline - ascent, TextFormat);
                    break;
                }

                case ImageItem img:
                {
                    var image = GetImage(img.Data);
                    if (image is null) break;
                    var state = g.Save();
                    g.SetClip(img.Clip, CombineMode.Intersect);
                    g.DrawImage(image, img.Destination);
                    g.Restore(state);
                    break;
                }
            }
        }
    }

    private static Pen CreatePen(Color color, float width, float minWidth) =>
        new(color, Math.Max(width, minWidth)) { Alignment = PenAlignment.Center };

    /// <summary>Decodes picture bytes once and caches the result for as long as the bytes live.</summary>
    public static Image? GetImage(byte[] data)
    {
        lock (Images)
        {
            if (Images.TryGetValue(data, out var cached)) return cached;
            try
            {
                // Copy into a Bitmap so the stream can be released.
                using var ms = new MemoryStream(data);
                using var loaded = Image.FromStream(ms);
                var bitmap = new Bitmap(loaded);
                Images.Add(data, bitmap);
                return bitmap;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}

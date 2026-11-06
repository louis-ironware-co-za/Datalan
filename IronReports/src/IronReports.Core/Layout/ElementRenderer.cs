using System.Drawing;
using IronReports.Imaging;
using IronReports.Model;

namespace IronReports.Layout;

/// <summary>Turns one element plus its resolved content into page items. Shared by the engine and the designer.</summary>
public static class ElementRenderer
{
    /// <summary>Height a text element needs for <paramref name="text"/> at its current width.</summary>
    public static float MeasureTextHeight(TextElement e, string text)
    {
        var inner = Math.Max(1f, e.Width - 2 * e.Padding);
        var lines = FontMetrics.WrapText(text, e.Font, inner, e.WordWrap);
        return lines.Count * FontMetrics.LineHeight(e.Font) + 2 * e.Padding;
    }

    public static void RenderText(TextElement e, RectangleF box, string text, List<PageItem> output)
    {
        RenderBackground(e, box, output);

        var inner = RectangleF.Inflate(box, -e.Padding, -e.Padding);
        if (inner.Width > 0 && text.Length > 0)
        {
            var font = e.Font;
            var lineHeight = FontMetrics.LineHeight(font);
            var lines = FontMetrics.WrapText(text, font, inner.Width, e.WordWrap);

            var fit = Math.Max(1, (int)Math.Floor((inner.Height + 0.01f) / lineHeight));
            if (lines.Count > fit) lines.RemoveRange(fit, lines.Count - fit);

            var blockHeight = lines.Count * lineHeight;
            var top = e.VerticalAlign switch
            {
                VerticalAlign.Middle => inner.Y + (inner.Height - blockHeight) / 2f,
                VerticalAlign.Bottom => inner.Bottom - blockHeight,
                _ => inner.Y
            };
            top = Math.Max(top, inner.Y);

            var ascent = FontMetrics.Ascent(font);
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Length == 0) continue;
                var width = FontMetrics.MeasureWidth(line, font);
                var x = e.HorizontalAlign switch
                {
                    HorizontalAlign.Center => inner.X + (inner.Width - width) / 2f,
                    HorizontalAlign.Right => inner.Right - width,
                    _ => inner.X
                };
                var baseline = top + i * lineHeight + ascent;
                output.Add(new TextItem(x, baseline, line, font, e.ForeColor));
                if (font.Underline)
                {
                    var uy = baseline + font.Size * 0.12f;
                    output.Add(new LineItem(x, uy, x + width, uy, e.ForeColor, Math.Max(0.25f, font.Size * 0.06f)));
                }
            }
        }

        RenderBorders(e, box, output);
    }

    public static void RenderLine(LineElement e, RectangleF box, List<PageItem> output)
    {
        RenderBackground(e, box, output);
        var midX = box.X + box.Width / 2f;
        var midY = box.Y + box.Height / 2f;
        var item = e.Direction switch
        {
            LineDirection.Vertical => new LineItem(midX, box.Top, midX, box.Bottom, e.LineColor, e.LineWidth),
            LineDirection.Slant => new LineItem(box.Left, box.Bottom, box.Right, box.Top, e.LineColor, e.LineWidth),
            LineDirection.BackSlant => new LineItem(box.Left, box.Top, box.Right, box.Bottom, e.LineColor, e.LineWidth),
            _ => new LineItem(box.Left, midY, box.Right, midY, e.LineColor, e.LineWidth)
        };
        if (e.LineWidth > 0) output.Add(item);
        RenderBorders(e, box, output);
    }

    public static void RenderShape(ShapeElement e, RectangleF box, List<PageItem> output)
    {
        RenderBackground(e, box, output);
        var half = e.LineWidth / 2f;
        var r = RectangleF.Inflate(box, -half, -half);
        var stroke = e.LineWidth > 0 ? e.LineColor : Color.Transparent;
        if (e.Shape == ShapeKind.Ellipse) output.Add(new EllipseItem(r, e.FillColor, stroke, e.LineWidth));
        else output.Add(new RectItem(r, e.FillColor, stroke, e.LineWidth));
        RenderBorders(e, box, output);
    }

    public static void RenderImage(ImageElement e, RectangleF box, byte[]? data, List<PageItem> output)
    {
        RenderBackground(e, box, output);
        var info = ImageInfo.TryRead(data);
        if (data is not null && info is not null)
        {
            RectangleF dest;
            switch (e.SizeMode)
            {
                case ImageSizeMode.Stretch:
                    dest = box;
                    break;
                case ImageSizeMode.Normal:
                    // Pixels at 96 DPI.
                    dest = new RectangleF(box.X, box.Y, info.PixelWidth * 72f / 96f, info.PixelHeight * 72f / 96f);
                    break;
                default:
                    var scale = Math.Min(box.Width / info.PixelWidth, box.Height / info.PixelHeight);
                    var w = info.PixelWidth * scale;
                    var h = info.PixelHeight * scale;
                    dest = new RectangleF(box.X + (box.Width - w) / 2f, box.Y + (box.Height - h) / 2f, w, h);
                    break;
            }
            output.Add(new ImageItem(dest, box, data, info));
        }
        RenderBorders(e, box, output);
    }

    private static void RenderBackground(ReportElement e, RectangleF box, List<PageItem> output)
    {
        if (e.BackColor.A > 0) output.Add(new RectItem(box, e.BackColor, Color.Transparent, 0f));
    }

    private static void RenderBorders(ReportElement e, RectangleF box, List<PageItem> output)
    {
        if (e.Borders == BorderSides.None || e.BorderWidth <= 0 || e.BorderColor.A == 0) return;
        var c = e.BorderColor;
        var w = e.BorderWidth;
        if (e.Borders == BorderSides.All)
        {
            output.Add(new RectItem(box, Color.Transparent, c, w));
            return;
        }
        if (e.Borders.HasFlag(BorderSides.Top)) output.Add(new LineItem(box.Left, box.Top, box.Right, box.Top, c, w));
        if (e.Borders.HasFlag(BorderSides.Bottom)) output.Add(new LineItem(box.Left, box.Bottom, box.Right, box.Bottom, c, w));
        if (e.Borders.HasFlag(BorderSides.Left)) output.Add(new LineItem(box.Left, box.Top, box.Left, box.Bottom, c, w));
        if (e.Borders.HasFlag(BorderSides.Right)) output.Add(new LineItem(box.Right, box.Top, box.Right, box.Bottom, c, w));
    }

    /// <summary>Design-time rendering: text shows the raw template, images show their embedded picture.</summary>
    public static List<PageItem> RenderForDesign(ReportElement element, RectangleF box)
    {
        var items = new List<PageItem>();
        switch (element)
        {
            case TextElement t: RenderText(t, box, t.Text, items); break;
            case LineElement l: RenderLine(l, box, items); break;
            case ShapeElement s: RenderShape(s, box, items); break;
            case ImageElement i: RenderImage(i, box, i.ImageData, items); break;
        }
        return items;
    }
}

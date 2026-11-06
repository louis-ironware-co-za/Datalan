using System.Drawing;
using IronReports.Imaging;
using IronReports.Model;

namespace IronReports.Layout;

/// <summary>The output of the layout engine: fixed pages of simple drawing items, in points from the page's top-left.</summary>
public sealed class RenderedDocument
{
    public RenderedDocument(float pageWidth, float pageHeight, string title)
    {
        PageWidth = pageWidth;
        PageHeight = pageHeight;
        Title = title;
    }

    public float PageWidth { get; }
    public float PageHeight { get; }
    public string Title { get; }
    public List<RenderedPage> Pages { get; } = new();
}

public sealed class RenderedPage
{
    public RenderedPage(int number) => Number = number;

    /// <summary>1-based page number.</summary>
    public int Number { get; }

    public List<PageItem> Items { get; } = new();
}

public abstract record PageItem;

/// <summary>One line of text. <see cref="Baseline"/> is the y position of the text baseline.</summary>
public sealed record TextItem(float X, float Baseline, string Text, FontSpec Font, Color Color) : PageItem;

public sealed record LineItem(float X1, float Y1, float X2, float Y2, Color Color, float Width) : PageItem;

/// <summary>A rectangle. Fill or Stroke may be empty (alpha 0) to skip that part.</summary>
public sealed record RectItem(RectangleF Bounds, Color Fill, Color Stroke, float StrokeWidth) : PageItem;

public sealed record EllipseItem(RectangleF Bounds, Color Fill, Color Stroke, float StrokeWidth) : PageItem;

/// <summary>A picture drawn into <see cref="Destination"/> and clipped to <see cref="Clip"/>.</summary>
public sealed record ImageItem(RectangleF Destination, RectangleF Clip, byte[] Data, ImageInfo Info) : PageItem;

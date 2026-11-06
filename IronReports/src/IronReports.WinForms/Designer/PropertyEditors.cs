using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using IronReports.Imaging;

namespace IronReports.WinForms.Designer;

/// <summary>PropertyGrid editor for text templates (TextElement.Text).</summary>
public sealed class TextTemplateEditor : UITypeEditor
{
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value) =>
        ExpressionEditorForm.Edit(null, value as string ?? "", isTemplate: true, "Edit Text") ?? value;
}

/// <summary>PropertyGrid editor for expressions (VisibleWhen, FilterExpression, group expressions).</summary>
public sealed class ExpressionEditor : UITypeEditor
{
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value) =>
        ExpressionEditorForm.Edit(null, value as string ?? "", isTemplate: false,
            "Edit Expression" + (context?.PropertyDescriptor is { } pd ? $" - {pd.DisplayName}" : "")) ?? value;
}

/// <summary>PropertyGrid editor that loads a picture file. Non-PNG/JPEG formats are converted to PNG.</summary>
public sealed class ImageDataEditor : UITypeEditor
{
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value) =>
        PictureLoader.Pick(null) ?? value;

    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => true;

    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e.Value is byte[] data && GdiRenderer.GetImage(data) is { } image)
            e.Graphics.DrawImage(image, e.Bounds);
    }
}

/// <summary>Shows picture data as "PNG 120 × 80" in the PropertyGrid and allows clearing it by typing "(none)".</summary>
public sealed class ImageDataConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != typeof(string)) return base.ConvertTo(context, culture, value, destinationType);
        if (value is not byte[] data) return "(none)";
        var info = ImageInfo.TryRead(data);
        return info is null ? $"({data.Length:N0} bytes)" : $"{info.Format.ToString().ToUpperInvariant()} {info.PixelWidth} × {info.PixelHeight}";
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s && (s.Length == 0 || s.Equals("(none)", StringComparison.OrdinalIgnoreCase))) return null;
        // Keep the current picture for any other text.
        return context?.PropertyDescriptor?.GetValue(context.Instance);
    }
}

public static class PictureLoader
{
    public static byte[]? Pick(IWin32Window? owner)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Pictures|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff|All files|*.*",
            Title = "Choose a picture"
        };
        if (dialog.ShowDialog(owner) != DialogResult.OK) return null;
        try
        {
            return Load(dialog.FileName);
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException or OutOfMemoryException)
        {
            MessageBox.Show(owner, $"The picture could not be loaded.\n\n{ex.Message}", "Picture", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }
    }

    /// <summary>Returns PNG or JPEG bytes. Other formats, and interlaced PNGs, are re-encoded as PNG.</summary>
    public static byte[] Load(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var info = ImageInfo.TryRead(bytes);
        if (info?.Format == ImageFormatKind.Jpeg) return bytes;
        if (info?.Format == ImageFormatKind.Png && PngDecoder.TryDecode(bytes) is not null) return bytes;

        using var ms = new MemoryStream(bytes);
        using var image = Image.FromStream(ms);
        using var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap)) g.DrawImage(image, 0, 0, image.Width, image.Height);
        using var output = new MemoryStream();
        bitmap.Save(output, ImageFormat.Png);
        return output.ToArray();
    }
}

namespace IronReports.Imaging;

public enum ImageFormatKind { Png, Jpeg }

/// <summary>Basic facts about an embedded picture, read from its header.</summary>
public sealed record ImageInfo(ImageFormatKind Format, int PixelWidth, int PixelHeight, int JpegComponents = 3)
{
    /// <summary>Reads the format and pixel size of PNG or JPEG data. Returns null for anything else.</summary>
    public static ImageInfo? TryRead(byte[]? data)
    {
        if (data is null || data.Length < 24) return null;

        // PNG: signature, then IHDR with big-endian width and height.
        if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
        {
            var w = ReadInt32BE(data, 16);
            var h = ReadInt32BE(data, 20);
            return w > 0 && h > 0 ? new ImageInfo(ImageFormatKind.Png, w, h) : null;
        }

        // JPEG: walk markers until a start-of-frame.
        if (data[0] == 0xFF && data[1] == 0xD8)
        {
            var i = 2;
            while (i + 9 < data.Length)
            {
                if (data[i] != 0xFF) { i++; continue; }
                var marker = data[i + 1];
                if (marker == 0xFF) { i++; continue; }
                if (marker is 0xD8 or 0x01 || (marker >= 0xD0 && marker <= 0xD7)) { i += 2; continue; }
                var length = (data[i + 2] << 8) | data[i + 3];
                var isSof = marker is >= 0xC0 and <= 0xCF and not 0xC4 and not 0xC8 and not 0xCC;
                if (isSof)
                {
                    var h = (data[i + 5] << 8) | data[i + 6];
                    var w = (data[i + 7] << 8) | data[i + 8];
                    var components = data[i + 9];
                    return w > 0 && h > 0 ? new ImageInfo(ImageFormatKind.Jpeg, w, h, components) : null;
                }
                i += 2 + length;
            }
        }
        return null;
    }

    private static int ReadInt32BE(byte[] d, int offset) =>
        (d[offset] << 24) | (d[offset + 1] << 16) | (d[offset + 2] << 8) | d[offset + 3];
}

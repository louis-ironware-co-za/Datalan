using System.IO.Compression;

namespace IronReports.Imaging;

/// <summary>Decoded picture: 8-bit RGB samples plus an optional 8-bit alpha plane.</summary>
public sealed record DecodedImage(int Width, int Height, byte[] Rgb, byte[]? Alpha);

/// <summary>
/// Minimal PNG decoder for PDF export. Supports non-interlaced images of every colour type
/// at bit depths 1, 2, 4, 8 and 16 (16-bit samples are reduced to 8-bit).
/// </summary>
public static class PngDecoder
{
    public static DecodedImage? TryDecode(byte[] data)
    {
        try { return Decode(data); }
        catch (Exception ex) when (ex is InvalidDataException or IndexOutOfRangeException or ArgumentException or NotSupportedException)
        {
            return null;
        }
    }

    public static DecodedImage Decode(byte[] data)
    {
        if (data.Length < 8 || data[0] != 0x89 || data[1] != 0x50) throw new InvalidDataException("Not a PNG file");

        int width = 0, height = 0, bitDepth = 0, colorType = 0, interlace = 0;
        byte[]? palette = null;
        byte[]? transparency = null;
        using var idat = new MemoryStream();

        var pos = 8;
        while (pos + 8 <= data.Length)
        {
            var length = (data[pos] << 24) | (data[pos + 1] << 16) | (data[pos + 2] << 8) | data[pos + 3];
            var type = System.Text.Encoding.ASCII.GetString(data, pos + 4, 4);
            var body = pos + 8;
            if (length < 0 || body + length > data.Length) throw new InvalidDataException("Truncated PNG chunk");
            switch (type)
            {
                case "IHDR":
                    width = ReadInt(data, body);
                    height = ReadInt(data, body + 4);
                    bitDepth = data[body + 8];
                    colorType = data[body + 9];
                    interlace = data[body + 12];
                    break;
                case "PLTE":
                    palette = data[body..(body + length)];
                    break;
                case "tRNS":
                    transparency = data[body..(body + length)];
                    break;
                case "IDAT":
                    idat.Write(data, body, length);
                    break;
                case "IEND":
                    pos = data.Length;
                    continue;
            }
            pos = body + length + 4; // skip CRC
        }

        if (width <= 0 || height <= 0) throw new InvalidDataException("Missing PNG header");
        if (interlace != 0) throw new NotSupportedException("Interlaced PNG is not supported");

        var channels = colorType switch { 0 => 1, 2 => 3, 3 => 1, 4 => 2, 6 => 4, _ => throw new NotSupportedException("Unknown PNG colour type") };
        var bitsPerPixel = channels * bitDepth;
        var stride = (width * bitsPerPixel + 7) / 8;
        var bpp = Math.Max(1, bitsPerPixel / 8);

        idat.Position = 0;
        var raw = new byte[(stride + 1) * height];
        using (var z = new ZLibStream(idat, CompressionMode.Decompress))
        {
            var read = 0;
            while (read < raw.Length)
            {
                var n = z.Read(raw, read, raw.Length - read);
                if (n == 0) throw new InvalidDataException("PNG image data is truncated");
                read += n;
            }
        }

        // Undo the per-row filters in place.
        var pixels = new byte[stride * height];
        var prior = new byte[stride];
        for (var y = 0; y < height; y++)
        {
            var filter = raw[y * (stride + 1)];
            var src = y * (stride + 1) + 1;
            var row = pixels.AsSpan(y * stride, stride);
            raw.AsSpan(src, stride).CopyTo(row);
            for (var x = 0; x < stride; x++)
            {
                int a = x >= bpp ? row[x - bpp] : 0;
                int b = prior[x];
                int c = x >= bpp ? prior[x - bpp] : 0;
                row[x] = filter switch
                {
                    0 => row[x],
                    1 => (byte)(row[x] + a),
                    2 => (byte)(row[x] + b),
                    3 => (byte)(row[x] + ((a + b) >> 1)),
                    4 => (byte)(row[x] + Paeth(a, b, c)),
                    _ => throw new InvalidDataException("Unknown PNG filter")
                };
            }
            row.CopyTo(prior);
        }

        var rgb = new byte[width * height * 3];
        byte[]? alpha = colorType is 4 or 6 || transparency is not null ? new byte[width * height] : null;
        var hasRealAlpha = false;

        for (var y = 0; y < height; y++)
        {
            var row = pixels.AsSpan(y * stride, stride);
            for (var x = 0; x < width; x++)
            {
                byte r, g, bl, al = 255;
                switch (colorType)
                {
                    case 0:
                    {
                        var v = Sample(row, x, 0, 1, bitDepth);
                        var gray = Scale(v, bitDepth);
                        r = g = bl = gray;
                        if (transparency is { Length: >= 2 } && v == ((transparency[0] << 8) | transparency[1])) al = 0;
                        break;
                    }
                    case 2:
                    {
                        var rv = Sample(row, x, 0, 3, bitDepth);
                        var gv = Sample(row, x, 1, 3, bitDepth);
                        var bv = Sample(row, x, 2, 3, bitDepth);
                        r = Scale(rv, bitDepth); g = Scale(gv, bitDepth); bl = Scale(bv, bitDepth);
                        if (transparency is { Length: >= 6 } &&
                            rv == ((transparency[0] << 8) | transparency[1]) &&
                            gv == ((transparency[2] << 8) | transparency[3]) &&
                            bv == ((transparency[4] << 8) | transparency[5])) al = 0;
                        break;
                    }
                    case 3:
                    {
                        var index = Sample(row, x, 0, 1, bitDepth);
                        if (palette is null || index * 3 + 2 >= palette.Length) { r = g = bl = 0; }
                        else { r = palette[index * 3]; g = palette[index * 3 + 1]; bl = palette[index * 3 + 2]; }
                        if (transparency is not null && index < transparency.Length) al = transparency[index];
                        break;
                    }
                    case 4:
                        r = g = bl = Scale(Sample(row, x, 0, 2, bitDepth), bitDepth);
                        al = Scale(Sample(row, x, 1, 2, bitDepth), bitDepth);
                        break;
                    default:
                        r = Scale(Sample(row, x, 0, 4, bitDepth), bitDepth);
                        g = Scale(Sample(row, x, 1, 4, bitDepth), bitDepth);
                        bl = Scale(Sample(row, x, 2, 4, bitDepth), bitDepth);
                        al = Scale(Sample(row, x, 3, 4, bitDepth), bitDepth);
                        break;
                }
                var p = y * width + x;
                rgb[p * 3] = r;
                rgb[p * 3 + 1] = g;
                rgb[p * 3 + 2] = bl;
                if (alpha is not null)
                {
                    alpha[p] = al;
                    if (al != 255) hasRealAlpha = true;
                }
            }
        }

        return new DecodedImage(width, height, rgb, hasRealAlpha ? alpha : null);
    }

    private static int Sample(ReadOnlySpan<byte> row, int x, int channel, int channels, int bitDepth)
    {
        switch (bitDepth)
        {
            case 8: return row[x * channels + channel];
            case 16:
            {
                var i = (x * channels + channel) * 2;
                return (row[i] << 8) | row[i + 1];
            }
            default:
            {
                // Sub-byte depths only occur with one channel.
                var bitIndex = x * bitDepth;
                var b = row[bitIndex >> 3];
                var shift = 8 - bitDepth - (bitIndex & 7);
                return (b >> shift) & ((1 << bitDepth) - 1);
            }
        }
    }

    private static byte Scale(int value, int bitDepth) => bitDepth switch
    {
        8 => (byte)value,
        16 => (byte)(value >> 8),
        _ => (byte)(value * 255 / ((1 << bitDepth) - 1))
    };

    private static int Paeth(int a, int b, int c)
    {
        var p = a + b - c;
        var pa = Math.Abs(p - a);
        var pb = Math.Abs(p - b);
        var pc = Math.Abs(p - c);
        return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
    }

    private static int ReadInt(byte[] d, int o) => (d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3];
}

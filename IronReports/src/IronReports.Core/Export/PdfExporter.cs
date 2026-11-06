using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using IronReports.Imaging;
using IronReports.Layout;
using IronReports.Model;

namespace IronReports.Export;

/// <summary>
/// Writes a rendered document as PDF 1.4 using the standard base fonts (no embedding) and WinAnsi text.
/// PNG and JPEG pictures are embedded; PNG transparency becomes a soft mask.
/// </summary>
public static class PdfExporter
{
    public static void Export(RenderedDocument document, string path)
    {
        using var fs = File.Create(path);
        Export(document, fs);
    }

    public static byte[] ExportToBytes(RenderedDocument document)
    {
        using var ms = new MemoryStream();
        Export(document, ms);
        return ms.ToArray();
    }

    public static void Export(RenderedDocument document, Stream output)
    {
        ArgumentNullException.ThrowIfNull(document);
        new Writer(document, output).Write();
    }

    private sealed class Writer
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        private readonly RenderedDocument _doc;
        private readonly Stream _out;
        private readonly List<long> _offsets = new() { 0 };
        private readonly Dictionary<string, (int Obj, string Res)> _fonts = new();
        private readonly Dictionary<byte[], (int Obj, string Res)?> _images = new(ReferenceEqualityComparer.Instance);
        private long _position;

        public Writer(RenderedDocument doc, Stream output)
        {
            _doc = doc;
            _out = output;
        }

        public void Write()
        {
            WriteRaw("%PDF-1.4\n%âãÏÓ\n");

            // Reserve: 1 = catalog, 2 = pages, 3 = info.
            const int catalogId = 1, pagesId = 2, infoId = 3;
            var nextId = 4;
            var pageIds = new List<int>();

            // Pages are written after their resources are known; give each its ids now.
            var pageObjects = new List<(int PageId, int ContentId, RenderedPage Page)>();
            foreach (var page in _doc.Pages)
            {
                pageObjects.Add((nextId, nextId + 1, page));
                pageIds.Add(nextId);
                nextId += 2;
            }

            foreach (var (pageId, contentId, page) in pageObjects)
            {
                var usedFonts = new HashSet<string>();
                var usedImages = new HashSet<string>();
                var content = BuildContent(page, ref nextId, usedFonts, usedImages);

                BeginObject(contentId);
                WriteStream(content, compress: true, extraDict: "");
                EndObject();

                var resources = new StringBuilder("<< /ProcSet [/PDF /Text /ImageC /ImageB]");
                if (usedFonts.Count > 0)
                {
                    resources.Append(" /Font <<");
                    foreach (var f in usedFonts) resources.Append($" /{_fonts[f].Res} {_fonts[f].Obj} 0 R");
                    resources.Append(" >>");
                }
                if (usedImages.Count > 0)
                {
                    resources.Append(" /XObject <<");
                    foreach (var res in usedImages)
                    {
                        var img = _images.Values.First(v => v?.Res == res)!.Value;
                        resources.Append($" /{img.Res} {img.Obj} 0 R");
                    }
                    resources.Append(" >>");
                }
                resources.Append(" >>");

                BeginObject(pageId);
                WriteRaw($"<< /Type /Page /Parent {pagesId} 0 R /MediaBox [0 0 {N(_doc.PageWidth)} {N(_doc.PageHeight)}] " +
                         $"/Contents {contentId} 0 R /Resources {resources} >>\n");
                EndObject();
            }

            BeginObject(pagesId);
            WriteRaw($"<< /Type /Pages /Count {pageIds.Count} /Kids [{string.Join(" ", pageIds.Select(i => $"{i} 0 R"))}] >>\n");
            EndObject();

            BeginObject(catalogId);
            WriteRaw($"<< /Type /Catalog /Pages {pagesId} 0 R >>\n");
            EndObject();

            BeginObject(infoId);
            var now = DateTime.Now;
            WriteRaw($"<< /Producer {PdfString("IronReports")} /Title {PdfString(_doc.Title)} " +
                     $"/CreationDate {PdfString("D:" + now.ToString("yyyyMMddHHmmss", Inv))} >>\n");
            EndObject();

            var xref = _position;
            var count = _offsets.Count;
            var sb = new StringBuilder();
            sb.Append($"xref\n0 {count}\n0000000000 65535 f \n");
            for (var i = 1; i < count; i++) sb.Append(_offsets[i].ToString("0000000000", Inv)).Append(" 00000 n \n");
            sb.Append($"trailer\n<< /Size {count} /Root {catalogId} 0 R /Info {infoId} 0 R >>\nstartxref\n{xref}\n%%EOF\n");
            WriteRaw(sb.ToString());
        }

        private string BuildContent(RenderedPage page, ref int nextId, HashSet<string> usedFonts, HashSet<string> usedImages)
        {
            var h = _doc.PageHeight;
            var sb = new StringBuilder();
            foreach (var item in page.Items)
            {
                switch (item)
                {
                    case RectItem r:
                    {
                        var hasFill = r.Fill.A > 0;
                        var hasStroke = r.Stroke.A > 0 && r.StrokeWidth > 0;
                        if (!hasFill && !hasStroke) break;
                        if (hasFill) sb.Append(Rgb(r.Fill, fill: true));
                        if (hasStroke) sb.Append(Rgb(r.Stroke, fill: false)).Append($"{N(r.StrokeWidth)} w ");
                        sb.Append($"{N(r.Bounds.X)} {N(h - r.Bounds.Bottom)} {N(r.Bounds.Width)} {N(r.Bounds.Height)} re ");
                        sb.Append(hasFill && hasStroke ? "B\n" : hasFill ? "f\n" : "S\n");
                        break;
                    }
                    case EllipseItem el:
                    {
                        var hasFill = el.Fill.A > 0;
                        var hasStroke = el.Stroke.A > 0 && el.StrokeWidth > 0;
                        if (!hasFill && !hasStroke) break;
                        if (hasFill) sb.Append(Rgb(el.Fill, fill: true));
                        if (hasStroke) sb.Append(Rgb(el.Stroke, fill: false)).Append($"{N(el.StrokeWidth)} w ");
                        AppendEllipse(sb, el.Bounds, h);
                        sb.Append(hasFill && hasStroke ? "B\n" : hasFill ? "f\n" : "S\n");
                        break;
                    }
                    case LineItem l:
                        if (l.Width <= 0 || l.Color.A == 0) break;
                        sb.Append(Rgb(l.Color, fill: false))
                          .Append($"{N(l.Width)} w {N(l.X1)} {N(h - l.Y1)} m {N(l.X2)} {N(h - l.Y2)} l S\n");
                        break;
                    case TextItem t:
                    {
                        var fontName = BaseFontName(t.Font);
                        var font = GetFont(fontName, ref nextId);
                        usedFonts.Add(fontName);
                        sb.Append("BT ").Append(Rgb(t.Color, fill: true))
                          .Append($"/{font.Res} {N(t.Font.Size)} Tf {N(t.X)} {N(h - t.Baseline)} Td ")
                          .Append(PdfTextString(t.Text)).Append(" Tj ET\n");
                        break;
                    }
                    case ImageItem img:
                    {
                        var res = GetImage(img, ref nextId);
                        if (res is null)
                        {
                            // Unsupported picture: draw a light placeholder.
                            sb.Append("0.85 0.85 0.85 rg ")
                              .Append($"{N(img.Clip.X)} {N(h - img.Clip.Bottom)} {N(img.Clip.Width)} {N(img.Clip.Height)} re f\n");
                            break;
                        }
                        usedImages.Add(res.Value.Res);
                        var d = img.Destination;
                        var c = img.Clip;
                        sb.Append($"q {N(c.X)} {N(h - c.Bottom)} {N(c.Width)} {N(c.Height)} re W n ");
                        sb.Append($"{N(d.Width)} 0 0 {N(d.Height)} {N(d.X)} {N(h - d.Bottom)} cm /{res.Value.Res} Do Q\n");
                        break;
                    }
                }
            }
            return sb.ToString();
        }

        private static void AppendEllipse(StringBuilder sb, RectangleF r, float pageHeight)
        {
            const float k = 0.5522848f;
            var cx = r.X + r.Width / 2f;
            var cy = pageHeight - (r.Y + r.Height / 2f);
            var rx = r.Width / 2f;
            var ry = r.Height / 2f;
            var ox = rx * k;
            var oy = ry * k;
            sb.Append($"{N(cx + rx)} {N(cy)} m ");
            sb.Append($"{N(cx + rx)} {N(cy + oy)} {N(cx + ox)} {N(cy + ry)} {N(cx)} {N(cy + ry)} c ");
            sb.Append($"{N(cx - ox)} {N(cy + ry)} {N(cx - rx)} {N(cy + oy)} {N(cx - rx)} {N(cy)} c ");
            sb.Append($"{N(cx - rx)} {N(cy - oy)} {N(cx - ox)} {N(cy - ry)} {N(cx)} {N(cy - ry)} c ");
            sb.Append($"{N(cx + ox)} {N(cy - ry)} {N(cx + rx)} {N(cy - oy)} {N(cx + rx)} {N(cy)} c ");
        }

        private (int Obj, string Res) GetFont(string baseFont, ref int nextId)
        {
            if (_fonts.TryGetValue(baseFont, out var f)) return f;
            var id = nextId++;
            f = (id, "F" + (_fonts.Count + 1));
            _fonts[baseFont] = f;
            BeginObject(id);
            WriteRaw($"<< /Type /Font /Subtype /Type1 /BaseFont /{baseFont} /Encoding /WinAnsiEncoding >>\n");
            EndObject();
            return f;
        }

        private (int Obj, string Res)? GetImage(ImageItem item, ref int nextId)
        {
            if (_images.TryGetValue(item.Data, out var existing)) return existing;
            (int, string)? result = null;
            var res = "Im" + (_images.Count + 1);

            if (item.Info.Format == ImageFormatKind.Jpeg)
            {
                var id = nextId++;
                var cs = item.Info.JpegComponents switch { 1 => "/DeviceGray", 4 => "/DeviceCMYK", _ => "/DeviceRGB" };
                var decode = item.Info.JpegComponents == 4 ? " /Decode [1 0 1 0 1 0 1 0]" : "";
                BeginObject(id);
                WriteBinaryStream(item.Data,
                    $"/Type /XObject /Subtype /Image /Width {item.Info.PixelWidth} /Height {item.Info.PixelHeight} " +
                    $"/ColorSpace {cs} /BitsPerComponent 8 /Filter /DCTDecode{decode}", compress: false);
                EndObject();
                result = (id, res);
            }
            else if (PngDecoder.TryDecode(item.Data) is { } png)
            {
                int? maskId = null;
                if (png.Alpha is not null)
                {
                    maskId = nextId++;
                    BeginObject(maskId.Value);
                    WriteBinaryStream(png.Alpha,
                        $"/Type /XObject /Subtype /Image /Width {png.Width} /Height {png.Height} /ColorSpace /DeviceGray /BitsPerComponent 8",
                        compress: true);
                    EndObject();
                }
                var id = nextId++;
                BeginObject(id);
                WriteBinaryStream(png.Rgb,
                    $"/Type /XObject /Subtype /Image /Width {png.Width} /Height {png.Height} /ColorSpace /DeviceRGB /BitsPerComponent 8" +
                    (maskId is null ? "" : $" /SMask {maskId} 0 R"),
                    compress: true);
                EndObject();
                result = (id, res);
            }

            _images[item.Data] = result;
            return result;
        }

        private static string BaseFontName(FontSpec f) => f.Family switch
        {
            FontFamilyKind.Serif => (f.Bold, f.Italic) switch
            {
                (true, true) => "Times-BoldItalic",
                (true, false) => "Times-Bold",
                (false, true) => "Times-Italic",
                _ => "Times-Roman"
            },
            FontFamilyKind.Mono => (f.Bold, f.Italic) switch
            {
                (true, true) => "Courier-BoldOblique",
                (true, false) => "Courier-Bold",
                (false, true) => "Courier-Oblique",
                _ => "Courier"
            },
            _ => (f.Bold, f.Italic) switch
            {
                (true, true) => "Helvetica-BoldOblique",
                (true, false) => "Helvetica-Bold",
                (false, true) => "Helvetica-Oblique",
                _ => "Helvetica"
            }
        };

        private static string Rgb(Color c, bool fill) =>
            $"{N(c.R / 255f)} {N(c.G / 255f)} {N(c.B / 255f)} {(fill ? "rg" : "RG")} ";

        private static string N(float v)
        {
            if (float.IsNaN(v) || float.IsInfinity(v)) v = 0f;
            var s = Math.Round(v, 3).ToString("0.###", Inv);
            return s == "-0" ? "0" : s;
        }

        /// <summary>Text for a Tj operator: WinAnsi bytes as Latin-1 chars, with escapes.</summary>
        private static string PdfTextString(string text)
        {
            var sb = new StringBuilder("(");
            foreach (var ch in text)
            {
                var b = WinAnsi.Encode(ch);
                switch (b)
                {
                    case (byte)'(': sb.Append("\\("); break;
                    case (byte)')': sb.Append("\\)"); break;
                    case (byte)'\\': sb.Append("\\\\"); break;
                    default:
                        if (b < 32) sb.Append(' ');
                        else sb.Append((char)b);
                        break;
                }
            }
            return sb.Append(')').ToString();
        }

        /// <summary>A string for the info dictionary, in UTF-16BE so any title is preserved.</summary>
        private static string PdfString(string text)
        {
            var bytes = Encoding.BigEndianUnicode.GetBytes(text ?? "");
            return "<FEFF" + Convert.ToHexString(bytes) + ">";
        }

        private void BeginObject(int id)
        {
            while (_offsets.Count <= id) _offsets.Add(0);
            _offsets[id] = _position;
            WriteRaw($"{id} 0 obj\n");
        }

        private void EndObject() => WriteRaw("endobj\n");

        private void WriteStream(string content, bool compress, string extraDict) =>
            WriteBinaryStream(Encoding.Latin1.GetBytes(content), extraDict.Trim(), compress);

        private void WriteBinaryStream(byte[] data, string dict, bool compress)
        {
            var body = data;
            var filter = "";
            if (compress)
            {
                using var ms = new MemoryStream();
                using (var z = new ZLibStream(ms, CompressionLevel.Optimal, leaveOpen: true)) z.Write(data, 0, data.Length);
                body = ms.ToArray();
                filter = " /Filter /FlateDecode";
            }
            WriteRaw($"<< {dict}{(dict.Length > 0 ? " " : "")}/Length {body.Length}{filter} >>\nstream\n");
            WriteBytes(body);
            WriteRaw("\nendstream\n");
        }

        private void WriteRaw(string s) => WriteBytes(Encoding.Latin1.GetBytes(s));

        private void WriteBytes(byte[] bytes)
        {
            _out.Write(bytes, 0, bytes.Length);
            _position += bytes.Length;
        }
    }
}

/// <summary>Maps Unicode characters to the PDF WinAnsiEncoding (Windows-1252).</summary>
internal static class WinAnsi
{
    private static readonly Dictionary<char, byte> High = new()
    {
        ['€'] = 0x80, ['‚'] = 0x82, ['ƒ'] = 0x83, ['„'] = 0x84, ['…'] = 0x85,
        ['†'] = 0x86, ['‡'] = 0x87, ['ˆ'] = 0x88, ['‰'] = 0x89, ['Š'] = 0x8A,
        ['‹'] = 0x8B, ['Œ'] = 0x8C, ['Ž'] = 0x8E, ['‘'] = 0x91, ['’'] = 0x92,
        ['“'] = 0x93, ['”'] = 0x94, ['•'] = 0x95, ['–'] = 0x96, ['—'] = 0x97,
        ['˜'] = 0x98, ['™'] = 0x99, ['š'] = 0x9A, ['›'] = 0x9B, ['œ'] = 0x9C,
        ['ž'] = 0x9E, ['Ÿ'] = 0x9F
    };

    public static byte Encode(char c)
    {
        if (c < 0x80 || (c >= 0xA0 && c <= 0xFF)) return (byte)c;
        if (High.TryGetValue(c, out var b)) return b;
        // Strip accents for characters outside the code page, else '?'.
        var basic = c.ToString().Normalize(NormalizationForm.FormD);
        return basic.Length > 0 && basic[0] < 0x80 ? (byte)basic[0] : (byte)'?';
    }
}

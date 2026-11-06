using System.Globalization;
using System.Text;
using IronReports.Model;

namespace IronReports.Layout;

/// <summary>
/// Character widths of the PDF base fonts (units of 1/1000 em), for characters 32..126.
/// Arial, Times New Roman and Courier New are metric-compatible with Helvetica, Times and Courier,
/// so layout done with these tables matches both the PDF and the screen.
/// </summary>
public static class FontMetrics
{
    private static readonly short[] Helvetica =
    {
        278, 278, 355, 556, 556, 889, 667, 191, 333, 333, 389, 584, 278, 333, 278, 278,
        556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 278, 278, 584, 584, 584, 556,
        1015, 667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778,
        667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 278, 278, 278, 469, 556,
        333, 556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556,
        556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500, 334, 260, 334, 584
    };

    private static readonly short[] HelveticaBold =
    {
        278, 333, 474, 556, 556, 889, 722, 238, 333, 333, 389, 584, 278, 333, 278, 278,
        556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 333, 333, 584, 584, 584, 611,
        975, 722, 722, 722, 722, 667, 611, 778, 722, 278, 556, 722, 611, 833, 722, 778,
        667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 333, 278, 333, 584, 556,
        333, 556, 611, 556, 611, 556, 333, 611, 611, 278, 278, 556, 278, 889, 611, 611,
        611, 611, 389, 556, 333, 611, 556, 778, 556, 556, 500, 389, 280, 389, 584
    };

    private static readonly short[] TimesRoman =
    {
        250, 333, 408, 500, 500, 833, 778, 180, 333, 333, 500, 564, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 278, 278, 564, 564, 564, 444,
        921, 722, 667, 667, 722, 611, 556, 722, 722, 333, 389, 722, 611, 889, 722, 722,
        556, 722, 667, 556, 611, 722, 722, 944, 722, 722, 611, 333, 278, 333, 469, 500,
        333, 444, 500, 444, 500, 444, 333, 500, 500, 278, 278, 500, 278, 778, 500, 500,
        500, 500, 333, 389, 278, 500, 500, 722, 500, 500, 444, 480, 200, 480, 541
    };

    private static readonly short[] TimesBold =
    {
        250, 333, 555, 500, 500, 1000, 833, 278, 333, 333, 500, 570, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 570, 570, 570, 500,
        930, 722, 667, 722, 722, 667, 611, 778, 778, 389, 500, 778, 667, 944, 722, 778,
        611, 778, 722, 556, 667, 722, 722, 1000, 722, 722, 667, 333, 278, 333, 581, 500,
        333, 500, 556, 444, 556, 444, 333, 500, 556, 278, 333, 556, 278, 833, 556, 500,
        556, 556, 444, 389, 333, 556, 500, 722, 500, 500, 444, 394, 220, 394, 520
    };

    private static readonly short[] TimesItalic =
    {
        250, 333, 420, 500, 500, 833, 778, 214, 333, 333, 500, 675, 250, 333, 250, 278,
        500, 500, 500, 500, 500, 500, 500, 500, 500, 500, 333, 333, 675, 675, 675, 500,
        920, 611, 611, 667, 722, 611, 611, 722, 722, 333, 444, 667, 556, 833, 667, 722,
        611, 722, 611, 500, 556, 722, 611, 833, 611, 556, 556, 389, 278, 389, 422, 500,
        333, 500, 500, 444, 500, 444, 278, 500, 500, 278, 278, 444, 278, 722, 500, 500,
        500, 500, 389, 389, 278, 500, 444, 667, 444, 444, 389, 400, 275, 400, 541
    };

    /// <summary>Widths for common characters outside 32..126 (Sans regular; others are scaled).</summary>
    private static readonly Dictionary<char, short> Extra = new()
    {
        [' '] = 278, ['–'] = 556, ['—'] = 1000, ['‘'] = 222, ['’'] = 222,
        ['“'] = 333, ['”'] = 333, ['•'] = 350, ['…'] = 1000, ['€'] = 556,
        ['£'] = 556, ['©'] = 737, ['®'] = 737, ['°'] = 400, ['±'] = 584,
        ['×'] = 584, ['÷'] = 584, ['½'] = 834, ['¼'] = 834, ['¾'] = 834,
        ['™'] = 1000, ['µ'] = 556, ['§'] = 556, ['·'] = 278
    };

    /// <summary>Line spacing as a multiple of the font size (matches Arial and Times New Roman).</summary>
    public const float LineSpacing = 1.15f;

    public static float LineHeight(FontSpec font) => font.Size * LineSpacing;

    /// <summary>Distance from the top of a line to the baseline.</summary>
    public static float Ascent(FontSpec font) => font.Size * font.Family switch
    {
        FontFamilyKind.Serif => 0.891f,
        FontFamilyKind.Mono => 0.833f,
        _ => 0.905f
    };

    public static float MeasureWidth(string text, FontSpec font)
    {
        if (string.IsNullOrEmpty(text)) return 0f;
        if (font.Family == FontFamilyKind.Mono) return text.Length * 0.6f * font.Size;
        var table = TableFor(font);
        var scale = TableScaleForExtras(font);
        var total = 0;
        foreach (var ch in text) total += CharWidth(ch, table, scale);
        return total * font.Size / 1000f;
    }

    private static short[] TableFor(FontSpec font) => font.Family switch
    {
        FontFamilyKind.Serif when font.Bold => TimesBold,
        FontFamilyKind.Serif when font.Italic => TimesItalic,
        FontFamilyKind.Serif => TimesRoman,
        _ when font.Bold => HelveticaBold,
        _ => Helvetica
    };

    private static float TableScaleForExtras(FontSpec font) => font.Family == FontFamilyKind.Serif ? 0.9f : font.Bold ? 1.05f : 1f;

    private static int CharWidth(char ch, short[] table, float extraScale)
    {
        if (ch >= 32 && ch <= 126) return table[ch - 32];
        if (ch == '\t') return table[0] * 4;
        if (Extra.TryGetValue(ch, out var w)) return (int)(w * extraScale);
        // Accented letters: use the width of the base letter.
        var decomposed = ch.ToString().Normalize(NormalizationForm.FormD);
        if (decomposed.Length > 0 && decomposed[0] >= 32 && decomposed[0] <= 126) return table[decomposed[0] - 32];
        return CharUnicodeInfo.GetUnicodeCategory(ch) is UnicodeCategory.UppercaseLetter ? 722 : 556;
    }

    /// <summary>
    /// Breaks text into lines that fit <paramref name="maxWidth"/>. Explicit line breaks are kept.
    /// Words longer than a line are broken between characters.
    /// </summary>
    public static List<string> WrapText(string text, FontSpec font, float maxWidth, bool wordWrap)
    {
        var lines = new List<string>();
        var paragraphs = (text ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        foreach (var paragraph in paragraphs)
        {
            if (!wordWrap || maxWidth <= 0f || MeasureWidth(paragraph, font) <= maxWidth)
            {
                lines.Add(paragraph.TrimEnd());
                continue;
            }

            var current = new StringBuilder();
            foreach (var word in SplitKeepingSpaces(paragraph))
            {
                var candidate = current.ToString() + word;
                if (MeasureWidth(candidate.TrimEnd(), font) <= maxWidth)
                {
                    current.Append(word);
                    continue;
                }

                if (current.Length > 0 && current.ToString().Trim().Length > 0)
                {
                    lines.Add(current.ToString().TrimEnd());
                    current.Clear();
                }

                var w = word.TrimStart();
                if (current.Length == 0 && MeasureWidth(w.TrimEnd(), font) > maxWidth)
                {
                    // Break a long word by characters.
                    var piece = new StringBuilder();
                    foreach (var ch in w)
                    {
                        if (piece.Length > 0 && MeasureWidth(piece.ToString() + ch, font) > maxWidth)
                        {
                            lines.Add(piece.ToString());
                            piece.Clear();
                        }
                        piece.Append(ch);
                    }
                    current.Append(piece);
                }
                else
                {
                    current.Append(w);
                }
            }
            lines.Add(current.ToString().TrimEnd());
        }
        return lines;
    }

    /// <summary>Splits into chunks of "word plus trailing spaces".</summary>
    private static IEnumerable<string> SplitKeepingSpaces(string s)
    {
        var start = 0;
        var i = 0;
        while (i < s.Length)
        {
            while (i < s.Length && s[i] != ' ') i++;
            while (i < s.Length && s[i] == ' ') i++;
            yield return s[start..i];
            start = i;
        }
    }
}

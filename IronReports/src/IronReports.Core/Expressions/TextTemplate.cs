using System.Collections.Concurrent;
using System.Text;

namespace IronReports.Expressions;

/// <summary>
/// Literal text with expression holes: "Total: {Sum([Amount]):N2}".
/// A colon outside quotes, brackets and parentheses starts the format. Use {{ and }} for literal braces.
/// </summary>
public sealed class TextTemplate
{
    private static readonly ConcurrentDictionary<string, TextTemplate> Cache = new();

    private abstract record Part;
    private sealed record Literal(string Text) : Part;
    private sealed record Hole(ReportExpression Expression, string? Format) : Part;

    private readonly List<Part> _parts;

    private TextTemplate(string source, List<Part> parts)
    {
        Source = source;
        _parts = parts;
        UsesPageInfo = parts.OfType<Hole>().Any(h => h.Expression.UsesPageInfo);
        IsSingleExpression = parts.Count == 1 && parts[0] is Hole;
    }

    public string Source { get; }

    public bool UsesPageInfo { get; }

    /// <summary>True when the whole template is one hole, so <see cref="EvaluateValue"/> keeps the raw type.</summary>
    public bool IsSingleExpression { get; }

    /// <summary>True when the template has no expressions at all.</summary>
    public bool IsLiteral => _parts.All(p => p is Literal);

    public static TextTemplate Parse(string source) => Cache.GetOrAdd(source ?? "", static s => new TextTemplate(s, ParseParts(s)));

    /// <summary>Returns null when the template is valid, otherwise the first error.</summary>
    public static string? Validate(string source)
    {
        try
        {
            ParseParts(source);
            return null;
        }
        catch (ExpressionException ex)
        {
            return ex.Message;
        }
    }

    public string Evaluate(EvalContext ctx, string? defaultFormat = null)
    {
        if (_parts.Count == 1 && _parts[0] is Literal only) return only.Text;
        var sb = new StringBuilder();
        foreach (var part in _parts)
        {
            switch (part)
            {
                case Literal l:
                    sb.Append(l.Text);
                    break;
                case Hole h:
                    var value = h.Expression.Evaluate(ctx);
                    var format = h.Format ?? (string.IsNullOrEmpty(defaultFormat) ? null : defaultFormat);
                    sb.Append(Operators.ToText(value, ctx.Culture, format));
                    break;
            }
        }
        return sb.ToString();
    }

    /// <summary>The raw value for single-expression templates (keeps numbers numeric for Excel), else the text.</summary>
    public object? EvaluateValue(EvalContext ctx, string? defaultFormat = null)
    {
        if (!IsSingleExpression || _parts[0] is not Hole hole) return Evaluate(ctx, defaultFormat);
        var value = hole.Expression.Evaluate(ctx);
        if (Operators.IsNumeric(value) || value is DateTime or bool) return value;
        return Operators.ToText(value, ctx.Culture, hole.Format ?? (string.IsNullOrEmpty(defaultFormat) ? null : defaultFormat));
    }

    private static List<Part> ParseParts(string s)
    {
        var parts = new List<Part>();
        var literal = new StringBuilder();
        var i = 0;
        while (i < s.Length)
        {
            var c = s[i];
            if (c == '{' && i + 1 < s.Length && s[i + 1] == '{') { literal.Append('{'); i += 2; continue; }
            if (c == '}' && i + 1 < s.Length && s[i + 1] == '}') { literal.Append('}'); i += 2; continue; }
            if (c != '{') { literal.Append(c); i++; continue; }

            var start = i;
            i++;
            int depth = 0, colon = -1;
            char quote = '\0';
            var inBracket = false;
            for (; i < s.Length; i++)
            {
                var ch = s[i];
                if (quote != '\0') { if (ch == quote) quote = '\0'; continue; }
                if (inBracket) { if (ch == ']') inBracket = false; continue; }
                if (ch is '\'' or '"') { quote = ch; continue; }
                if (ch == '[') { inBracket = true; continue; }
                if (ch == '(') { depth++; continue; }
                if (ch == ')') { depth--; continue; }
                if (ch == ':' && depth == 0 && colon < 0) { colon = i; continue; }
                if (ch == '}' && (depth <= 0 || colon >= 0)) break;
            }
            if (i >= s.Length) throw new ExpressionException("Missing '}' in text", start);

            var exprEnd = colon >= 0 ? colon : i;
            var exprText = s[(start + 1)..exprEnd].Trim();
            var format = colon >= 0 ? s[(colon + 1)..i] : null;
            if (exprText.Length == 0) throw new ExpressionException("Empty {} in text", start);

            if (literal.Length > 0) { parts.Add(new Literal(literal.ToString())); literal.Clear(); }
            parts.Add(new Hole(ReportExpression.Compile(exprText), string.IsNullOrEmpty(format) ? null : format));
            i++; // past }
        }
        if (literal.Length > 0 || parts.Count == 0) parts.Add(new Literal(literal.ToString()));
        return parts;
    }
}

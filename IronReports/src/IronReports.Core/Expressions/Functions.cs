using System.Globalization;
using IronReports.Data;

namespace IronReports.Expressions;

/// <summary>Describes a function for the designer's expression helper.</summary>
public sealed record FunctionInfo(string Name, string Signature, string Description, string Category);

/// <summary>Built-in expression functions.</summary>
public static class Functions
{
    private delegate object? Eager(object?[] args, EvalContext ctx);

    private delegate object? Lazy(Evaluator[] args, EvalContext ctx);

    private sealed record Def(int Min, int Max, Eager? Eager, Lazy? Lazy);

    private static readonly Dictionary<string, Def> Defs = new(StringComparer.OrdinalIgnoreCase);
    private static readonly List<FunctionInfo> InfoList = new();

    /// <summary>Every function, for display in the designer.</summary>
    public static IReadOnlyList<FunctionInfo> Catalog => InfoList;

    static Functions()
    {
        // Logic
        Lz("Iif", 3, 3, (a, c) => Operators.ToBool(a[0](c)) ? a[1](c) : a[2](c),
            "Iif(condition, whenTrue, whenFalse)", "Returns one of two values.", "Logic");
        Lz("IsNull", 2, 2, (a, c) => a[0](c) ?? a[1](c),
            "IsNull(value, fallback)", "Returns fallback when value is null.", "Logic");
        Fn("IsNullOrEmpty", 1, 1, (a, c) => a[0] is null || (a[0] is string s && s.Length == 0),
            "IsNullOrEmpty(value)", "True when value is null or empty text.", "Logic");
        Fn("IsNullOrWhiteSpace", 1, 1, (a, c) => a[0] is null || (a[0] is string s && string.IsNullOrWhiteSpace(s)),
            "IsNullOrWhiteSpace(value)", "True when value is null, empty or only spaces.", "Logic");

        // Text
        Fn("Len", 1, 1, (a, c) => Str(a[0], c).Length, "Len(text)", "Number of characters.", "Text");
        Fn("Upper", 1, 1, (a, c) => Str(a[0], c).ToUpper(c.Culture), "Upper(text)", "Upper case.", "Text");
        Fn("Lower", 1, 1, (a, c) => Str(a[0], c).ToLower(c.Culture), "Lower(text)", "Lower case.", "Text");
        Fn("Trim", 1, 1, (a, c) => Str(a[0], c).Trim(), "Trim(text)", "Removes leading and trailing spaces.", "Text");
        Fn("Left", 2, 2, (a, c) => { var s = Str(a[0], c); var n = Math.Clamp(Int(a[1], c), 0, s.Length); return s[..n]; },
            "Left(text, count)", "First characters.", "Text");
        Fn("Right", 2, 2, (a, c) => { var s = Str(a[0], c); var n = Math.Clamp(Int(a[1], c), 0, s.Length); return s[^n..]; },
            "Right(text, count)", "Last characters.", "Text");
        Fn("Substring", 2, 3, (a, c) =>
            {
                var s = Str(a[0], c);
                var start = Math.Clamp(Int(a[1], c), 0, s.Length);
                var len = a.Length > 2 ? Math.Clamp(Int(a[2], c), 0, s.Length - start) : s.Length - start;
                return s.Substring(start, len);
            },
            "Substring(text, start[, length])", "Part of the text. Start is zero-based.", "Text");
        Fn("Replace", 3, 3, (a, c) => { var find = Str(a[1], c); return find.Length == 0 ? Str(a[0], c) : Str(a[0], c).Replace(find, Str(a[2], c)); },
            "Replace(text, find, replacement)", "Replaces every occurrence.", "Text");
        Fn("Contains", 2, 2, (a, c) => Str(a[0], c).Contains(Str(a[1], c), StringComparison.OrdinalIgnoreCase),
            "Contains(text, part)", "True when part occurs in text (ignores case).", "Text");
        Fn("StartsWith", 2, 2, (a, c) => Str(a[0], c).StartsWith(Str(a[1], c), StringComparison.OrdinalIgnoreCase),
            "StartsWith(text, part)", "True when text starts with part (ignores case).", "Text");
        Fn("EndsWith", 2, 2, (a, c) => Str(a[0], c).EndsWith(Str(a[1], c), StringComparison.OrdinalIgnoreCase),
            "EndsWith(text, part)", "True when text ends with part (ignores case).", "Text");
        Fn("PadLeft", 2, 3, (a, c) => Str(a[0], c).PadLeft(Math.Max(0, Int(a[1], c)), a.Length > 2 ? FirstChar(a[2], c) : ' '),
            "PadLeft(text, width[, char])", "Pads on the left to the given width.", "Text");
        Fn("PadRight", 2, 3, (a, c) => Str(a[0], c).PadRight(Math.Max(0, Int(a[1], c)), a.Length > 2 ? FirstChar(a[2], c) : ' '),
            "PadRight(text, width[, char])", "Pads on the right to the given width.", "Text");
        Fn("Concat", 0, 64, (a, c) => string.Concat(a.Select(x => Str(x, c))),
            "Concat(a, b, ...)", "Joins values as text.", "Text");
        Fn("Format", 1, 64, (a, c) => string.Format(c.Culture, Str(a[0], c), a.Skip(1).ToArray()),
            "Format('{0:N2} of {1}', a, b)", ".NET composite formatting.", "Text");
        Fn("ToStr", 1, 2, (a, c) => Operators.ToText(a[0], c.Culture, a.Length > 1 ? Str(a[1], c) : null),
            "ToStr(value[, format])", "Converts to text with an optional .NET format, e.g. ToStr([Date], 'dd MMM yyyy').", "Text");
        Fn("NewLine", 0, 0, (a, c) => "\n", "NewLine()", "A line break.", "Text");

        // Numbers
        Fn("ToNumber", 1, 1, (a, c) => a[0] is null ? null : Operators.ToDecimal(a[0], c.Culture),
            "ToNumber(value)", "Converts to a decimal number.", "Math");
        Fn("ToInt", 1, 1, (a, c) => a[0] is null ? null : (long)decimal.Truncate(Operators.ToDecimal(a[0], c.Culture)),
            "ToInt(value)", "Converts to a whole number (truncates).", "Math");
        Fn("Round", 1, 2, (a, c) => a[0] is null ? null : Math.Round(Operators.ToDecimal(a[0], c.Culture), a.Length > 1 ? Math.Clamp(Int(a[1], c), 0, 28) : 0, MidpointRounding.AwayFromZero),
            "Round(value[, digits])", "Rounds half away from zero.", "Math");
        Fn("Floor", 1, 1, (a, c) => a[0] is null ? null : Math.Floor(Operators.ToDecimal(a[0], c.Culture)), "Floor(value)", "Rounds down.", "Math");
        Fn("Ceiling", 1, 1, (a, c) => a[0] is null ? null : Math.Ceiling(Operators.ToDecimal(a[0], c.Culture)), "Ceiling(value)", "Rounds up.", "Math");
        Fn("Abs", 1, 1, (a, c) => a[0] is null ? null : Math.Abs(Operators.ToDecimal(a[0], c.Culture)), "Abs(value)", "Absolute value.", "Math");

        // Dates
        Fn("Now", 0, 0, (a, c) => c.Now, "Now()", "Date and time the report started running.", "Date");
        Fn("Today", 0, 0, (a, c) => c.Now.Date, "Today()", "Today's date.", "Date");
        Fn("ToDate", 1, 1, (a, c) => ToDate(a[0], c), "ToDate(value)", "Converts text to a date.", "Date");
        Fn("Year", 1, 1, (a, c) => ToDate(a[0], c)?.Year, "Year(date)", "Year part.", "Date");
        Fn("Month", 1, 1, (a, c) => ToDate(a[0], c)?.Month, "Month(date)", "Month part (1-12).", "Date");
        Fn("Day", 1, 1, (a, c) => ToDate(a[0], c)?.Day, "Day(date)", "Day of the month.", "Date");
        Fn("AddDays", 2, 2, (a, c) => ToDate(a[0], c)?.AddDays(Operators.ToDouble(a[1], c.Culture)), "AddDays(date, days)", "Adds days.", "Date");
        Fn("AddMonths", 2, 2, (a, c) => ToDate(a[0], c)?.AddMonths(Int(a[1], c)), "AddMonths(date, months)", "Adds months.", "Date");
        Fn("DateDiffDays", 2, 2, (a, c) =>
            {
                var d1 = ToDate(a[0], c);
                var d2 = ToDate(a[1], c);
                return d1 is null || d2 is null ? null : (long)(d2.Value.Date - d1.Value.Date).TotalDays;
            },
            "DateDiffDays(from, to)", "Whole days between two dates.", "Date");

        // Report
        Fn("PageNumber", 0, 0, (a, c) => c.PageNumber, "PageNumber()", "Current page number.", "Report");
        Fn("PageCount", 0, 0, (a, c) => c.PageCount, "PageCount()", "Total number of pages.", "Report");
        Fn("RowNumber", 0, 0, (a, c) => c.RowNumber, "RowNumber()", "Row number within the current group.", "Report");
        Fn("RecordNumber", 0, 0, (a, c) => c.RecordNumber, "RecordNumber()", "Row number within the report.", "Report");

        // Aggregates over the current scope (group, or whole report in report bands)
        Lz("Sum", 1, 1, (a, c) => Aggregate(a[0], c, values =>
            {
                if (values.Any(v => v is double or float)) return values.Sum(v => Operators.ToDouble(v, c.Culture));
                return values.Sum(v => Operators.ToDecimal(v, c.Culture));
            }, emptyResult: 0m),
            "Sum(expression)", "Total over the current group or report.", "Aggregate");
        Lz("Avg", 1, 1, (a, c) => Aggregate(a[0], c, values => values.Average(v => Operators.ToDecimal(v, c.Culture)), emptyResult: null),
            "Avg(expression)", "Average of non-null values.", "Aggregate");
        Lz("Count", 0, 1, (a, c) => a.Length == 0 ? c.ScopeRows.Count : Aggregate(a[0], c, values => values.Count, emptyResult: 0),
            "Count([expression])", "Number of rows, or of non-null values.", "Aggregate");
        Lz("CountDistinct", 1, 1, (a, c) => Aggregate(a[0], c, values => values.Select(v => Operators.ToText(v, CultureInfo.InvariantCulture)).Distinct().Count(), emptyResult: 0),
            "CountDistinct(expression)", "Number of different non-null values.", "Aggregate");
        Lz("Min", 1, 2, (a, c) => a.Length == 2 ? Pick(a[0](c), a[1](c), c, wantMin: true) : Aggregate(a[0], c, values => values.Aggregate((x, y) => Operators.Compare(x, y, c.Culture) <= 0 ? x : y), emptyResult: null),
            "Min(expression) or Min(a, b)", "Smallest value in the scope, or of two values.", "Aggregate");
        Lz("Max", 1, 2, (a, c) => a.Length == 2 ? Pick(a[0](c), a[1](c), c, wantMin: false) : Aggregate(a[0], c, values => values.Aggregate((x, y) => Operators.Compare(x, y, c.Culture) >= 0 ? x : y), emptyResult: null),
            "Max(expression) or Max(a, b)", "Largest value in the scope, or of two values.", "Aggregate");
        Lz("First", 1, 1, (a, c) => OnRow(a[0], c, c.ScopeRows.Count > 0 ? c.ScopeRows[0] : null),
            "First(expression)", "Value from the first row of the scope.", "Aggregate");
        Lz("Last", 1, 1, (a, c) => OnRow(a[0], c, c.ScopeRows.Count > 0 ? c.ScopeRows[^1] : null),
            "Last(expression)", "Value from the last row of the scope.", "Aggregate");
        Lz("SumAll", 1, 1, (a, c) =>
            {
                var saved = c.ScopeRows;
                c.ScopeRows = c.AllRows;
                try { return Aggregate(a[0], c, values => values.Sum(v => Operators.ToDecimal(v, c.Culture)), emptyResult: 0m); }
                finally { c.ScopeRows = saved; }
            },
            "SumAll(expression)", "Total over the whole report, from any band (for percentages).", "Aggregate");
    }

    internal static Evaluator Bind(string name, Evaluator[] args, int position)
    {
        if (!Defs.TryGetValue(name, out var def)) throw new ExpressionException($"Unknown function '{name}'", position);
        if (args.Length < def.Min || args.Length > def.Max)
        {
            var expected = def.Min == def.Max ? $"{def.Min}" : $"{def.Min} to {def.Max}";
            throw new ExpressionException($"{name} expects {expected} argument(s), got {args.Length}", position);
        }
        if (def.Lazy is { } lazy) return ctx => lazy(args, ctx);
        var eager = def.Eager!;
        return ctx =>
        {
            var values = new object?[args.Length];
            for (var i = 0; i < args.Length; i++) values[i] = args[i](ctx);
            return eager(values, ctx);
        };
    }

    private static void Fn(string name, int min, int max, Eager body, string signature, string description, string category)
    {
        Defs[name] = new Def(min, max, body, null);
        InfoList.Add(new FunctionInfo(name, signature, description, category));
    }

    private static void Lz(string name, int min, int max, Lazy body, string signature, string description, string category)
    {
        Defs[name] = new Def(min, max, null, body);
        InfoList.Add(new FunctionInfo(name, signature, description, category));
    }

    private static string Str(object? v, EvalContext c) => Operators.ToText(v, c.Culture);

    private static int Int(object? v, EvalContext c) => (int)decimal.Truncate(Operators.ToDecimal(v, c.Culture));

    private static char FirstChar(object? v, EvalContext c)
    {
        var s = Str(v, c);
        return s.Length > 0 ? s[0] : ' ';
    }

    private static DateTime? ToDate(object? v, EvalContext c) => v switch
    {
        null => null,
        DateTime d => d,
        DateTimeOffset o => o.DateTime,
        DateOnly d => d.ToDateTime(TimeOnly.MinValue),
        string s when s.Length == 0 => null,
        string s => DateTime.TryParse(s, c.Culture, DateTimeStyles.None, out var r) ? r
            : DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out r) ? r
            : throw new ExpressionException($"'{s}' is not a date"),
        _ => throw new ExpressionException($"Value of type {v.GetType().Name} is not a date")
    };

    private static object? Pick(object? a, object? b, EvalContext c, bool wantMin)
    {
        if (a is null) return b;
        if (b is null) return a;
        var cmp = Operators.Compare(a, b, c.Culture);
        return wantMin ? (cmp <= 0 ? a : b) : (cmp >= 0 ? a : b);
    }

    private static object? OnRow(Evaluator arg, EvalContext c, IReportRow? row)
    {
        if (row is null) return null;
        var saved = c.Row;
        c.Row = row;
        try { return arg(c); }
        finally { c.Row = saved; }
    }

    private static object? Aggregate(Evaluator arg, EvalContext c, Func<List<object>, object?> reduce, object? emptyResult)
    {
        var saved = c.Row;
        var values = new List<object>(c.ScopeRows.Count);
        try
        {
            foreach (var row in c.ScopeRows)
            {
                c.Row = row;
                var v = arg(c);
                if (v is not null) values.Add(v);
            }
        }
        finally
        {
            c.Row = saved;
        }
        return values.Count == 0 ? emptyResult : reduce(values);
    }
}

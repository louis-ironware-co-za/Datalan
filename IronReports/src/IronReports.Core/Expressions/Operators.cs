using System.Globalization;

namespace IronReports.Expressions;

/// <summary>Type rules for expression operators. Numbers are decimal unless a double is involved.</summary>
public static class Operators
{
    public static bool IsNumeric(object? v) => v is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;

    private static bool IsFloating(object? v) => v is float or double;

    public static decimal ToDecimal(object? v, CultureInfo culture)
    {
        switch (v)
        {
            case null: return 0m;
            case decimal d: return d;
            case double or float:
                var dbl = Convert.ToDouble(v, CultureInfo.InvariantCulture);
                if (double.IsNaN(dbl) || double.IsInfinity(dbl)) throw new ExpressionException("Number is not finite");
                return (decimal)dbl;
            case bool b: return b ? 1m : 0m;
            case string s:
                if (decimal.TryParse(s, NumberStyles.Any, culture, out var r)) return r;
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out r)) return r;
                throw new ExpressionException($"'{s}' is not a number");
            default:
                if (IsNumeric(v)) return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
                throw new ExpressionException($"Value of type {v.GetType().Name} is not a number");
        }
    }

    public static double ToDouble(object? v, CultureInfo culture) => v switch
    {
        null => 0d,
        double d => d,
        float f => f,
        string s => double.TryParse(s, NumberStyles.Any, culture, out var r) ? r : (double)ToDecimal(s, culture),
        bool b => b ? 1d : 0d,
        _ when IsNumeric(v) => Convert.ToDouble(v, CultureInfo.InvariantCulture),
        _ => throw new ExpressionException($"Value of type {v.GetType().Name} is not a number")
    };

    public static bool ToBool(object? v) => v switch
    {
        null => false,
        bool b => b,
        string s => s.Length > 0 && !s.Equals("false", StringComparison.OrdinalIgnoreCase) && s != "0",
        _ when IsNumeric(v) => Convert.ToDecimal(v, CultureInfo.InvariantCulture) != 0m,
        _ => true
    };

    public static string ToText(object? v, CultureInfo culture, string? format = null)
    {
        if (v is null) return "";
        if (!string.IsNullOrEmpty(format) && v is IFormattable f) return f.ToString(format, culture);
        return v switch
        {
            string s => s,
            bool b => b ? "True" : "False",
            DateTime dt => dt.TimeOfDay == TimeSpan.Zero ? dt.ToString("d", culture) : dt.ToString(culture),
            IFormattable fm => fm.ToString(null, culture),
            byte[] bytes => $"[{bytes.Length} bytes]",
            _ => v.ToString() ?? ""
        };
    }

    public static object? Add(object? a, object? b, CultureInfo culture)
    {
        if (a is string || b is string) return ToText(a, culture) + ToText(b, culture);
        if (a is null) return b;
        if (b is null) return a;
        if (a is DateTime da && IsNumeric(b)) return da.AddDays(ToDouble(b, culture));
        if (a is DateTime db2 && b is TimeSpan ts) return db2 + ts;
        if (a is TimeSpan t1 && b is TimeSpan t2) return t1 + t2;
        return Arithmetic(a, b, culture, (x, y) => x + y, (x, y) => x + y);
    }

    public static object? Subtract(object? a, object? b, CultureInfo culture)
    {
        if (a is null || b is null) return null;
        if (a is DateTime d1 && b is DateTime d2) return (d1 - d2).TotalDays;
        if (a is DateTime d && IsNumeric(b)) return d.AddDays(-ToDouble(b, culture));
        if (a is DateTime d3 && b is TimeSpan ts) return d3 - ts;
        if (a is TimeSpan t1 && b is TimeSpan t2) return t1 - t2;
        return Arithmetic(a, b, culture, (x, y) => x - y, (x, y) => x - y);
    }

    public static object? Multiply(object? a, object? b, CultureInfo culture) =>
        a is null || b is null ? null : Arithmetic(a, b, culture, (x, y) => x * y, (x, y) => x * y);

    public static object? Divide(object? a, object? b, CultureInfo culture)
    {
        if (a is null || b is null) return null;
        if (IsFloating(a) || IsFloating(b)) return ToDouble(a, culture) / ToDouble(b, culture);
        var divisor = ToDecimal(b, culture);
        if (divisor == 0m) return null; // a report should not crash on a zero total
        return ToDecimal(a, culture) / divisor;
    }

    public static object? Modulo(object? a, object? b, CultureInfo culture)
    {
        if (a is null || b is null) return null;
        if (IsFloating(a) || IsFloating(b)) return ToDouble(a, culture) % ToDouble(b, culture);
        var divisor = ToDecimal(b, culture);
        if (divisor == 0m) return null;
        return ToDecimal(a, culture) % divisor;
    }

    public static object? Negate(object? a, CultureInfo culture) => a switch
    {
        null => null,
        double d => -d,
        float f => -f,
        TimeSpan t => -t,
        _ => -ToDecimal(a, culture)
    };

    private static object Arithmetic(object a, object b, CultureInfo culture, Func<decimal, decimal, decimal> dec, Func<double, double, double> dbl)
    {
        if (IsFloating(a) || IsFloating(b)) return dbl(ToDouble(a, culture), ToDouble(b, culture));
        return dec(ToDecimal(a, culture), ToDecimal(b, culture));
    }

    public static bool AreEqual(object? a, object? b, CultureInfo culture)
    {
        if (a is null || b is null) return a is null && b is null;
        if (IsNumeric(a) && IsNumeric(b))
            return IsFloating(a) || IsFloating(b)
                ? ToDouble(a, culture).Equals(ToDouble(b, culture))
                : ToDecimal(a, culture) == ToDecimal(b, culture);
        if (a is string sa && b is string sb) return string.Equals(sa, sb, StringComparison.Ordinal);
        if (a is string && IsNumeric(b) || IsNumeric(a) && b is string)
        {
            try { return ToDecimal(a, culture) == ToDecimal(b, culture); }
            catch (ExpressionException) { return false; }
        }
        return a.Equals(b);
    }

    /// <summary>Total ordering used for sorting and grouping. Nulls sort first.</summary>
    public static int Compare(object? a, object? b, CultureInfo culture)
    {
        if (a is null || b is null) return a is null ? (b is null ? 0 : -1) : 1;
        if (IsNumeric(a) && IsNumeric(b))
            return IsFloating(a) || IsFloating(b)
                ? ToDouble(a, culture).CompareTo(ToDouble(b, culture))
                : ToDecimal(a, culture).CompareTo(ToDecimal(b, culture));
        if (a is string sa && b is string sb) return string.Compare(sa, sb, culture, CompareOptions.None);
        if (a.GetType() == b.GetType() && a is IComparable ca) return ca.CompareTo(b);
        if (a is string && IsNumeric(b) || IsNumeric(a) && b is string)
        {
            try { return ToDecimal(a, culture).CompareTo(ToDecimal(b, culture)); }
            catch (ExpressionException) { }
        }
        return string.Compare(ToText(a, culture), ToText(b, culture), culture, CompareOptions.None);
    }
}

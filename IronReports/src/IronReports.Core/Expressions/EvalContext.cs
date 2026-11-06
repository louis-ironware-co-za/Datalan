using System.Globalization;
using IronReports.Data;

namespace IronReports.Expressions;

/// <summary>Everything an expression can see while it is evaluated.</summary>
public sealed class EvalContext
{
    private static readonly IReadOnlyList<IReportRow> NoRows = Array.Empty<IReportRow>();

    public EvalContext(CultureInfo? culture = null)
    {
        Culture = culture ?? CultureInfo.CurrentCulture;
        Parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        Now = DateTime.Now;
    }

    public CultureInfo Culture { get; set; }

    /// <summary>The current data row, or null when there is none.</summary>
    public IReportRow? Row { get; set; }

    /// <summary>Rows that aggregates (Sum, Count, ...) run over: the current group, or the whole report.</summary>
    public IReadOnlyList<IReportRow> ScopeRows { get; set; } = NoRows;

    /// <summary>All rows of the report after filtering and sorting.</summary>
    public IReadOnlyList<IReportRow> AllRows { get; set; } = NoRows;

    public Dictionary<string, object?> Parameters { get; private set; }

    public int PageNumber { get; set; }
    public int PageCount { get; set; }

    /// <summary>1-based row number within the current group.</summary>
    public int RowNumber { get; set; }

    /// <summary>1-based row number within the whole report.</summary>
    public int RecordNumber { get; set; }

    /// <summary>Fixed at the start of a run so every Now() on every page agrees.</summary>
    public DateTime Now { get; set; }

    public EvalContext Clone()
    {
        var c = (EvalContext)MemberwiseClone();
        c.Parameters = Parameters; // shared, read-only during a run
        return c;
    }
}

public sealed class ExpressionException : Exception
{
    public ExpressionException(string message, int position = -1) : base(position >= 0 ? $"{message} (at position {position + 1})" : message)
    {
        Position = position;
    }

    public int Position { get; }
}

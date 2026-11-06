using System.Drawing;
using System.Globalization;
using IronReports.Data;
using IronReports.Expressions;
using IronReports.Model;

namespace IronReports.Layout;

public sealed class ReportException : Exception
{
    public ReportException(string message, Exception? inner = null) : base(message, inner) { }
}

/// <summary>
/// Runs a report definition against data and produces fixed pages.
/// Order per page: page header, body bands, page footer. Report header prints first on page 1.
/// </summary>
public sealed class ReportEngine
{
    /// <summary>Safety limit so a broken layout cannot run forever.</summary>
    public int MaxPages { get; set; } = 20000;

    public RenderedDocument Run(ReportDefinition report, ReportDataSource? data = null, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(report);
        return new RunState(this, report, data, parameters).Execute();
    }

    /// <summary>Builds the evaluation context the engine would use (parameters, culture). Used by exporters.</summary>
    public static EvalContext CreateContext(ReportDefinition report, IReadOnlyDictionary<string, object?>? parameters)
    {
        var ctx = new EvalContext(ResolveCulture(report.Culture));
        foreach (var p in report.Parameters)
            ctx.Parameters[p.Name] = ParseParameter(p, ctx.Culture);
        if (parameters is not null)
            foreach (var kv in parameters)
                ctx.Parameters[kv.Key] = kv.Value;
        return ctx;
    }

    /// <summary>Applies the report filter and sorts rows by group expressions, then the sort expression.</summary>
    public static List<IReportRow> PrepareRows(ReportDefinition report, ReportDataSource? data, EvalContext ctx)
    {
        IReadOnlyList<IReportRow> source = data?.Rows ?? new IReportRow[] { EmptyRow.Instance };
        ctx.ScopeRows = source;
        ctx.AllRows = source;

        IEnumerable<IReportRow> rows = source;
        if (!string.IsNullOrWhiteSpace(report.FilterExpression))
        {
            var filter = CompileOrThrow(report.FilterExpression, "Filter");
            rows = rows.Where(r =>
            {
                ctx.Row = r;
                return Operators.ToBool(EvaluateOrThrow(filter, ctx, "Filter"));
            }).ToList();
        }

        var sortKeys = new List<(ReportExpression Expr, bool Descending)>();
        foreach (var g in report.Groups)
            if (!string.IsNullOrWhiteSpace(g.Expression)) sortKeys.Add((CompileOrThrow(g.Expression, "Group"), g.Descending));
        if (!string.IsNullOrWhiteSpace(report.SortExpression))
            sortKeys.Add((CompileOrThrow(report.SortExpression, "Sort"), report.SortDescending));

        var list = rows.ToList();
        if (sortKeys.Count == 0) return list;

        var keyed = list.Select(r =>
        {
            ctx.Row = r;
            return (Row: r, Keys: sortKeys.Select(k => EvaluateOrThrow(k.Expr, ctx, "Sort")).ToArray());
        }).ToList();

        var culture = ctx.Culture;
        var comparer = Comparer<object?[]>.Create((a, b) =>
        {
            for (var i = 0; i < a.Length; i++)
            {
                var c = Operators.Compare(a[i], b[i], culture);
                if (c != 0) return sortKeys[i].Descending ? -c : c;
            }
            return 0;
        });
        return keyed.OrderBy(k => k.Keys, comparer).Select(k => k.Row).ToList();
    }

    private static ReportExpression CompileOrThrow(string source, string what)
    {
        try { return ReportExpression.Compile(source); }
        catch (ExpressionException ex) { throw new ReportException($"{what} expression '{source}': {ex.Message}", ex); }
    }

    private static object? EvaluateOrThrow(ReportExpression e, EvalContext ctx, string what)
    {
        try { return e.Evaluate(ctx); }
        catch (Exception ex) when (ex is ExpressionException or InvalidCastException or FormatException or OverflowException)
        {
            throw new ReportException($"{what} expression '{e.Source}': {ex.Message}", ex);
        }
    }

    private static CultureInfo ResolveCulture(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return CultureInfo.CurrentCulture;
        try { return CultureInfo.GetCultureInfo(name); }
        catch (CultureNotFoundException) { return CultureInfo.CurrentCulture; }
    }

    private static object? ParseParameter(ReportParameter p, CultureInfo culture)
    {
        var text = p.DefaultValue ?? "";
        switch (p.Type)
        {
            case ParameterType.Number:
                return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
            case ParameterType.Date:
                return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : DateTime.Today;
            case ParameterType.Boolean:
                return Operators.ToBool(text);
            default:
                return text;
        }
    }

    private sealed class EmptyRow : IReportRow
    {
        public static readonly EmptyRow Instance = new();
        public object? GetValue(string field) => null;
    }

    private sealed record PreparedElement(ReportElement Element, float Height, string? Text, byte[]? ImageData, TextTemplate? Deferred);

    private sealed record PreparedBand(Band Band, float Height, List<PreparedElement> Elements);

    private sealed record DeferredText(RenderedPage Page, TextElement Element, RectangleF Box, TextTemplate Template, EvalContext Context);

    private sealed class RunState
    {
        private readonly ReportEngine _engine;
        private readonly ReportDefinition _report;
        private readonly ReportDataSource? _data;
        private readonly EvalContext _ctx;
        private readonly PageSetup _page;
        private readonly RenderedDocument _doc;
        private readonly List<DeferredText> _deferred = new();
        private List<IReportRow> _rows = new();
        private RenderedPage? _current;
        private float _y;
        private bool _pageHasBody;
        private IReportRow? _lastRow;
        private int _recordNumber;

        public RunState(ReportEngine engine, ReportDefinition report, ReportDataSource? data, IReadOnlyDictionary<string, object?>? parameters)
        {
            _engine = engine;
            _report = report;
            _data = data;
            _page = report.Page;
            _ctx = CreateContext(report, parameters);
            _doc = new RenderedDocument(_page.Width, _page.Height, report.Name);
        }

        private float BodyBottom => _page.Height - _page.MarginBottom - (_report.PageFooter.Visible ? _report.PageFooter.Height : 0f);

        public RenderedDocument Execute()
        {
            _rows = PrepareRows(_report, _data, _ctx);
            _ctx.AllRows = _rows;
            _ctx.ScopeRows = _rows;
            _lastRow = _rows.FirstOrDefault();

            StartPage(first: true);
            ProcessLevel(0, _rows);
            PrintBand(_report.ReportFooter, _rows.LastOrDefault(), _rows);
            FinishPage();

            _ctx.PageCount = _doc.Pages.Count;
            foreach (var d in _deferred)
            {
                d.Context.PageNumber = d.Page.Number;
                d.Context.PageCount = _doc.Pages.Count;
                var text = SafeEvaluate(d.Template, d.Element, d.Context);
                ElementRenderer.RenderText(d.Element, d.Box, text, d.Page.Items);
            }
            return _doc;
        }

        private void ProcessLevel(int level, List<IReportRow> rows)
        {
            var groups = _report.Groups;
            if (level >= groups.Count)
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    _ctx.RowNumber = i + 1;
                    _ctx.RecordNumber = ++_recordNumber;
                    _lastRow = rows[i];
                    PrintBand(_report.Detail, rows[i], rows);
                }
                return;
            }

            var group = groups[level];
            if (string.IsNullOrWhiteSpace(group.Expression))
            {
                // A group without an expression is one group over all rows.
                PrintGroup(level, group, rows);
                return;
            }

            var expr = CompileOrThrow(group.Expression, "Group");
            var start = 0;
            object? currentKey = null;
            for (var i = 0; i <= rows.Count; i++)
            {
                object? key = null;
                if (i < rows.Count)
                {
                    _ctx.Row = rows[i];
                    key = EvaluateOrThrow(expr, _ctx, "Group");
                }
                if (i == 0) { currentKey = key; continue; }
                if (i == rows.Count || !Operators.AreEqual(key, currentKey, _ctx.Culture))
                {
                    PrintGroup(level, group, rows.GetRange(start, i - start));
                    start = i;
                    currentKey = key;
                }
            }
        }

        private void PrintGroup(int level, GroupDefinition group, List<IReportRow> rows)
        {
            if (rows.Count == 0) return;
            PrintBand(group.Header, rows[0], rows);
            ProcessLevel(level + 1, rows);
            PrintBand(group.Footer, rows[^1], rows);
        }

        private void StartPage(bool first)
        {
            if (_doc.Pages.Count >= _engine.MaxPages)
                throw new ReportException($"The report exceeded {_engine.MaxPages} pages.");

            _current = new RenderedPage(_doc.Pages.Count + 1);
            _doc.Pages.Add(_current);
            _ctx.PageNumber = _current.Number;
            _y = _page.MarginTop;
            _pageHasBody = false;

            if (first) PrintBand(_report.ReportHeader, _rows.FirstOrDefault(), _rows);

            var header = _report.PageHeader;
            if (header.Visible)
            {
                var (row, scope) = (_ctx.Row, _ctx.ScopeRows);
                _ctx.Row = _lastRow;
                _ctx.ScopeRows = _rows;
                var prepared = Prepare(header);
                Place(prepared, _y);
                _y += prepared.Height;
                (_ctx.Row, _ctx.ScopeRows) = (row, scope);
            }
        }

        private void FinishPage()
        {
            var footer = _report.PageFooter;
            if (_current is null || !footer.Visible) return;
            var (row, scope) = (_ctx.Row, _ctx.ScopeRows);
            _ctx.Row = _lastRow;
            _ctx.ScopeRows = _rows;
            var prepared = Prepare(footer, allowGrow: false);
            Place(prepared, _page.Height - _page.MarginBottom - footer.Height);
            (_ctx.Row, _ctx.ScopeRows) = (row, scope);
        }

        private void PrintBand(Band band, IReportRow? row, IReadOnlyList<IReportRow> scope)
        {
            if (!band.Visible) return;
            _ctx.Row = row;
            _ctx.ScopeRows = scope;

            if (band.PageBreakBefore && _pageHasBody)
            {
                FinishPage();
                StartPage(first: false);
                _ctx.Row = row;
                _ctx.ScopeRows = scope;
            }

            var prepared = Prepare(band);
            if (prepared.Height <= 0f && prepared.Elements.Count == 0) return;

            if (_y + prepared.Height > BodyBottom + 0.01f && _pageHasBody)
            {
                FinishPage();
                StartPage(first: false);
                _ctx.Row = row;
                _ctx.ScopeRows = scope;
            }

            Place(prepared, _y);
            _y += prepared.Height;
            _pageHasBody = true;
        }

        private PreparedBand Prepare(Band band, bool allowGrow = true)
        {
            var list = new List<PreparedElement>();
            var height = band.Height;
            foreach (var e in band.Elements)
            {
                if (!IsVisible(e)) continue;
                var h = e.Height;
                string? text = null;
                byte[]? image = null;
                TextTemplate? deferred = null;

                switch (e)
                {
                    case TextElement t:
                        var template = ParseTemplate(t, out var parseError);
                        if (template is null)
                        {
                            text = parseError;
                        }
                        else if (template.UsesPageInfo)
                        {
                            deferred = template;
                        }
                        else
                        {
                            text = SafeEvaluate(template, t, _ctx);
                            if (t.CanGrow && allowGrow) h = Math.Max(h, ElementRenderer.MeasureTextHeight(t, text));
                        }
                        break;
                    case ImageElement img:
                        image = ResolveImage(img);
                        break;
                }

                list.Add(new PreparedElement(e, h, text, image, deferred));
                if (band.CanGrow && allowGrow) height = Math.Max(height, e.Y + h);
            }

            if (height > band.Height)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var p = list[i];
                    if (p.Element.StretchToBand && p.Element is not TextElement { CanGrow: true })
                        list[i] = p with { Height = Math.Max(p.Height, height - p.Element.Y) };
                }
            }
            return new PreparedBand(band, height, list);
        }

        private void Place(PreparedBand prepared, float top)
        {
            var page = _current!;
            var left = _page.MarginLeft;
            if (prepared.Band.BackColor.A > 0)
                page.Items.Add(new RectItem(new RectangleF(left, top, _page.PrintableWidth, prepared.Height), prepared.Band.BackColor, Color.Transparent, 0f));

            foreach (var p in prepared.Elements)
            {
                var e = p.Element;
                var box = new RectangleF(left + e.X, top + e.Y, e.Width, p.Height);
                switch (e)
                {
                    case TextElement t when p.Deferred is not null:
                        _deferred.Add(new DeferredText(page, t, box, p.Deferred, _ctx.Clone()));
                        break;
                    case TextElement t:
                        ElementRenderer.RenderText(t, box, p.Text ?? "", page.Items);
                        break;
                    case LineElement l:
                        ElementRenderer.RenderLine(l, box, page.Items);
                        break;
                    case ShapeElement s:
                        ElementRenderer.RenderShape(s, box, page.Items);
                        break;
                    case ImageElement i:
                        ElementRenderer.RenderImage(i, box, p.ImageData, page.Items);
                        break;
                }
            }
        }

        private bool IsVisible(ReportElement e)
        {
            if (string.IsNullOrWhiteSpace(e.VisibleWhen)) return true;
            try { return Operators.ToBool(ReportExpression.Compile(e.VisibleWhen).Evaluate(_ctx)); }
            catch (Exception ex) when (IsExpressionFailure(ex)) { return true; }
        }

        private static TextTemplate? ParseTemplate(TextElement t, out string error)
        {
            error = "";
            try { return TextTemplate.Parse(t.Text); }
            catch (ExpressionException ex)
            {
                error = "#ERR " + ex.Message;
                return null;
            }
        }

        private static string SafeEvaluate(TextTemplate template, TextElement t, EvalContext ctx)
        {
            try { return template.Evaluate(ctx, t.Format); }
            catch (Exception ex) when (IsExpressionFailure(ex)) { return "#ERR " + ex.Message; }
        }

        private byte[]? ResolveImage(ImageElement img)
        {
            if (string.IsNullOrWhiteSpace(img.ImageExpression)) return img.ImageData;
            try
            {
                return ReportExpression.Compile(img.ImageExpression).Evaluate(_ctx) switch
                {
                    byte[] b => b,
                    string s when s.Length > 0 => Convert.FromBase64String(s),
                    _ => img.ImageData
                };
            }
            catch (Exception ex) when (IsExpressionFailure(ex))
            {
                return img.ImageData;
            }
        }

        private static bool IsExpressionFailure(Exception ex) =>
            ex is ExpressionException or InvalidCastException or FormatException or OverflowException or ArgumentException;
    }
}

using System.Drawing;
using System.Drawing.Drawing2D;
using IronReports.Layout;
using IronReports.Model;
using IronReports.Serialization;

namespace IronReports.WinForms.Designer;

public enum DesignTool { Pointer, Text, Line, Rectangle, Ellipse, Image }

/// <summary>
/// The drawing canvas of the designer: bands stacked vertically, each with a caption strip.
/// Handles selection, moving, resizing, band resizing, rubber-band selection, element creation and field drops.
/// </summary>
public sealed class DesignSurface : ScrollableControl
{
    public const string FieldDragFormat = "IronReportsField";
    private const string ClipboardFormat = "IronReportsElements";
    private const int Gutter = 24;
    private const int TopGap = 10;
    private const int CaptionHeight = 20;
    private const int HandleSize = 7;
    private const int EdgeGrip = 4;

    private ReportDefinition _report = new();
    private readonly List<ReportElement> _selection = new();
    private readonly List<BandLayout> _layout = new();
    private Band? _selectedBand;
    private float _zoom = 1f;

    private DragKind _drag = DragKind.None;
    private Point _dragStart;
    private Point _dragCurrent;
    private int _dragHandle;
    private Band? _dragBand;
    private float _dragBandHeight;
    private List<(ReportElement Element, RectangleF Bounds)> _dragOriginals = new();
    private bool _dragMoved;

    private enum DragKind { None, Move, Resize, BandResize, RubberBand, Create }

    private sealed record BandLayout(Band Band, Rectangle Caption, RectangleF Area);

    public DesignSurface()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        AutoScroll = true;
        AllowDrop = true;
        BackColor = Color.FromArgb(160, 160, 160);
        SetStyle(ControlStyles.Selectable | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    // ---------------- Public state ----------------

    public ReportDefinition Report
    {
        get => _report;
        set
        {
            _report = value ?? new ReportDefinition();
            _selection.Clear();
            _selectedBand = null;
            RefreshLayout();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public IReadOnlyList<ReportElement> Selection => _selection;

    /// <summary>The band whose caption or empty area was clicked last, or the band of the primary selection.</summary>
    public Band? SelectedBand => _selection.Count > 0 ? _report.FindBandOf(_selection[0]) : _selectedBand;

    /// <summary>True when the user selected a band itself (not an element).</summary>
    public bool IsBandSelected => _selection.Count == 0 && _selectedBand is not null;

    public DesignTool Tool { get; set; } = DesignTool.Pointer;

    public float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = Math.Clamp(value, 0.25f, 4f);
            RefreshLayout();
        }
    }

    public float GridSize { get; set; } = 4f;
    public bool SnapToGrid { get; set; } = true;
    public bool ShowGrid { get; set; } = true;

    /// <summary>Raised after a user action changed the report (commit point for undo).</summary>
    public event EventHandler? Changed;

    public event EventHandler? SelectionChanged;

    /// <summary>Raised after an element was created with a tool, so the toolbar can return to the pointer.</summary>
    public event EventHandler? ToolFinished;

    /// <summary>Raised when the user double-clicks an element.</summary>
    public event EventHandler<ReportElement>? ElementActivated;

    /// <summary>Raised on right-click with the screen location for a context menu.</summary>
    public event EventHandler<Point>? ContextMenuRequested;

    private float PixelsPerPoint => _zoom * DeviceDpi / 72f;

    // ---------------- Selection API ----------------

    public void Select(IEnumerable<ReportElement> elements, bool add = false)
    {
        if (!add) _selection.Clear();
        foreach (var e in elements)
            if (!_selection.Contains(e)) _selection.Add(e);
        if (_selection.Count > 0) _selectedBand = _report.FindBandOf(_selection[0]);
        Invalidate();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SelectBand(Band? band)
    {
        _selection.Clear();
        _selectedBand = band;
        Invalidate();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearSelection() => SelectBand(null);

    public void SelectAllInBand()
    {
        var band = SelectedBand ?? _report.Detail;
        Select(band.Elements);
    }

    // ---------------- Editing commands ----------------

    public void DeleteSelection()
    {
        if (_selection.Count == 0) return;
        foreach (var e in _selection.ToList()) _report.FindBandOf(e)?.Elements.Remove(e);
        var band = _selectedBand;
        _selection.Clear();
        _selectedBand = band;
        RaiseChanged();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CopySelection()
    {
        if (_selection.Count == 0) return;
        var xml = ReportSerializer.SaveElements(_selection);
        var data = new DataObject();
        data.SetData(ClipboardFormat, xml);
        data.SetText(xml);
        Clipboard.SetDataObject(data, copy: true);
    }

    public void CutSelection()
    {
        CopySelection();
        DeleteSelection();
    }

    public bool CanPaste => Clipboard.ContainsData(ClipboardFormat);

    public void Paste()
    {
        if (!Clipboard.ContainsData(ClipboardFormat) || Clipboard.GetData(ClipboardFormat) is not string xml) return;
        List<ReportElement> elements;
        try { elements = ReportSerializer.LoadElements(xml); }
        catch (Exception ex) when (ex is FormatException or System.Xml.XmlException) { return; }
        if (elements.Count == 0) return;

        var band = SelectedBand ?? _report.Detail;
        foreach (var e in elements)
        {
            // Offset when pasting onto an identical position.
            while (band.Elements.Any(x => Math.Abs(x.X - e.X) < 0.5f && Math.Abs(x.Y - e.Y) < 0.5f))
            {
                e.X += 8;
                e.Y += 8;
            }
            e.Name = _report.CreateUniqueName(e.TypeName);
            band.Elements.Add(e);
            band.Height = Math.Max(band.Height, e.Y + e.Height);
        }
        Select(elements);
        RaiseChanged();
    }

    public void BringSelectionToFront() => Reorder(toFront: true);

    public void SendSelectionToBack() => Reorder(toFront: false);

    private void Reorder(bool toFront)
    {
        foreach (var e in _selection)
        {
            var band = _report.FindBandOf(e);
            if (band is null) continue;
            band.Elements.Remove(e);
            if (toFront) band.Elements.Add(e);
            else band.Elements.Insert(0, e);
        }
        RaiseChanged();
    }

    public enum Alignment { Left, Right, Top, Bottom, CenterHorizontal, SameWidth, SameHeight, SameSize }

    /// <summary>Aligns the selection to the first selected element.</summary>
    public void Align(Alignment how)
    {
        if (_selection.Count < 2) return;
        var anchor = _selection[0];
        foreach (var e in _selection.Skip(1))
        {
            switch (how)
            {
                case Alignment.Left: e.X = anchor.X; break;
                case Alignment.Right: e.X = anchor.X + anchor.Width - e.Width; break;
                case Alignment.Top: e.Y = anchor.Y; break;
                case Alignment.Bottom: e.Y = anchor.Y + anchor.Height - e.Height; break;
                case Alignment.CenterHorizontal: e.X = anchor.X + (anchor.Width - e.Width) / 2f; break;
                case Alignment.SameWidth: e.Width = anchor.Width; break;
                case Alignment.SameHeight: e.Height = anchor.Height; break;
                case Alignment.SameSize: e.Width = anchor.Width; e.Height = anchor.Height; break;
            }
        }
        RaiseChanged();
    }

    /// <summary>Adds a text element for a data field to the selected band (or the detail band).</summary>
    public void InsertField(string field, PointF? bandPoint = null, Band? band = null)
    {
        band ??= SelectedBand ?? _report.Detail;
        var p = bandPoint ?? NextFreeSpot(band);
        var e = new TextElement
        {
            Name = _report.CreateUniqueName("text"),
            Text = "{[" + field + "]}",
            X = Snap(p.X),
            Y = Snap(p.Y),
            Width = 100,
            Height = 18,
            ExportCaption = field
        };
        e.X = Math.Clamp(e.X, 0, Math.Max(0, _report.Page.PrintableWidth - e.Width));
        band.Elements.Add(e);
        band.Height = Math.Max(band.Height, e.Y + e.Height);
        Select(new[] { e });
        RaiseChanged();
    }

    private PointF NextFreeSpot(Band band)
    {
        var right = band.Elements.Where(e => e.Y < 18).Select(e => e.X + e.Width).DefaultIfEmpty(0).Max();
        return right + 100 <= _report.Page.PrintableWidth ? new PointF(right, 0) : new PointF(0, 0);
    }

    /// <summary>Call after the report changed outside the surface (property grid, undo).</summary>
    public void RefreshLayout()
    {
        BuildLayout();
        Invalidate();
    }

    private void RaiseChanged()
    {
        BuildLayout();
        Invalidate();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    // ---------------- Geometry ----------------

    private void BuildLayout()
    {
        _layout.Clear();
        var s = PixelsPerPoint;
        var width = _report.Page.PrintableWidth * s;
        float y = TopGap;
        foreach (var band in _report.BandsInDesignOrder())
        {
            var caption = new Rectangle(Gutter, (int)y, (int)Math.Ceiling(width), CaptionHeight);
            y += CaptionHeight;
            var area = new RectangleF(Gutter, y, width, Math.Max(band.Height * s, 1f));
            _layout.Add(new BandLayout(band, caption, area));
            y += area.Height + 1;
        }
        AutoScrollMinSize = new Size((int)(Gutter * 2 + width + 40), (int)y + 60);
    }

    private BandLayout? LayoutOf(Band band) => _layout.FirstOrDefault(l => l.Band == band);

    private RectangleF ElementRect(ReportElement e, BandLayout bl)
    {
        var s = PixelsPerPoint;
        return new RectangleF(bl.Area.X + e.X * s, bl.Area.Y + e.Y * s, Math.Max(e.Width * s, 1f), Math.Max(e.Height * s, 1f));
    }

    private RectangleF? ElementRect(ReportElement e)
    {
        var band = _report.FindBandOf(e);
        var bl = band is null ? null : LayoutOf(band);
        return bl is null ? null : ElementRect(e, bl);
    }

    private static RectangleF[] Handles(RectangleF r)
    {
        float h = HandleSize, h2 = HandleSize / 2f;
        float l = r.Left - h2, t = r.Top - h2, cx = r.Left + r.Width / 2 - h2, cy = r.Top + r.Height / 2 - h2, rr = r.Right - h2, b = r.Bottom - h2;
        return new[]
        {
            new RectangleF(l, t, h, h), new RectangleF(cx, t, h, h), new RectangleF(rr, t, h, h), new RectangleF(rr, cy, h, h),
            new RectangleF(rr, b, h, h), new RectangleF(cx, b, h, h), new RectangleF(l, b, h, h), new RectangleF(l, cy, h, h)
        };
    }

    private static readonly Cursor[] HandleCursors =
    {
        Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE,
        Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE
    };

    private Point ToContent(Point client) => new(client.X - AutoScrollPosition.X, client.Y - AutoScrollPosition.Y);

    private float Snap(float value) => SnapToGrid && (ModifierKeys & Keys.Alt) == 0 && GridSize > 0
        ? (float)Math.Round(value / GridSize) * GridSize
        : (float)Math.Round(value, 1);

    private BandLayout? BandAreaAt(Point p) => _layout.FirstOrDefault(l => p.Y >= l.Area.Top && p.Y < l.Area.Bottom + 1 && p.X >= 0);

    private BandLayout? BandEdgeAt(Point p) =>
        _layout.FirstOrDefault(l => Math.Abs(p.Y - l.Area.Bottom) <= EdgeGrip && p.X >= l.Area.Left && p.X <= l.Area.Right + 20);

    private ReportElement? ElementAt(Point p)
    {
        foreach (var bl in _layout)
        {
            if (p.Y < bl.Area.Top - HandleSize || p.Y > bl.Area.Bottom + HandleSize) continue;
            for (var i = bl.Band.Elements.Count - 1; i >= 0; i--)
            {
                var e = bl.Band.Elements[i];
                var r = ElementRect(e, bl);
                if (r.Height < 4) r.Inflate(0, 3); // thin lines are easier to grab
                if (r.Width < 4) r.Inflate(3, 0);
                if (r.Contains(p)) return e;
            }
        }
        return null;
    }

    private int HandleAt(Point p)
    {
        if (_selection.Count != 1) return -1;
        var r = ElementRect(_selection[0]);
        if (r is null) return -1;
        var handles = Handles(r.Value);
        for (var i = 0; i < handles.Length; i++)
        {
            var hr = handles[i];
            hr.Inflate(2, 2);
            if (hr.Contains(p)) return i;
        }
        return -1;
    }

    // ---------------- Mouse ----------------

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();
        var p = ToContent(e.Location);

        if (e.Button == MouseButtons.Right)
        {
            var hit = ElementAt(p);
            if (hit is not null && !_selection.Contains(hit)) Select(new[] { hit });
            else if (hit is null && BandAreaAt(p) is { } bl && _selection.Count == 0) SelectBand(bl.Band);
            ContextMenuRequested?.Invoke(this, PointToScreen(e.Location));
            return;
        }
        if (e.Button != MouseButtons.Left) return;

        _dragStart = _dragCurrent = p;
        _dragMoved = false;

        if (Tool != DesignTool.Pointer)
        {
            var target = BandAreaAt(p);
            if (target is null) return;
            _drag = DragKind.Create;
            _dragBand = target.Band;
            Capture = true;
            return;
        }

        var handle = HandleAt(p);
        if (handle >= 0)
        {
            _drag = DragKind.Resize;
            _dragHandle = handle;
            _dragOriginals = _selection.Select(x => (x, x.Bounds)).ToList();
            Capture = true;
            return;
        }

        var element = ElementAt(p);
        if (element is not null)
        {
            var additive = (ModifierKeys & (Keys.Control | Keys.Shift)) != 0;
            if (additive)
            {
                if (_selection.Contains(element)) _selection.Remove(element);
                else _selection.Add(element);
                Invalidate();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
            if (!_selection.Contains(element)) Select(new[] { element });
            else if (_selection[0] != element)
            {
                // Make the clicked element the primary (alignment anchor).
                _selection.Remove(element);
                _selection.Insert(0, element);
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
            _drag = DragKind.Move;
            _dragOriginals = _selection.Select(x => (x, x.Bounds)).ToList();
            Capture = true;
            return;
        }

        var edge = BandEdgeAt(p);
        if (edge is not null)
        {
            _drag = DragKind.BandResize;
            _dragBand = edge.Band;
            _dragBandHeight = edge.Band.Height;
            Capture = true;
            return;
        }

        var caption = _layout.FirstOrDefault(l => l.Caption.Contains(p));
        if (caption is not null)
        {
            SelectBand(caption.Band);
            return;
        }

        var area = BandAreaAt(p);
        if (area is not null && p.X >= area.Area.Left && p.X <= area.Area.Right)
        {
            if ((ModifierKeys & Keys.Control) == 0) SelectBand(area.Band);
            _drag = DragKind.RubberBand;
            Capture = true;
            return;
        }

        ClearSelection();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var p = ToContent(e.Location);
        var s = PixelsPerPoint;

        switch (_drag)
        {
            case DragKind.None:
                UpdateCursor(p);
                return;

            case DragKind.Move:
            {
                var dx = (p.X - _dragStart.X) / s;
                var dy = (p.Y - _dragStart.Y) / s;
                if (!PastDragThreshold(p)) return;
                _dragMoved = true;
                var anchor = _dragOriginals[0].Bounds;
                var sx = Snap(anchor.X + dx) - anchor.X;
                var sy = Snap(anchor.Y + dy) - anchor.Y;
                var maxX = _report.Page.PrintableWidth;
                foreach (var (el, b) in _dragOriginals)
                {
                    el.X = Math.Clamp(b.X + sx, 0, Math.Max(0, maxX - Math.Min(el.Width, maxX)));
                    el.Y = b.Y + sy;
                }
                Invalidate();
                return;
            }

            case DragKind.Resize:
            {
                if (!PastDragThreshold(p)) return;
                _dragMoved = true;
                var (el, b) = _dragOriginals[0];
                var dx = (p.X - _dragStart.X) / s;
                var dy = (p.Y - _dragStart.Y) / s;
                float left = b.Left, top = b.Top, right = b.Right, bottom = b.Bottom;
                var h = _dragHandle;
                if (h is 0 or 6 or 7) left = Math.Min(Snap(b.Left + dx), right - 2);
                if (h is 2 or 3 or 4) right = Math.Max(Snap(b.Right + dx), left + 2);
                if (h is 0 or 1 or 2) top = Math.Min(Snap(b.Top + dy), bottom - 2);
                if (h is 4 or 5 or 6) bottom = Math.Max(Snap(b.Bottom + dy), top + 2);
                left = Math.Max(0, left);
                top = Math.Max(0, top);
                el.Bounds = RectangleF.FromLTRB(left, top, right, bottom);
                Invalidate();
                return;
            }

            case DragKind.BandResize:
            {
                if (!PastDragThreshold(p)) return;
                _dragMoved = true;
                var band = _dragBand!;
                var minHeight = band.Elements.Select(x => x.Y + x.Height).DefaultIfEmpty(0).Max();
                band.Height = Math.Max(Math.Max(0, minHeight), Snap(_dragBandHeight + (p.Y - _dragStart.Y) / s));
                RefreshLayout();
                return;
            }

            case DragKind.RubberBand:
            case DragKind.Create:
                _dragCurrent = p;
                _dragMoved = true;
                Invalidate();
                return;
        }
    }

    /// <summary>
    /// A click is not a drag. Moving, resizing and band resizing only start once the mouse has
    /// travelled a few pixels, so a plain click never snaps or changes anything.
    /// </summary>
    private bool PastDragThreshold(Point p) =>
        _dragMoved || Math.Abs(p.X - _dragStart.X) >= 3 || Math.Abs(p.Y - _dragStart.Y) >= 3;

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button != MouseButtons.Left) return;
        Capture = false;
        var drag = _drag;
        _drag = DragKind.None;

        switch (drag)
        {
            case DragKind.Move when _dragMoved:
                MoveAcrossBands();
                RaiseChanged();
                break;
            case DragKind.Resize when _dragMoved:
            case DragKind.BandResize when _dragMoved:
                RaiseChanged();
                break;
            case DragKind.RubberBand:
                if (_dragMoved)
                {
                    var rect = Normalize(_dragStart, _dragCurrent);
                    var hits = _layout.SelectMany(bl => bl.Band.Elements.Where(x => ElementRect(x, bl).IntersectsWith(rect))).ToList();
                    Select(hits, add: (ModifierKeys & Keys.Control) != 0);
                }
                Invalidate();
                break;
            case DragKind.Create:
                CreateFromDrag();
                break;
        }
        UpdateCursor(ToContent(e.Location));
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);
        if (e.Button != MouseButtons.Left || Tool != DesignTool.Pointer) return;
        var element = ElementAt(ToContent(e.Location));
        if (element is not null) ElementActivated?.Invoke(this, element);
    }

    private void UpdateCursor(Point p)
    {
        if (Tool != DesignTool.Pointer) { Cursor = Cursors.Cross; return; }
        var handle = HandleAt(p);
        if (handle >= 0) { Cursor = HandleCursors[handle]; return; }
        if (ElementAt(p) is not null) { Cursor = Cursors.SizeAll; return; }
        Cursor = BandEdgeAt(p) is not null ? Cursors.HSplit : Cursors.Default;
    }

    private void MoveAcrossBands()
    {
        var s = PixelsPerPoint;
        foreach (var (el, _) in _dragOriginals)
        {
            var source = _report.FindBandOf(el);
            var sourceLayout = source is null ? null : LayoutOf(source);
            if (source is null || sourceLayout is null) continue;
            var absTop = sourceLayout.Area.Y + el.Y * s;
            var target = _layout.FirstOrDefault(l => absTop >= l.Caption.Top && absTop < l.Area.Bottom) ??
                         (absTop < _layout[0].Caption.Top ? _layout[0] : _layout[^1]);
            if (target.Band != source)
            {
                source.Elements.Remove(el);
                target.Band.Elements.Add(el);
                el.Y = Snap(Math.Max(0, (absTop - target.Area.Y) / s));
            }
            el.Y = Math.Max(0, el.Y);
            target.Band.Height = Math.Max(target.Band.Height, el.Y + el.Height);
        }
        _selectedBand = _selection.Count > 0 ? _report.FindBandOf(_selection[0]) : _selectedBand;
    }

    private void CreateFromDrag()
    {
        var bl = _dragBand is null ? null : LayoutOf(_dragBand);
        if (bl is null) return;
        var s = PixelsPerPoint;
        var r = Normalize(_dragStart, _dragCurrent);
        var x = Snap(Math.Max(0, (r.X - bl.Area.X) / s));
        var y = Snap(Math.Max(0, (r.Y - bl.Area.Y) / s));
        var w = Snap(r.Width / s);
        var h = Snap(r.Height / s);
        var clicked = r.Width < 4 && r.Height < 4;

        ReportElement element = Tool switch
        {
            DesignTool.Text => new TextElement { Text = "Text" },
            DesignTool.Line => new LineElement(),
            DesignTool.Rectangle => new ShapeElement { Shape = ShapeKind.Rectangle },
            DesignTool.Ellipse => new ShapeElement { Shape = ShapeKind.Ellipse },
            _ => new ImageElement()
        };
        element.Name = _report.CreateUniqueName(element.TypeName);
        element.X = x;
        element.Y = y;
        if (!clicked)
        {
            element.Width = Math.Max(4, w);
            element.Height = Math.Max(Tool == DesignTool.Line ? 2 : 4, h);
            if (element is LineElement line && h > w * 3) line.Direction = LineDirection.Vertical;
        }
        element.Width = Math.Min(element.Width, Math.Max(4, _report.Page.PrintableWidth - element.X));
        bl.Band.Elements.Add(element);
        bl.Band.Height = Math.Max(bl.Band.Height, element.Y + element.Height);

        if ((ModifierKeys & Keys.Shift) == 0)
        {
            Tool = DesignTool.Pointer;
            ToolFinished?.Invoke(this, EventArgs.Empty);
        }
        Select(new[] { element });
        RaiseChanged();
    }

    private static Rectangle Normalize(Point a, Point b) =>
        Rectangle.FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    // ---------------- Keyboard ----------------

    protected override bool IsInputKey(Keys keyData)
    {
        var key = keyData & Keys.KeyCode;
        return key is Keys.Left or Keys.Right or Keys.Up or Keys.Down || base.IsInputKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Escape)
        {
            if (_drag != DragKind.None)
            {
                foreach (var (el, b) in _dragOriginals) el.Bounds = b;
                if (_drag == DragKind.BandResize && _dragBand is not null) _dragBand.Height = _dragBandHeight;
                _drag = DragKind.None;
                Capture = false;
                RefreshLayout();
            }
            else if (Tool != DesignTool.Pointer)
            {
                Tool = DesignTool.Pointer;
                ToolFinished?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ClearSelection();
            }
            e.Handled = true;
            return;
        }

        if (_selection.Count == 0) return;
        float dx = 0, dy = 0;
        switch (e.KeyCode)
        {
            case Keys.Left: dx = -1; break;
            case Keys.Right: dx = 1; break;
            case Keys.Up: dy = -1; break;
            case Keys.Down: dy = 1; break;
            case Keys.Delete:
                DeleteSelection();
                e.Handled = true;
                return;
            default: return;
        }
        var step = e.Control || !SnapToGrid ? 1f : GridSize;
        foreach (var el in _selection)
        {
            if (e.Shift)
            {
                el.Width = Math.Max(2, el.Width + dx * step);
                el.Height = Math.Max(2, el.Height + dy * step);
            }
            else
            {
                el.X = Math.Max(0, el.X + dx * step);
                el.Y = Math.Max(0, el.Y + dy * step);
            }
            var band = _report.FindBandOf(el);
            if (band is not null) band.Height = Math.Max(band.Height, el.Y + el.Height);
        }
        e.Handled = true;
        RaiseChanged();
    }

    // ---------------- Drag and drop of fields ----------------

    protected override void OnDragOver(DragEventArgs e)
    {
        base.OnDragOver(e);
        var p = ToContent(PointToClient(new Point(e.X, e.Y)));
        e.Effect = e.Data?.GetDataPresent(FieldDragFormat) == true && BandAreaAt(p) is not null ? DragDropEffects.Copy : DragDropEffects.None;
    }

    protected override void OnDragDrop(DragEventArgs e)
    {
        base.OnDragDrop(e);
        if (e.Data?.GetData(FieldDragFormat) is not string field) return;
        var p = ToContent(PointToClient(new Point(e.X, e.Y)));
        var bl = BandAreaAt(p);
        if (bl is null) return;
        var s = PixelsPerPoint;
        InsertField(field, new PointF((p.X - bl.Area.X) / s, (p.Y - bl.Area.Y) / s), bl.Band);
    }

    // ---------------- Painting ----------------

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_layout.Count == 0) BuildLayout();
        var g = e.Graphics;
        g.Clear(BackColor);
        g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
        var s = PixelsPerPoint;

        using var captionFont = new Font("Segoe UI", 8f);
        using var outlinePen = new Pen(Color.FromArgb(200, 200, 200)) { DashStyle = DashStyle.Dot };
        using var selectionPen = new Pen(Color.FromArgb(0, 120, 215), 1f);
        using var gridBrush = new SolidBrush(Color.FromArgb(205, 205, 205));

        foreach (var bl in _layout)
        {
            var band = bl.Band;
            var bandSelected = IsBandSelected && _selectedBand == band;

            // Caption strip
            var captionColor = bandSelected ? Color.FromArgb(204, 228, 247) : Color.FromArgb(232, 232, 232);
            using (var cb = new SolidBrush(captionColor)) g.FillRectangle(cb, bl.Caption);
            var label = band.DisplayName + (band.Visible ? "" : "  (hidden)");
            TextRenderer.DrawText(g, label, captionFont, new Rectangle(bl.Caption.X + 6, bl.Caption.Y, bl.Caption.Width - 12, bl.Caption.Height),
                Color.FromArgb(60, 60, 60), TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            // Band area
            g.FillRectangle(Brushes.White, bl.Area);
            if (band.BackColor.A > 0)
                using (var bb = new SolidBrush(band.BackColor)) g.FillRectangle(bb, bl.Area);

            if (ShowGrid && GridSize * s >= 5)
            {
                var step = GridSize * s;
                var every = step < 8 ? 2 : 1;
                for (var gy = bl.Area.Top; gy < bl.Area.Bottom; gy += step * every)
                for (var gx = bl.Area.Left; gx < bl.Area.Right; gx += step * every)
                    g.FillRectangle(gridBrush, gx, gy, 1, 1);
            }

            // Elements
            var state = g.Save();
            g.SetClip(RectangleF.Inflate(bl.Area, 0, 0));
            g.TranslateTransform(bl.Area.X, bl.Area.Y);
            g.ScaleTransform(s, s);
            foreach (var el in band.Elements)
            {
                var items = ElementRenderer.RenderForDesign(el, el.Bounds);
                GdiRenderer.DrawItems(g, items, 1f / s);
                if (el is ImageElement { ImageData: null } img) DrawImagePlaceholder(g, img, s);
            }
            g.Restore(state);

            foreach (var el in band.Elements)
            {
                var r = ElementRect(el, bl);
                if (!string.IsNullOrWhiteSpace(el.VisibleWhen))
                    using (var vp = new Pen(Color.FromArgb(230, 160, 0)) { DashStyle = DashStyle.Dash })
                        g.DrawRectangle(vp, r.X, r.Y, r.Width, r.Height);
                else
                    g.DrawRectangle(outlinePen, r.X, r.Y, r.Width, r.Height);
            }

            using (var edge = new Pen(Color.FromArgb(150, 150, 150)))
                g.DrawRectangle(edge, bl.Area.X, bl.Area.Y, bl.Area.Width, bl.Area.Height);
        }

        // Selection
        foreach (var el in _selection)
        {
            var r = ElementRect(el);
            if (r is null) continue;
            g.DrawRectangle(selectionPen, r.Value.X, r.Value.Y, r.Value.Width, r.Value.Height);
            if (_selection.Count == 1)
            {
                foreach (var h in Handles(r.Value))
                {
                    g.FillRectangle(Brushes.White, h);
                    g.DrawRectangle(selectionPen, h.X, h.Y, h.Width, h.Height);
                }
            }
            else if (el == _selection[0])
            {
                using var primary = new SolidBrush(Color.FromArgb(0, 120, 215));
                g.FillRectangle(primary, r.Value.X - 3, r.Value.Y - 3, 6, 6);
            }
        }

        // Rubber band and creation preview
        if (_drag is DragKind.RubberBand or DragKind.Create && _dragMoved)
        {
            using var pen = new Pen(Color.FromArgb(0, 120, 215)) { DashStyle = DashStyle.Dash };
            var rect = Normalize(_dragStart, _dragCurrent);
            using (var fill = new SolidBrush(Color.FromArgb(30, 0, 120, 215))) g.FillRectangle(fill, rect);
            g.DrawRectangle(pen, rect);
        }
    }

    private static void DrawImagePlaceholder(Graphics g, ImageElement img, float s)
    {
        using var pen = new Pen(Color.Silver, 1f / s);
        var r = img.Bounds;
        g.DrawLine(pen, r.Left, r.Top, r.Right, r.Bottom);
        g.DrawLine(pen, r.Right, r.Top, r.Left, r.Bottom);
    }

    protected override void OnScroll(ScrollEventArgs se)
    {
        base.OnScroll(se);
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if ((ModifierKeys & Keys.Control) != 0)
        {
            Zoom = (float)Math.Round(_zoom * (e.Delta > 0 ? 1.25f : 0.8f), 2);
            ZoomChanged?.Invoke(this, EventArgs.Empty);
            return;
        }
        base.OnMouseWheel(e);
        Invalidate();
    }

    public event EventHandler? ZoomChanged;

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        RefreshLayout();
    }
}

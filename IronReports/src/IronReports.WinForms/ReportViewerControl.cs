using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using IronReports.Data;
using IronReports.Export;
using IronReports.Layout;
using IronReports.Model;

namespace IronReports.WinForms;

public enum ViewerZoomMode { Percent, PageWidth, WholePage }

/// <summary>
/// Shows a rendered report with page navigation, zoom, printing and export to PDF, Excel and CSV.
/// Call <see cref="ShowReport"/> to run a report, or set <see cref="Document"/> directly.
/// </summary>
public class ReportViewerControl : UserControl
{
    private readonly ToolStrip _toolbar = new() { GripStyle = ToolStripGripStyle.Hidden, Padding = new Padding(4, 2, 4, 2) };
    private readonly PageCanvas _canvas;
    private readonly ToolStripTextBox _pageBox = new() { AutoSize = false, Width = 44, TextBoxTextAlign = HorizontalAlignment.Center };
    private readonly ToolStripLabel _pageCount = new("of 0");
    private readonly ToolStripComboBox _zoomBox = new() { AutoSize = false, Width = 90 };
    private readonly ToolStripButton _printButton;
    private readonly ToolStripDropDownButton _exportButton;
    private readonly ToolStripMenuItem _exportExcel;
    private readonly ToolStripMenuItem _exportCsv;
    private readonly ToolStripLabel _statusLabel = new() { Alignment = ToolStripItemAlignment.Right, ForeColor = SystemColors.GrayText };
    private ReportDefinition? _report;
    private ReportDataSource? _data;
    private IReadOnlyDictionary<string, object?>? _parameters;

    private static readonly int[] ZoomSteps = { 25, 50, 75, 100, 125, 150, 200, 300, 400 };

    public ReportViewerControl()
    {
        _canvas = new PageCanvas(this) { Dock = DockStyle.Fill };

        _toolbar.Items.Add(Button("|◀", "First page", (_, _) => GoToPage(1)));
        _toolbar.Items.Add(Button("◀", "Previous page", (_, _) => GoToPage(CurrentPage - 1)));
        _toolbar.Items.Add(_pageBox);
        _toolbar.Items.Add(_pageCount);
        _toolbar.Items.Add(Button("▶", "Next page", (_, _) => GoToPage(CurrentPage + 1)));
        _toolbar.Items.Add(Button("▶|", "Last page", (_, _) => GoToPage(PageCount)));
        _toolbar.Items.Add(new ToolStripSeparator());
        _toolbar.Items.Add(Button("−", "Zoom out", (_, _) => ZoomStep(-1)));
        _toolbar.Items.Add(_zoomBox);
        _toolbar.Items.Add(Button("+", "Zoom in", (_, _) => ZoomStep(1)));
        _toolbar.Items.Add(new ToolStripSeparator());
        _printButton = Button("Print…", "Print the report", (_, _) => PrintReport());
        _toolbar.Items.Add(_printButton);

        _exportButton = new ToolStripDropDownButton("Export") { ToolTipText = "Save the report as a file" };
        _exportButton.DropDownItems.Add(new ToolStripMenuItem("PDF document…", null, (_, _) => ExportPdf()));
        _exportExcel = new ToolStripMenuItem("Excel workbook (detail rows)…", null, (_, _) => ExportDetailRows(excel: true));
        _exportCsv = new ToolStripMenuItem("CSV file (detail rows)…", null, (_, _) => ExportDetailRows(excel: false));
        _exportButton.DropDownItems.Add(_exportExcel);
        _exportButton.DropDownItems.Add(_exportCsv);
        _toolbar.Items.Add(_exportButton);
        _toolbar.Items.Add(_statusLabel);

        _zoomBox.Items.AddRange(new object[] { "Page width", "Whole page" });
        foreach (var z in ZoomSteps) _zoomBox.Items.Add($"{z}%");
        _zoomBox.SelectedIndex = 0;
        _zoomBox.SelectedIndexChanged += (_, _) => ApplyZoomText(_zoomBox.Text);
        _zoomBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            ApplyZoomText(_zoomBox.Text);
            e.SuppressKeyPress = true;
        };
        _pageBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            if (int.TryParse(_pageBox.Text, out var n)) GoToPage(n);
            e.SuppressKeyPress = true;
        };

        Controls.Add(_canvas);
        Controls.Add(_toolbar);
        UpdateUi();
    }

    private static ToolStripButton Button(string text, string tip, EventHandler click) =>
        new(text, null, click) { ToolTipText = tip, DisplayStyle = ToolStripItemDisplayStyle.Text };

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RenderedDocument? Document
    {
        get => _canvas.Document;
        set
        {
            _canvas.Document = value;
            _canvas.UpdateLayout(resetScroll: true);
            UpdateUi();
        }
    }

    [Browsable(false)]
    public int PageCount => Document?.Pages.Count ?? 0;

    [Browsable(false)]
    public int CurrentPage => _canvas.CurrentPageIndex + 1;

    [DefaultValue(ViewerZoomMode.PageWidth)]
    public ViewerZoomMode ZoomMode
    {
        get => _canvas.ZoomMode;
        set
        {
            _canvas.ZoomMode = value;
            _canvas.UpdateLayout(resetScroll: false);
            UpdateZoomText();
        }
    }

    /// <summary>Zoom factor where 1 is 100%.</summary>
    [Browsable(false)]
    public float Zoom
    {
        get => _canvas.EffectiveZoom;
        set
        {
            _canvas.ZoomMode = ViewerZoomMode.Percent;
            _canvas.PercentZoom = Math.Clamp(value, 0.1f, 8f);
            _canvas.UpdateLayout(resetScroll: false);
            UpdateZoomText();
        }
    }

    /// <summary>Runs the report and shows the result. Errors are shown to the user and also returned.</summary>
    public Exception? ShowReport(ReportDefinition report, ReportDataSource? data = null, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        _report = report;
        _data = data;
        _parameters = parameters;
        try
        {
            Cursor = Cursors.WaitCursor;
            Document = new ReportEngine().Run(report, data, parameters);
            return null;
        }
        catch (ReportException ex)
        {
            Document = null;
            MessageBox.Show(this, ex.Message, "Report", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return ex;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    public void GoToPage(int pageNumber)
    {
        if (PageCount == 0) return;
        _canvas.ScrollToPage(Math.Clamp(pageNumber, 1, PageCount) - 1);
        UpdateUi();
    }

    public void PrintReport()
    {
        if (Document is null || PageCount == 0) return;
        try
        {
            Cursor = Cursors.WaitCursor;
            var result = ReportPrinter.Print(Document, FindForm());
            if (result is null) return;
            var pages = $"{result.PagesSent} page{(result.PagesSent == 1 ? "" : "s")}";
            if (result.OutputFile is null)
            {
                SetStatus($"Sent {pages} to {result.PrinterName} at {DateTime.Now:HH:mm}");
                return;
            }
            if (!ReportPrinter.WaitForFile(result.OutputFile, TimeSpan.FromSeconds(20)))
            {
                SetStatus($"Sent {pages} to {result.PrinterName}; the file is still being written");
                return;
            }
            Cursor = Cursors.Default;
            OfferToOpen(result.OutputFile, "Print to PDF");
        }
        catch (Exception ex) when (ex is System.Drawing.Printing.InvalidPrinterException or System.ComponentModel.Win32Exception or IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, ex.Message, "Print", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    /// <summary>The last print or export message, also shown on the toolbar.</summary>
    [Browsable(false)]
    public string StatusText => _statusLabel.Text ?? "";

    /// <summary>Raised when a print or export finishes, so a host window can show the message.</summary>
    public event EventHandler? StatusChanged;

    private void SetStatus(string text)
    {
        _statusLabel.Text = text;
        _statusLabel.ToolTipText = text;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ExportPdf()
    {
        if (Document is null) return;
        using var dialog = new SaveFileDialog { Filter = "PDF document (*.pdf)|*.pdf", FileName = SafeFileName(Document.Title) + ".pdf" };
        if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
        Run(() => PdfExporter.Export(Document, dialog.FileName), dialog.FileName);
    }

    private void ExportDetailRows(bool excel)
    {
        if (_report is null) return;
        var ext = excel ? "xlsx" : "csv";
        using var dialog = new SaveFileDialog
        {
            Filter = excel ? "Excel workbook (*.xlsx)|*.xlsx" : "CSV file (*.csv)|*.csv",
            FileName = SafeFileName(_report.Name) + "." + ext
        };
        if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
        Run(() =>
        {
            var table = IronReports.Export.ExportTable.Build(_report, _data, _parameters);
            if (excel) XlsxExporter.Export(table, dialog.FileName, _report.Name);
            else CsvExporter.Export(table, dialog.FileName);
        }, dialog.FileName);
    }

    private void Run(Action action, string path)
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            action();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ReportException)
        {
            MessageBox.Show(this, ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
        OfferToOpen(path, "Export");
    }

    private void OfferToOpen(string path, string caption)
    {
        SetStatus($"Saved {Path.GetFileName(path)} at {DateTime.Now:HH:mm}");
        if (MessageBox.Show(this, $"Saved {Path.GetFileName(path)}.\n\nOpen it now?", caption,
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes) return;
        try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true }); }
        catch (System.ComponentModel.Win32Exception) { }
    }

    private static string SafeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string((name ?? "Report").Select(c => invalid.Contains(c) ? '_' : c).ToArray()).Trim();
        return cleaned.Length == 0 ? "Report" : cleaned;
    }

    private void ZoomStep(int direction)
    {
        var current = (int)Math.Round(_canvas.EffectiveZoom * 100);
        var next = direction > 0
            ? ZoomSteps.FirstOrDefault(z => z > current, ZoomSteps[^1])
            : ZoomSteps.LastOrDefault(z => z < current, ZoomSteps[0]);
        Zoom = next / 100f;
    }

    private void ApplyZoomText(string text)
    {
        if (text.StartsWith("Page width", StringComparison.OrdinalIgnoreCase)) { ZoomMode = ViewerZoomMode.PageWidth; return; }
        if (text.StartsWith("Whole page", StringComparison.OrdinalIgnoreCase)) { ZoomMode = ViewerZoomMode.WholePage; return; }
        if (float.TryParse(text.Trim().TrimEnd('%'), out var percent) && percent > 0) Zoom = percent / 100f;
        else UpdateZoomText();
    }

    private void UpdateZoomText()
    {
        _zoomBox.Text = _canvas.ZoomMode switch
        {
            ViewerZoomMode.PageWidth => "Page width",
            ViewerZoomMode.WholePage => "Whole page",
            _ => $"{Math.Round(_canvas.PercentZoom * 100)}%"
        };
    }

    internal void UpdateUi()
    {
        var has = PageCount > 0;
        _pageBox.Text = has ? CurrentPage.ToString() : "";
        _pageCount.Text = $"of {PageCount}";
        _printButton.Enabled = has;
        _exportButton.Enabled = has;
        _exportExcel.Enabled = _exportCsv.Enabled = _report is not null;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.P: PrintReport(); return true;
            case Keys.Control | Keys.Home: GoToPage(1); return true;
            case Keys.Control | Keys.End: GoToPage(PageCount); return true;
            case Keys.PageDown when !_pageBox.Focused: GoToPage(CurrentPage + 1); return true;
            case Keys.PageUp when !_pageBox.Focused: GoToPage(CurrentPage - 1); return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>Scrollable surface that draws the pages stacked vertically.</summary>
    private sealed class PageCanvas : ScrollableControl
    {
        private const int Gap = 16;
        private readonly ReportViewerControl _owner;

        public PageCanvas(ReportViewerControl owner)
        {
            _owner = owner;
            DoubleBuffered = true;
            ResizeRedraw = true;
            AutoScroll = true;
            BackColor = Color.FromArgb(128, 128, 128);
            SetStyle(ControlStyles.Selectable | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public RenderedDocument? Document { get; set; }
        public ViewerZoomMode ZoomMode { get; set; } = ViewerZoomMode.PageWidth;
        public float PercentZoom { get; set; } = 1f;

        private float PixelsPerPoint => DeviceDpi / 72f;

        public float EffectiveZoom
        {
            get
            {
                if (Document is null) return PercentZoom;
                var availableW = Math.Max(50, ClientSize.Width - 2 * Gap - SystemInformation.VerticalScrollBarWidth);
                var availableH = Math.Max(50, ClientSize.Height - 2 * Gap);
                return ZoomMode switch
                {
                    ViewerZoomMode.PageWidth => availableW / (Document.PageWidth * PixelsPerPoint),
                    ViewerZoomMode.WholePage => Math.Min(availableW / (Document.PageWidth * PixelsPerPoint), availableH / (Document.PageHeight * PixelsPerPoint)),
                    _ => PercentZoom
                };
            }
        }

        private float DrawScale => EffectiveZoom * PixelsPerPoint;

        private Size PageSizePx => Document is null
            ? Size.Empty
            : new Size((int)Math.Ceiling(Document.PageWidth * DrawScale), (int)Math.Ceiling(Document.PageHeight * DrawScale));

        public int CurrentPageIndex
        {
            get
            {
                if (Document is null || Document.Pages.Count == 0) return 0;
                var step = PageSizePx.Height + Gap;
                var viewTop = -AutoScrollPosition.Y + ClientSize.Height / 3;
                return Math.Clamp((viewTop - Gap) / Math.Max(1, step), 0, Document.Pages.Count - 1);
            }
        }

        public void UpdateLayout(bool resetScroll)
        {
            var fraction = 0f;
            if (!resetScroll && AutoScrollMinSize.Height > 0) fraction = -AutoScrollPosition.Y / (float)AutoScrollMinSize.Height;

            if (Document is null || Document.Pages.Count == 0)
            {
                AutoScrollMinSize = Size.Empty;
            }
            else
            {
                var size = PageSizePx;
                AutoScrollMinSize = new Size(size.Width + 2 * Gap, Document.Pages.Count * (size.Height + Gap) + Gap);
            }

            AutoScrollPosition = new Point(0, resetScroll ? 0 : (int)(fraction * AutoScrollMinSize.Height));
            Invalidate();
        }

        public void ScrollToPage(int index)
        {
            var y = Gap + index * (PageSizePx.Height + Gap) - Gap / 2;
            AutoScrollPosition = new Point(-AutoScrollPosition.X, Math.Max(0, y));
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (ZoomMode != ViewerZoomMode.Percent) UpdateLayout(resetScroll: false);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
            _owner.UpdateUi();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) != 0)
            {
                _owner.ZoomStep(e.Delta > 0 ? 1 : -1);
                return;
            }
            base.OnMouseWheel(e);
            Invalidate();
            _owner.UpdateUi();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            base.OnMouseDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            if (Document is null || Document.Pages.Count == 0)
            {
                TextRenderer.DrawText(g, Document is null ? "No report" : "The report has no pages", Font, ClientRectangle, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            var size = PageSizePx;
            var scale = DrawScale;
            var left = Math.Max(Gap, (ClientSize.Width - size.Width) / 2) + AutoScrollPosition.X;
            for (var i = 0; i < Document.Pages.Count; i++)
            {
                var top = Gap + i * (size.Height + Gap) + AutoScrollPosition.Y;
                var rect = new Rectangle(left, top, size.Width, size.Height);
                if (rect.Bottom + 4 < e.ClipRectangle.Top) continue;
                if (rect.Top > e.ClipRectangle.Bottom) break;

                using (var shadow = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
                    g.FillRectangle(shadow, rect.X + 3, rect.Y + 3, rect.Width, rect.Height);
                g.FillRectangle(Brushes.White, rect);

                var state = g.Save();
                g.SetClip(rect);
                g.TranslateTransform(rect.X, rect.Y);
                g.ScaleTransform(scale, scale);
                GdiRenderer.DrawPage(g, Document.Pages[i], minLineWidth: 1f / scale);
                g.Restore(state);
            }
        }
    }
}

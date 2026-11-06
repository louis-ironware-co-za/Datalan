using System.ComponentModel;
using System.Drawing;
using IronReports.Data;
using IronReports.Export;
using IronReports.Layout;
using IronReports.Model;
using IronReports.Serialization;
using IronReports.WinForms.Designer;

namespace IronReports.WinForms;

/// <summary>
/// A complete designer window: menus, file handling, a Design tab and a Preview tab.
/// <code>
/// using var designer = new ReportDesignerForm();
/// designer.DataSource = ReportDataSource.FromDataTable(table);
/// designer.OpenFile(@"C:\Reports\Invoice.irpt");
/// designer.ShowDialog(this);
/// </code>
/// </summary>
public class ReportDesignerForm : Form
{
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly TabPage _designTab = new("Design");
    private readonly TabPage _previewTab = new("Preview");
    private readonly ReportViewerControl _viewer = new() { Dock = DockStyle.Fill };
    private readonly StatusStrip _status = new();
    private readonly ToolStripStatusLabel _statusText = new() { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
    private string? _filePath;

    public ReportDesignerForm()
    {
        Text = "IronReports Designer";
        Width = 1400;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9f);
        KeyPreview = true;

        Designer = new ReportDesignerControl { Dock = DockStyle.Fill };
        _designTab.Controls.Add(Designer);
        _previewTab.Controls.Add(_viewer);
        _tabs.TabPages.Add(_designTab);
        _tabs.TabPages.Add(_previewTab);
        _tabs.SelectedIndexChanged += (_, _) =>
        {
            if (_tabs.SelectedTab == _previewTab) RunPreview();
        };

        _status.Items.Add(_statusText);
        _viewer.StatusChanged += (_, _) => _statusText.Text = _viewer.StatusText;

        var menu = BuildMenu();
        MainMenuStrip = menu;
        Controls.Add(_tabs);
        Controls.Add(_status);
        Controls.Add(menu);

        Designer.DirtyChanged += (_, _) => UpdateTitle();
        Designer.ReportReplaced += (_, _) => UpdateTitle();
        UpdateTitle();
        _statusText.Text = "Drag fields onto a band. Double-click a text element to edit it. F5 previews the report.";
    }

    public ReportDesignerControl Designer { get; }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReportDefinition Report
    {
        get => Designer.Report;
        set => Designer.Report = value;
    }

    /// <summary>Data used by the field list and the preview.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReportDataSource? DataSource
    {
        get => Designer.DataSource;
        set => Designer.DataSource = value;
    }

    /// <summary>Parameter values for the preview. Report defaults apply when a value is missing.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyDictionary<string, object?>? PreviewParameters { get; set; }

    /// <summary>The file the report was loaded from or saved to.</summary>
    [Browsable(false)]
    public string? FilePath => _filePath;

    /// <summary>Raised after the report has been saved, so a host application can react.</summary>
    public event EventHandler? Saved;

    public void OpenFile(string path)
    {
        Report = ReportSerializer.Load(path);
        _filePath = path;
        Designer.DataSource = Designer.DataSource; // refresh stored field list against live data
        Designer.MarkClean();
        UpdateTitle();
    }

    // ---------------- Menu ----------------

    private MenuStrip BuildMenu()
    {
        var menu = new MenuStrip();

        var file = new ToolStripMenuItem("&File");
        file.DropDownItems.Add(Item("&New", Keys.Control | Keys.N, NewReport));
        file.DropDownItems.Add(Item("&Open…", Keys.Control | Keys.O, OpenReport));
        file.DropDownItems.Add(Item("&Save", Keys.Control | Keys.S, () => Save(saveAs: false)));
        file.DropDownItems.Add(Item("Save &As…", Keys.None, () => Save(saveAs: true)));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add(Item("Export &PDF…", Keys.None, () => { if (RunPreviewQuiet()) _viewer.ExportPdf(); }));
        file.DropDownItems.Add(Item("&Print…", Keys.Control | Keys.P, () => { if (RunPreviewQuiet()) _viewer.PrintReport(); }));
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add(Item("E&xit", Keys.None, Close));

        var edit = new ToolStripMenuItem("&Edit");
        // Shortcuts are handled by the designer control so they do not steal keys from text boxes.
        edit.DropDownItems.Add(Item("&Undo\tCtrl+Z", Keys.None, Designer.Undo));
        edit.DropDownItems.Add(Item("&Redo\tCtrl+Y", Keys.None, Designer.Redo));
        edit.DropDownItems.Add(new ToolStripSeparator());
        edit.DropDownItems.Add(Item("Cu&t\tCtrl+X", Keys.None, Designer.Cut));
        edit.DropDownItems.Add(Item("&Copy\tCtrl+C", Keys.None, Designer.Copy));
        edit.DropDownItems.Add(Item("&Paste\tCtrl+V", Keys.None, Designer.Paste));
        edit.DropDownItems.Add(Item("&Delete\tDel", Keys.None, Designer.DeleteSelection));
        edit.DropDownItems.Add(Item("Select &All in Band\tCtrl+A", Keys.None, Designer.SelectAll));

        var report = new ToolStripMenuItem("&Report");
        report.DropDownItems.Add(Item("&Preview", Keys.F5, () => _tabs.SelectedTab = _previewTab));
        report.DropDownItems.Add(Item("&Design", Keys.F6, () => _tabs.SelectedTab = _designTab));
        report.DropDownItems.Add(Item("&Check Expressions", Keys.F7, CheckExpressions));
        report.DropDownItems.Add(new ToolStripSeparator());
        report.DropDownItems.Add(Item("Add &Group…", Keys.None, Designer.AddGroup));
        report.DropDownItems.Add(Item("&Remove Group", Keys.None, Designer.RemoveGroup));
        report.DropDownItems.Add(new ToolStripSeparator());
        report.DropDownItems.Add(Item("Report &Properties", Keys.None, () => { _tabs.SelectedTab = _designTab; Designer.Surface.ClearSelection(); }));
        report.DropDownItems.Add(Item("Export Detail Rows to &Excel…", Keys.None, () => ExportRows(excel: true)));
        report.DropDownItems.Add(Item("Export Detail Rows to C&SV…", Keys.None, () => ExportRows(excel: false)));

        menu.Items.Add(file);
        menu.Items.Add(edit);
        menu.Items.Add(report);
        return menu;
    }

    private static ToolStripMenuItem Item(string text, Keys shortcut, Action action)
    {
        var item = new ToolStripMenuItem(text, null, (_, _) => action());
        if (shortcut != Keys.None) item.ShortcutKeys = shortcut;
        return item;
    }

    // ---------------- Commands ----------------

    private void NewReport()
    {
        if (!ConfirmDiscard()) return;
        var report = new ReportDefinition();
        Report = report;
        _filePath = null;
        Designer.DataSource = Designer.DataSource;
        Designer.MarkClean();
        UpdateTitle();
    }

    private void OpenReport()
    {
        if (!ConfirmDiscard()) return;
        using var dialog = new OpenFileDialog { Filter = $"IronReports report (*{ReportSerializer.FileExtension})|*{ReportSerializer.FileExtension}|All files|*.*" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            OpenFile(dialog.FileName);
            _tabs.SelectedTab = _designTab;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException or System.Xml.XmlException)
        {
            MessageBox.Show(this, $"The report could not be opened.\n\n{ex.Message}", "Open", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>Saves the report. Returns false when the user cancelled or saving failed.</summary>
    public bool Save(bool saveAs)
    {
        var path = _filePath;
        if (saveAs || path is null)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = $"IronReports report (*{ReportSerializer.FileExtension})|*{ReportSerializer.FileExtension}",
                FileName = (path is null ? Report.Name : Path.GetFileName(path)) is var n && !n.EndsWith(ReportSerializer.FileExtension, StringComparison.OrdinalIgnoreCase)
                    ? n + ReportSerializer.FileExtension
                    : n
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return false;
            path = dialog.FileName;
        }
        try
        {
            // Write to a temporary file first so a failure never leaves a half-written report.
            var temp = path + ".tmp";
            ReportSerializer.Save(Report, temp);
            File.Move(temp, path, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"The report could not be saved.\n\n{ex.Message}", "Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        _filePath = path;
        Designer.MarkClean();
        UpdateTitle();
        _statusText.Text = $"Saved {path}";
        Saved?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private void CheckExpressions()
    {
        var problems = Designer.CheckExpressions();
        if (problems.Count == 0)
        {
            MessageBox.Show(this, "Every expression in the report is valid.", "Check Expressions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var shown = string.Join(Environment.NewLine, problems.Take(25));
        if (problems.Count > 25) shown += $"{Environment.NewLine}… and {problems.Count - 25} more";
        MessageBox.Show(this, shown, $"Check Expressions - {problems.Count} problem(s)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void RunPreview()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var error = _viewer.ShowReport(Report, DataSource, PreviewParameters);
        if (error is null)
            _statusText.Text = $"{_viewer.PageCount} page(s) in {watch.ElapsedMilliseconds} ms" +
                               (DataSource is null ? "  (no data source: detail prints once)" : $"  ·  {DataSource.Rows.Count} rows");
        else
            _statusText.Text = error.Message;
    }

    private bool RunPreviewQuiet()
    {
        RunPreview();
        return _viewer.Document is not null;
    }

    private void ExportRows(bool excel)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = excel ? "Excel workbook (*.xlsx)|*.xlsx" : "CSV file (*.csv)|*.csv",
            FileName = Report.Name + (excel ? ".xlsx" : ".csv")
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var table = ExportTable.Build(Report, DataSource, PreviewParameters);
            if (excel) XlsxExporter.Export(table, dialog.FileName, Report.Name);
            else CsvExporter.Export(table, dialog.FileName);
            _statusText.Text = $"Exported {table.Rows.Count} rows to {dialog.FileName}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ReportException)
        {
            MessageBox.Show(this, ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private bool ConfirmDiscard()
    {
        if (!Designer.IsDirty) return true;
        var answer = MessageBox.Show(this, $"Save changes to {DisplayName}?", "IronReports", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        return answer switch
        {
            DialogResult.Yes => Save(saveAs: false),
            DialogResult.No => true,
            _ => false
        };
    }

    private string DisplayName => _filePath is null ? Report.Name : Path.GetFileName(_filePath);

    private void UpdateTitle() => Text = $"{DisplayName}{(Designer.IsDirty ? " *" : "")} - IronReports Designer";

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!e.Cancel && e.CloseReason == CloseReason.UserClosing && !ConfirmDiscard()) e.Cancel = true;
        base.OnFormClosing(e);
    }
}

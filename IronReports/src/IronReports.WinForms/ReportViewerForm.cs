using IronReports.Data;
using IronReports.Model;
using IronReports.Serialization;

namespace IronReports.WinForms;

/// <summary>
/// A ready-made preview window.
/// <code>
/// var report = ReportSerializer.Load("Invoice.irpt");
/// ReportViewerForm.ShowReport(report, ReportDataSource.FromDataTable(table), owner: this);
/// </code>
/// </summary>
public class ReportViewerForm : Form
{
    public ReportViewerForm()
    {
        Text = "Report Preview";
        Width = 1000;
        Height = 820;
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;
        Viewer = new ReportViewerControl { Dock = DockStyle.Fill };
        Controls.Add(Viewer);
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape) Close();
        };
    }

    public ReportViewerControl Viewer { get; }

    public static void ShowReport(ReportDefinition report, ReportDataSource? data = null,
        IReadOnlyDictionary<string, object?>? parameters = null, IWin32Window? owner = null)
    {
        using var form = new ReportViewerForm { Text = $"{report.Name} - Preview" };
        form.Shown += (_, _) => form.Viewer.ShowReport(report, data, parameters);
        form.ShowDialog(owner);
    }

    public static void ShowReport(string reportPath, ReportDataSource? data = null,
        IReadOnlyDictionary<string, object?>? parameters = null, IWin32Window? owner = null) =>
        ShowReport(ReportSerializer.Load(reportPath), data, parameters, owner);
}

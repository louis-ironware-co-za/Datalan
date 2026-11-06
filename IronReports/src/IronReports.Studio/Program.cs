using IronReports.Samples;
using IronReports.Serialization;
using IronReports.WinForms;

namespace IronReports.Studio;

/// <summary>
/// Stand-alone designer. With a file argument it opens that report; otherwise it opens the sample report.
/// Sample data is always loaded so the preview has rows to show.
/// </summary>
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var form = new ReportDesignerForm
        {
            DataSource = SampleReports.CreateSalesData()
        };

        var path = args.FirstOrDefault(a => a.EndsWith(ReportSerializer.FileExtension, StringComparison.OrdinalIgnoreCase));
        if (path is not null && File.Exists(path))
        {
            form.OpenFile(path);
        }
        else
        {
            form.Report = SampleReports.CreateSalesReport();
        }

        Application.Run(form);
    }
}

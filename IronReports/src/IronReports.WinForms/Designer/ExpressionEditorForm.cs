using System.Drawing;
using IronReports.Expressions;

namespace IronReports.WinForms.Designer;

/// <summary>
/// Edits a text template ("Total: {Sum([Amount]):N2}") or a plain expression ("[Amount] > 0"),
/// with field and function lists and live validation.
/// </summary>
public sealed class ExpressionEditorForm : Form
{
    private readonly TextBox _text = new()
    {
        Multiline = true,
        AcceptsReturn = true,
        AcceptsTab = false,
        ScrollBars = ScrollBars.Vertical,
        Font = new Font("Consolas", 10.5f),
        Dock = DockStyle.Fill,
        WordWrap = true
    };

    private readonly Label _status = new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };
    private readonly Label _help = new() { Dock = DockStyle.Fill, ForeColor = Color.DimGray, AutoEllipsis = true };
    private readonly Button _ok = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
    private readonly bool _isTemplate;

    /// <summary>Fields offered by the editor. The designer keeps this current.</summary>
    public static IReadOnlyList<string> AvailableFields { get; set; } = Array.Empty<string>();

    /// <summary>Report parameters offered by the editor.</summary>
    public static IReadOnlyList<string> AvailableParameters { get; set; } = Array.Empty<string>();

    public ExpressionEditorForm(string value, bool isTemplate, string title)
    {
        _isTemplate = isTemplate;
        Text = title;
        Width = 820;
        Height = 520;
        MinimumSize = new Size(560, 360);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimizeBox = false;
        MaximizeBox = true;
        Font = new Font("Segoe UI", 9f);

        _text.Text = (value ?? "").Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        var fields = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false };
        fields.Items.AddRange(AvailableFields.Select(f => (object)$"[{f}]").ToArray());
        fields.Items.AddRange(AvailableParameters.Select(p => (object)$"?{p}").ToArray());
        fields.DoubleClick += (_, _) => { if (fields.SelectedItem is string s) Insert(s); };

        var functions = new TreeView { Dock = DockStyle.Fill, HideSelection = false };
        foreach (var group in Functions.Catalog.GroupBy(f => f.Category))
        {
            var node = functions.Nodes.Add(group.Key);
            foreach (var f in group.OrderBy(f => f.Name))
                node.Nodes.Add(new TreeNode(f.Signature) { Tag = f });
        }
        functions.AfterSelect += (_, e) => _help.Text = e.Node?.Tag is FunctionInfo f ? f.Description : "";
        functions.NodeMouseDoubleClick += (_, e) =>
        {
            if (e.Node.Tag is FunctionInfo f) Insert(f.Name + "()", caretBack: 1);
        };

        var operators = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
        foreach (var op in new[] { "+", "-", "*", "/", "%", "=", "<>", "<", "<=", ">", ">=", "and", "or", "not", "( )", "' '" })
        {
            var b = new Button { Text = op, Width = 42, Height = 26, Margin = new Padding(2) };
            b.Click += (_, _) =>
            {
                if (op == "( )") Insert("()", caretBack: 1);
                else if (op == "' '") Insert("''", caretBack: 1);
                else Insert($" {op} ");
            };
            operators.Controls.Add(b);
        }
        if (_isTemplate)
        {
            var hole = new Button { Text = "{ }", Width = 48, Height = 26, Margin = new Padding(2) };
            hole.Click += (_, _) => Insert("{}", caretBack: 1);
            operators.Controls.Add(hole);
        }

        var lists = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical };
        lists.Panel1.Controls.Add(fields);
        lists.Panel1.Controls.Add(new Label { Text = "Fields and parameters (double-click)", Dock = DockStyle.Top, Height = 20 });
        lists.Panel2.Controls.Add(functions);
        lists.Panel2.Controls.Add(new Label { Text = "Functions (double-click)", Dock = DockStyle.Top, Height = 20 });

        var editorPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 4) };
        editorPanel.Controls.Add(_text);
        editorPanel.Controls.Add(new Label
        {
            Dock = DockStyle.Top,
            Height = 34,
            ForeColor = Color.DimGray,
            Text = _isTemplate
                ? "Text with expressions in braces. Add a format after a colon.  Example:  Total: {Sum([Amount]):N2}   Page {PageNumber()} of {PageCount()}"
                : "An expression.  Example:  [Amount] > 0 and not IsNullOrEmpty([Customer])"
        });

        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
        split.Panel1.Controls.Add(editorPanel);
        split.Panel1.Controls.Add(operators);
        split.Panel2.Controls.Add(lists);

        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        var bottom = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 64, ColumnCount = 3, RowCount = 2, Padding = new Padding(0, 4, 0, 0) };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        bottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        bottom.Controls.Add(_help, 0, 0);
        bottom.SetColumnSpan(_help, 3);
        bottom.Controls.Add(_status, 0, 1);
        bottom.Controls.Add(_ok, 1, 1);
        bottom.Controls.Add(cancel, 2, 1);

        var root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        root.Controls.Add(split);
        root.Controls.Add(bottom);
        Controls.Add(root);

        AcceptButton = null; // Enter adds a line in templates
        CancelButton = cancel;
        _text.TextChanged += (_, _) => ValidateInput();
        _text.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter && (e.Control || !_isTemplate))
            {
                e.SuppressKeyPress = true;
                if (_ok.Enabled) { DialogResult = DialogResult.OK; Close(); }
            }
        };
        Load += (_, _) =>
        {
            LayoutHelpers.SetSplitter(split, 190);
            LayoutHelpers.SetSplitter(lists, 240);
        };
        Shown += (_, _) =>
        {
            _text.Focus();
            _text.SelectionStart = _text.TextLength;
        };
        ValidateInput();
    }

    public string Value => _text.Text.Replace("\r\n", "\n");

    private void Insert(string s, int caretBack = 0)
    {
        var start = _text.SelectionStart;
        _text.SelectedText = s;
        _text.SelectionStart = Math.Max(0, start + s.Length - caretBack);
        _text.SelectionLength = 0;
        _text.Focus();
    }

    private void ValidateInput()
    {
        var value = Value;
        string? error = null;
        if (value.Trim().Length > 0)
            error = _isTemplate ? TextTemplate.Validate(value) : ReportExpression.Validate(value);

        if (error is null)
        {
            _status.ForeColor = Color.ForestGreen;
            _status.Text = value.Trim().Length == 0 ? "Empty" : "Valid";
        }
        else
        {
            _status.ForeColor = Color.Firebrick;
            _status.Text = error;
        }
        _ok.Enabled = error is null;
    }

    /// <summary>Shows the editor. Returns the new value, or null when cancelled.</summary>
    public static string? Edit(IWin32Window? owner, string value, bool isTemplate, string title)
    {
        using var form = new ExpressionEditorForm(value, isTemplate, title);
        return form.ShowDialog(owner) == DialogResult.OK ? form.Value : null;
    }
}

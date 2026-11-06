using System.ComponentModel;
using System.Drawing;
using IronReports.Data;
using IronReports.Expressions;
using IronReports.Model;
using IronReports.Serialization;

namespace IronReports.WinForms.Designer;

/// <summary>
/// The full report designer: toolbox, field list, design surface and property grid, with undo and clipboard.
/// Host it in any form, or use <see cref="ReportDesignerForm"/>.
/// </summary>
public class ReportDesignerControl : UserControl
{
    private readonly DesignSurface _surface = new() { Dock = DockStyle.Fill };
    private readonly PropertyGrid _properties = new() { Dock = DockStyle.Fill, ToolbarVisible = true, PropertySort = PropertySort.Categorized };
    private readonly Label _selectionLabel = new() { Dock = DockStyle.Top, Height = 24, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(4, 0, 0, 0), AutoEllipsis = true };
    private readonly ListBox _fields = new() { Dock = DockStyle.Fill, IntegralHeight = false };
    private readonly ToolStrip _toolbar = new() { GripStyle = ToolStripGripStyle.Hidden, Padding = new Padding(4, 2, 4, 2) };
    private readonly ContextMenuStrip _contextMenu = new();
    private readonly UndoManager _undo = new();
    private readonly Dictionary<DesignTool, ToolStripButton> _toolButtons = new();
    private readonly ToolStripComboBox _zoomBox = new() { AutoSize = false, Width = 70 };
    private ToolStripButton _undoButton = null!, _redoButton = null!;
    private ReportDataSource? _dataSource;
    private bool _dirty;
    private bool _restoring;

    public ReportDesignerControl()
    {
        BuildToolbar();
        BuildContextMenu();

        var fieldsPanel = new Panel { Dock = DockStyle.Fill };
        fieldsPanel.Controls.Add(_fields);
        fieldsPanel.Controls.Add(new Label
        {
            Text = "Fields  (drag onto a band, or double-click)",
            Dock = DockStyle.Top,
            Height = 24,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(4, 0, 0, 0),
            BackColor = SystemColors.ControlLight
        });

        var propertiesPanel = new Panel { Dock = DockStyle.Fill };
        propertiesPanel.Controls.Add(_properties);
        propertiesPanel.Controls.Add(_selectionLabel);
        _selectionLabel.BackColor = SystemColors.ControlLight;

        var inner = new SplitContainer { Dock = DockStyle.Fill, FixedPanel = FixedPanel.Panel2 };
        inner.Panel1.Controls.Add(_surface);
        inner.Panel2.Controls.Add(propertiesPanel);

        var outer = new SplitContainer { Dock = DockStyle.Fill, FixedPanel = FixedPanel.Panel1 };
        outer.Panel1.Controls.Add(fieldsPanel);
        outer.Panel2.Controls.Add(inner);

        Controls.Add(outer);
        Controls.Add(_toolbar);

        Load += (_, _) =>
        {
            LayoutHelpers.SetSplitter(outer, Math.Min(210, Width / 5));
            LayoutHelpers.SetSplitter(inner, inner.Width - 320);
        };

        _surface.SelectionChanged += (_, _) => UpdatePropertyGrid();
        _surface.Changed += (_, _) => CommitChange();
        _surface.ToolFinished += (_, _) => UpdateToolButtons();
        _surface.ElementActivated += (_, e) => ActivateElement(e);
        _surface.ContextMenuRequested += (_, p) => _contextMenu.Show(p);
        _surface.ZoomChanged += (_, _) => _zoomBox.Text = $"{Math.Round(_surface.Zoom * 100)}%";

        _properties.PropertyValueChanged += (_, _) =>
        {
            _surface.RefreshLayout();
            CommitChange();
            UpdateSelectionLabel();
        };

        _fields.DoubleClick += (_, _) =>
        {
            if (_fields.SelectedItem is ReportField f) _surface.InsertField(f.Name);
        };
        _fields.MouseDown += (_, e) =>
        {
            var index = _fields.IndexFromPoint(e.Location);
            if (e.Button != MouseButtons.Left || index < 0) return;
            _fields.SelectedIndex = index;
        };
        _fields.MouseMove += (_, e) =>
        {
            if (e.Button != MouseButtons.Left || _fields.SelectedItem is not ReportField f) return;
            var data = new DataObject();
            data.SetData(DesignSurface.FieldDragFormat, f.Name);
            _fields.DoDragDrop(data, DragDropEffects.Copy);
        };

        Report = new ReportDefinition();
    }

    // ---------------- Public API ----------------

    /// <summary>The report being edited. Setting it resets undo history and the dirty flag.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReportDefinition Report
    {
        get => _surface.Report;
        set
        {
            _surface.Report = value ?? new ReportDefinition();
            SyncFieldsFromData();
            _undo.Reset(ReportSerializer.SaveToString(_surface.Report));
            _dirty = false;
            RefreshFieldList();
            UpdatePropertyGrid();
            UpdateCommandState();
            ReportReplaced?.Invoke(this, EventArgs.Empty);
            DirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Data used for the field list and for preview. Its fields are stored in the report.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReportDataSource? DataSource
    {
        get => _dataSource;
        set
        {
            _dataSource = value;
            if (SyncFieldsFromData()) CommitChange();
            RefreshFieldList();
        }
    }

    [Browsable(false)]
    public bool IsDirty => _dirty;

    [Browsable(false)]
    public DesignSurface Surface => _surface;

    /// <summary>Raised when the dirty flag changes.</summary>
    public event EventHandler? DirtyChanged;

    /// <summary>Raised when <see cref="Report"/> is replaced by a new instance (load, new, undo, redo).</summary>
    public event EventHandler? ReportReplaced;

    public void MarkClean()
    {
        _dirty = false;
        DirtyChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Undo() => Restore(_undo.Undo());

    public void Redo() => Restore(_undo.Redo());

    public void Cut() => _surface.CutSelection();

    public void Copy() => _surface.CopySelection();

    public void Paste() => _surface.Paste();

    public void DeleteSelection() => _surface.DeleteSelection();

    public void SelectAll() => _surface.SelectAllInBand();

    public void AddGroup()
    {
        var field = _fields.SelectedItem is ReportField f ? $"[{f.Name}]" : "";
        var expression = ExpressionEditorForm.Edit(FindForm(), field, isTemplate: false, "Group by");
        if (expression is null) return;
        var group = Report.AddGroup(expression);
        if (expression.Length > 2 && expression.StartsWith('[') && expression.EndsWith(']'))
            group.Header.Elements.Add(new TextElement
            {
                Name = Report.CreateUniqueName("text"),
                Text = "{" + expression + "}",
                Width = 200,
                Height = 18,
                Y = 3,
                Font = new FontSpec(FontFamilyKind.Sans, 10, bold: true)
            });
        _surface.RefreshLayout();
        _surface.SelectBand(group.Header);
        CommitChange();
    }

    public void RemoveGroup()
    {
        var group = _surface.SelectedBand?.Group ?? Report.Groups.LastOrDefault();
        if (group is null) return;
        if (MessageBox.Show(FindForm(), $"Remove the group on {group} and its header and footer bands?", "Remove group",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
        Report.RemoveGroup(group);
        _surface.ClearSelection();
        _surface.RefreshLayout();
        CommitChange();
    }

    /// <summary>Checks every expression in the report. Returns one line per problem.</summary>
    public List<string> CheckExpressions()
    {
        var problems = new List<string>();
        void Check(string where, string? source, bool template)
        {
            if (string.IsNullOrWhiteSpace(source)) return;
            var error = template ? TextTemplate.Validate(source) : ReportExpression.Validate(source);
            if (error is not null) problems.Add($"{where}: {error}");
        }

        var r = Report;
        Check("Report filter", r.FilterExpression, false);
        Check("Report sort", r.SortExpression, false);
        foreach (var g in r.Groups) Check($"Group {g.Level + 1}", g.Expression, false);
        foreach (var band in r.BandsInDesignOrder())
        foreach (var e in band.Elements)
        {
            var where = $"{band.DisplayName} / {e.Name}";
            Check(where + " (VisibleWhen)", e.VisibleWhen, false);
            if (e is TextElement t) Check(where, t.Text, true);
            if (e is ImageElement i) Check(where + " (ImageExpression)", i.ImageExpression, false);
        }

        var known = new HashSet<string>(r.Fields.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);
        if (known.Count > 0)
        {
            foreach (var band in r.BandsInDesignOrder())
            foreach (var t in band.Elements.OfType<TextElement>())
            {
                foreach (var name in FieldsIn(t.Text).Where(n => !known.Contains(n)))
                    problems.Add($"{band.DisplayName} / {t.Name}: unknown field [{name}]");
            }
        }
        return problems;
    }

    private static IEnumerable<string> FieldsIn(string template)
    {
        var i = 0;
        while ((i = template.IndexOf('[', i)) >= 0)
        {
            var end = template.IndexOf(']', i + 1);
            if (end < 0) yield break;
            yield return template[(i + 1)..end].Trim();
            i = end + 1;
        }
    }

    // ---------------- Internals ----------------

    private bool SyncFieldsFromData()
    {
        if (_dataSource is null) return false;
        var current = string.Join("|", Report.Fields.Select(f => f.Name + ":" + f.TypeName));
        var fresh = string.Join("|", _dataSource.Fields.Select(f => f.Name + ":" + f.TypeName));
        if (current == fresh) return false;
        Report.Fields.Clear();
        Report.Fields.AddRange(_dataSource.Fields.Select(f => new ReportField(f.Name, f.TypeName)));
        return true;
    }

    private void RefreshFieldList()
    {
        _fields.BeginUpdate();
        _fields.Items.Clear();
        foreach (var f in Report.Fields) _fields.Items.Add(f);
        _fields.EndUpdate();
        ExpressionEditorForm.AvailableFields = Report.Fields.Select(f => f.Name).ToList();
        ExpressionEditorForm.AvailableParameters = Report.Parameters.Select(p => p.Name).ToList();
    }

    private void CommitChange()
    {
        if (_restoring) return;
        if (_undo.Commit(ReportSerializer.SaveToString(Report)))
        {
            var wasDirty = _dirty;
            _dirty = true;
            if (!wasDirty) DirtyChanged?.Invoke(this, EventArgs.Empty);
        }
        ExpressionEditorForm.AvailableParameters = Report.Parameters.Select(p => p.Name).ToList();
        _properties.Refresh();
        UpdateCommandState();
    }

    private void Restore(string? snapshot)
    {
        if (snapshot is null) return;
        _restoring = true;
        try
        {
            _surface.Report = ReportSerializer.LoadFromString(snapshot);
            RefreshFieldList();
            UpdatePropertyGrid();
        }
        finally
        {
            _restoring = false;
        }
        _dirty = true;
        DirtyChanged?.Invoke(this, EventArgs.Empty);
        ReportReplaced?.Invoke(this, EventArgs.Empty);
        UpdateCommandState();
    }

    private void UpdatePropertyGrid()
    {
        var selection = _surface.Selection;
        if (selection.Count > 0) _properties.SelectedObjects = selection.Cast<object>().ToArray();
        else if (_surface.IsBandSelected && _surface.SelectedBand is { } band) _properties.SelectedObject = band;
        else _properties.SelectedObject = Report;
        UpdateSelectionLabel();
        UpdateCommandState();
    }

    private void UpdateSelectionLabel()
    {
        var selection = _surface.Selection;
        _selectionLabel.Text = selection.Count switch
        {
            0 when _surface.IsBandSelected => _surface.SelectedBand?.DisplayName ?? "",
            0 => $"Report: {Report.Name}",
            1 => selection[0].ToString(),
            _ => $"{selection.Count} elements"
        };
    }

    private void ActivateElement(ReportElement element)
    {
        switch (element)
        {
            case TextElement t:
                var text = ExpressionEditorForm.Edit(FindForm(), t.Text, isTemplate: true, $"Edit Text - {t.Name}");
                if (text is null || text == t.Text) return;
                t.Text = text;
                break;
            case ImageElement i:
                var data = PictureLoader.Pick(FindForm());
                if (data is null) return;
                i.ImageData = data;
                break;
            default:
                return;
        }
        _surface.RefreshLayout();
        CommitChange();
    }

    private void BuildToolbar()
    {
        void Tool(DesignTool tool, string text, string tip)
        {
            var b = new ToolStripButton(text) { ToolTipText = tip, CheckOnClick = false, DisplayStyle = ToolStripItemDisplayStyle.Text };
            b.Click += (_, _) =>
            {
                _surface.Tool = tool;
                UpdateToolButtons();
                _surface.Focus();
            };
            _toolButtons[tool] = b;
            _toolbar.Items.Add(b);
        }

        ToolStripButton Command(string text, string tip, Action action)
        {
            var b = new ToolStripButton(text) { ToolTipText = tip, DisplayStyle = ToolStripItemDisplayStyle.Text };
            b.Click += (_, _) => action();
            _toolbar.Items.Add(b);
            return b;
        }

        Tool(DesignTool.Pointer, "Select", "Select and move elements (Esc)");
        Tool(DesignTool.Text, "Text", "Draw a text element. Hold Shift to draw several.");
        Tool(DesignTool.Line, "Line", "Draw a line");
        Tool(DesignTool.Rectangle, "Box", "Draw a rectangle");
        Tool(DesignTool.Ellipse, "Ellipse", "Draw an ellipse");
        Tool(DesignTool.Image, "Picture", "Draw a picture box, then double-click it to choose a file");
        _toolbar.Items.Add(new ToolStripSeparator());

        _undoButton = Command("Undo", "Undo (Ctrl+Z)", Undo);
        _redoButton = Command("Redo", "Redo (Ctrl+Y)", Redo);
        _toolbar.Items.Add(new ToolStripSeparator());

        var align = new ToolStripDropDownButton("Align") { ToolTipText = "Align the selection to the first selected element" };
        align.DropDownItems.Add("Lefts", null, (_, _) => _surface.Align(DesignSurface.Alignment.Left));
        align.DropDownItems.Add("Rights", null, (_, _) => _surface.Align(DesignSurface.Alignment.Right));
        align.DropDownItems.Add("Centers", null, (_, _) => _surface.Align(DesignSurface.Alignment.CenterHorizontal));
        align.DropDownItems.Add("Tops", null, (_, _) => _surface.Align(DesignSurface.Alignment.Top));
        align.DropDownItems.Add("Bottoms", null, (_, _) => _surface.Align(DesignSurface.Alignment.Bottom));
        align.DropDownItems.Add(new ToolStripSeparator());
        align.DropDownItems.Add("Same width", null, (_, _) => _surface.Align(DesignSurface.Alignment.SameWidth));
        align.DropDownItems.Add("Same height", null, (_, _) => _surface.Align(DesignSurface.Alignment.SameHeight));
        align.DropDownItems.Add("Same size", null, (_, _) => _surface.Align(DesignSurface.Alignment.SameSize));
        _toolbar.Items.Add(align);
        Command("Front", "Bring to front", _surface.BringSelectionToFront);
        Command("Back", "Send to back", _surface.SendSelectionToBack);
        _toolbar.Items.Add(new ToolStripSeparator());

        Command("+ Group", "Add a group (group header and footer bands)", AddGroup);
        Command("− Group", "Remove the selected group", RemoveGroup);
        _toolbar.Items.Add(new ToolStripSeparator());

        _toolbar.Items.Add(new ToolStripLabel("Zoom"));
        foreach (var z in new[] { 50, 75, 100, 125, 150, 200, 300 }) _zoomBox.Items.Add($"{z}%");
        _zoomBox.Text = "100%";
        void ApplyZoom()
        {
            if (float.TryParse(_zoomBox.Text.Trim().TrimEnd('%'), out var z) && z > 0) _surface.Zoom = z / 100f;
            _zoomBox.Text = $"{Math.Round(_surface.Zoom * 100)}%";
        }
        _zoomBox.SelectedIndexChanged += (_, _) => ApplyZoom();
        _zoomBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            ApplyZoom();
            e.SuppressKeyPress = true;
        };
        _toolbar.Items.Add(_zoomBox);

        var grid = new ToolStripButton("Grid") { CheckOnClick = true, Checked = true, ToolTipText = "Show the grid" };
        grid.CheckedChanged += (_, _) => { _surface.ShowGrid = grid.Checked; _surface.Invalidate(); };
        var snap = new ToolStripButton("Snap") { CheckOnClick = true, Checked = true, ToolTipText = "Snap to the 4 pt grid (hold Alt to place freely)" };
        snap.CheckedChanged += (_, _) => _surface.SnapToGrid = snap.Checked;
        _toolbar.Items.Add(grid);
        _toolbar.Items.Add(snap);

        UpdateToolButtons();
    }

    private void BuildContextMenu()
    {
        _contextMenu.Items.Add("Edit text…", null, (_, _) => { if (_surface.Selection.Count == 1) ActivateElement(_surface.Selection[0]); });
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add("Cut", null, (_, _) => Cut());
        _contextMenu.Items.Add("Copy", null, (_, _) => Copy());
        _contextMenu.Items.Add("Paste", null, (_, _) => Paste());
        _contextMenu.Items.Add("Delete", null, (_, _) => DeleteSelection());
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add("Bring to front", null, (_, _) => _surface.BringSelectionToFront());
        _contextMenu.Items.Add("Send to back", null, (_, _) => _surface.SendSelectionToBack());
        _contextMenu.Opening += (_, _) =>
        {
            var hasSelection = _surface.Selection.Count > 0;
            _contextMenu.Items[0].Enabled = _surface.Selection.Count == 1 && _surface.Selection[0] is TextElement or ImageElement;
            _contextMenu.Items[0].Text = _surface.Selection.Count == 1 && _surface.Selection[0] is ImageElement ? "Choose picture…" : "Edit text…";
            _contextMenu.Items[2].Enabled = _contextMenu.Items[3].Enabled = _contextMenu.Items[5].Enabled = hasSelection;
            _contextMenu.Items[4].Enabled = _surface.CanPaste;
            _contextMenu.Items[7].Enabled = _contextMenu.Items[8].Enabled = hasSelection;
        };
    }

    private void UpdateToolButtons()
    {
        foreach (var (tool, button) in _toolButtons) button.Checked = _surface.Tool == tool;
    }

    private void UpdateCommandState()
    {
        if (_undoButton is null) return;
        _undoButton.Enabled = _undo.CanUndo;
        _redoButton.Enabled = _undo.CanRedo;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // Leave text editing keys to text boxes (property grid, zoom box).
        var inTextBox = ActiveControlIsTextInput();
        switch (keyData)
        {
            case Keys.Control | Keys.Z when !inTextBox: Undo(); return true;
            case Keys.Control | Keys.Y when !inTextBox: Redo(); return true;
            case Keys.Control | Keys.Shift | Keys.Z when !inTextBox: Redo(); return true;
            case Keys.Control | Keys.C when !inTextBox: Copy(); return true;
            case Keys.Control | Keys.X when !inTextBox: Cut(); return true;
            case Keys.Control | Keys.V when !inTextBox: Paste(); return true;
            case Keys.Control | Keys.A when !inTextBox: SelectAll(); return true;
            case Keys.Delete when _surface.Focused: DeleteSelection(); return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private bool ActiveControlIsTextInput()
    {
        Control? c = this;
        while (c is ContainerControl cc && cc.ActiveControl is not null) c = cc.ActiveControl;
        if (c is TextBoxBase) return true;
        // PropertyGrid's in-place editor is a child text box of the grid.
        return _properties.ContainsFocus || _zoomBox.Focused;
    }
}

using System.ComponentModel;
using System.Drawing;

namespace IronReports.Model;

/// <summary>A horizontal strip of the report that is printed once per occurrence (page, group, row, ...).</summary>
public sealed class Band
{
    public Band() { }

    public Band(BandKind kind, float height)
    {
        Kind = kind;
        Height = height;
    }

    [Category("Design"), ReadOnly(true)]
    public BandKind Kind { get; set; }

    private float _height = 30f;

    [Category("Layout")]
    public float Height { get => _height; set => _height = Math.Max(0f, value); }

    [Category("Behavior")] public bool Visible { get; set; } = true;

    [Category("Behavior"), Description("Grow the band when an element grows.")]
    public bool CanGrow { get; set; } = true;

    [Category("Behavior"), Description("Start a new page before printing this band (ignored when the page is still empty).")]
    public bool PageBreakBefore { get; set; }

    [Category("Appearance")] public Color BackColor { get; set; } = Color.Transparent;

    [Browsable(false), NotSerialized]
    public List<ReportElement> Elements { get; } = new();

    /// <summary>The group that owns this band, for group header and footer bands.</summary>
    [Category("Group"), NotSerialized, TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("Grouping settings. Only used by group header and footer bands.")]
    public GroupDefinition? Group { get; internal set; }

    [Browsable(false), NotSerialized]
    public string DisplayName => Kind switch
    {
        BandKind.ReportHeader => "Report Header",
        BandKind.PageHeader => "Page Header",
        BandKind.GroupHeader => $"Group Header {GroupLevelText}",
        BandKind.Detail => "Detail",
        BandKind.GroupFooter => $"Group Footer {GroupLevelText}",
        BandKind.PageFooter => "Page Footer",
        BandKind.ReportFooter => "Report Footer",
        _ => Kind.ToString()
    };

    private string GroupLevelText =>
        Group is null ? "" : $"{Group.Level + 1}" + (string.IsNullOrWhiteSpace(Group.Expression) ? "" : $"  [{Group.Expression}]");

    public override string ToString() => DisplayName;
}

/// <summary>Groups rows by an expression, with a header and footer band per group value.</summary>
public sealed class GroupDefinition
{
    public GroupDefinition()
    {
        Header = new Band(BandKind.GroupHeader, 24f) { Group = this };
        Footer = new Band(BandKind.GroupFooter, 24f) { Group = this };
    }

    [Description("Expression to group by. Example: [Region]")]
    public string Expression { get; set; } = "";

    [Description("Sort groups in descending order.")]
    public bool Descending { get; set; }

    [Browsable(false), NotSerialized]
    public Band Header { get; }

    [Browsable(false), NotSerialized]
    public Band Footer { get; }

    /// <summary>Zero-based nesting level, outermost first. Set by the owning report.</summary>
    [Browsable(false), NotSerialized]
    public int Level { get; internal set; }

    public override string ToString() => string.IsNullOrWhiteSpace(Expression) ? "(no expression)" : Expression;
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class PageSetup
{
    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.All)]
    public PaperKind Paper { get; set; } = PaperKind.A4;

    [NotifyParentProperty(true), RefreshProperties(RefreshProperties.All)]
    public bool Landscape { get; set; }

    [NotifyParentProperty(true), Description("Paper width in points when Paper is Custom.")]
    public float CustomWidth { get; set; } = 595.28f;

    [NotifyParentProperty(true), Description("Paper height in points when Paper is Custom.")]
    public float CustomHeight { get; set; } = 841.89f;

    [NotifyParentProperty(true)] public float MarginLeft { get; set; } = 36f;
    [NotifyParentProperty(true)] public float MarginTop { get; set; } = 36f;
    [NotifyParentProperty(true)] public float MarginRight { get; set; } = 36f;
    [NotifyParentProperty(true)] public float MarginBottom { get; set; } = 36f;

    /// <summary>Page width in points, after orientation.</summary>
    [NotSerialized, Description("Page width in points (1 pt = 1/72 inch).")]
    public float Width => Landscape ? PortraitSize.Height : PortraitSize.Width;

    /// <summary>Page height in points, after orientation.</summary>
    [NotSerialized]
    public float Height => Landscape ? PortraitSize.Width : PortraitSize.Height;

    [Browsable(false), NotSerialized]
    public float PrintableWidth => Math.Max(10f, Width - MarginLeft - MarginRight);

    [Browsable(false), NotSerialized]
    public SizeF PortraitSize => Paper switch
    {
        PaperKind.A4 => new SizeF(595.28f, 841.89f),
        PaperKind.A3 => new SizeF(841.89f, 1190.55f),
        PaperKind.A5 => new SizeF(419.53f, 595.28f),
        PaperKind.Letter => new SizeF(612f, 792f),
        PaperKind.Legal => new SizeF(612f, 1008f),
        _ => new SizeF(Math.Max(36f, CustomWidth), Math.Max(36f, CustomHeight))
    };

    public override string ToString() => $"{Paper}{(Landscape ? " landscape" : "")}";
}

/// <summary>A named value supplied by the calling application. Referenced in expressions as ?Name.</summary>
public sealed class ReportParameter
{
    public string Name { get; set; } = "Parameter1";
    public ParameterType Type { get; set; } = ParameterType.String;

    [Description("Default value as text. Dates use yyyy-MM-dd, numbers use a dot as decimal separator.")]
    public string DefaultValue { get; set; } = "";

    public override string ToString() => Name;
}

/// <summary>Describes a data field so the designer can list fields without live data.</summary>
public sealed class ReportField
{
    public ReportField() { }

    public ReportField(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public string Name { get; set; } = "";
    public string TypeName { get; set; } = "String";

    public override string ToString() => $"{Name} ({TypeName})";
}

/// <summary>The complete, serializable definition of a report.</summary>
public sealed class ReportDefinition
{
    public ReportDefinition()
    {
        ReportHeader = new Band(BandKind.ReportHeader, 40f);
        PageHeader = new Band(BandKind.PageHeader, 24f);
        Detail = new Band(BandKind.Detail, 20f);
        PageFooter = new Band(BandKind.PageFooter, 24f);
        ReportFooter = new Band(BandKind.ReportFooter, 30f);
    }

    [Category("Design")] public string Name { get; set; } = "Report1";

    [Category("Page")]
    public PageSetup Page { get; set; } = new();

    [Category("Data"), Description("Expression used to sort detail rows (after group expressions). Example: [InvoiceDate]")]
    public string SortExpression { get; set; } = "";

    [Category("Data")] public bool SortDescending { get; set; }

    [Category("Data"), Description("Rows are printed only when this expression is true. Example: [Amount] > 0 and ?ShowAll")]
    public string FilterExpression { get; set; } = "";

    [Category("Data"), Description("Culture used for number and date formatting, for example en-ZA. Empty uses the machine culture.")]
    public string Culture { get; set; } = "";

    [Category("Data"), NotSerialized]
    public List<ReportParameter> Parameters { get; } = new();

    [Browsable(false), NotSerialized]
    public List<ReportField> Fields { get; } = new();

    [Browsable(false), NotSerialized] public Band ReportHeader { get; }
    [Browsable(false), NotSerialized] public Band PageHeader { get; }
    [Browsable(false), NotSerialized] public Band Detail { get; }
    [Browsable(false), NotSerialized] public Band PageFooter { get; }
    [Browsable(false), NotSerialized] public Band ReportFooter { get; }

    private readonly List<GroupDefinition> _groups = new();

    /// <summary>Groups, outermost first.</summary>
    [Browsable(false), NotSerialized]
    public IReadOnlyList<GroupDefinition> Groups => _groups;

    public GroupDefinition AddGroup(string expression, int? index = null)
    {
        var group = new GroupDefinition { Expression = expression };
        _groups.Insert(Math.Clamp(index ?? _groups.Count, 0, _groups.Count), group);
        RenumberGroups();
        return group;
    }

    public void RemoveGroup(GroupDefinition group)
    {
        _groups.Remove(group);
        RenumberGroups();
    }

    private void RenumberGroups()
    {
        for (var i = 0; i < _groups.Count; i++) _groups[i].Level = i;
    }

    /// <summary>All bands in the order they appear in the designer.</summary>
    public IEnumerable<Band> BandsInDesignOrder()
    {
        yield return ReportHeader;
        yield return PageHeader;
        foreach (var g in _groups) yield return g.Header;
        yield return Detail;
        for (var i = _groups.Count - 1; i >= 0; i--) yield return _groups[i].Footer;
        yield return PageFooter;
        yield return ReportFooter;
    }

    public IEnumerable<ReportElement> AllElements() => BandsInDesignOrder().SelectMany(b => b.Elements);

    public Band? FindBandOf(ReportElement element) => BandsInDesignOrder().FirstOrDefault(b => b.Elements.Contains(element));

    /// <summary>Creates a name such as "text3" that is not yet used in this report.</summary>
    public string CreateUniqueName(string prefix)
    {
        prefix = prefix.ToLowerInvariant();
        var used = new HashSet<string>(AllElements().Select(e => e.Name), StringComparer.OrdinalIgnoreCase);
        for (var i = 1; ; i++)
        {
            var candidate = prefix + i;
            if (!used.Contains(candidate)) return candidate;
        }
    }

    public override string ToString() => Name;
}

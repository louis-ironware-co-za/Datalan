namespace IronReports.Model;

/// <summary>The role a band plays in the report layout.</summary>
public enum BandKind
{
    ReportHeader,
    PageHeader,
    GroupHeader,
    Detail,
    GroupFooter,
    PageFooter,
    ReportFooter
}

public enum HorizontalAlign { Left, Center, Right }

public enum VerticalAlign { Top, Middle, Bottom }

[Flags]
public enum BorderSides
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8,
    All = Left | Top | Right | Bottom
}

/// <summary>
/// Font families are limited to the three PDF base families so that layout,
/// screen and PDF output measure text identically.
/// Sans = Helvetica / Arial, Serif = Times / Times New Roman, Mono = Courier / Courier New.
/// </summary>
public enum FontFamilyKind { Sans, Serif, Mono }

public enum LineDirection { Horizontal, Vertical, Slant, BackSlant }

public enum ShapeKind { Rectangle, Ellipse }

public enum ImageSizeMode { Normal, Stretch, Zoom }

public enum PaperKind { A4, A3, A5, Letter, Legal, Custom }

public enum ParameterType { String, Number, Date, Boolean }

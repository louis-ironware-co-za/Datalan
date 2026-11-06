namespace IronReports.WinForms.Designer;

internal static class LayoutHelpers
{
    /// <summary>Sets a splitter position, clamped to what the container allows. Setting it out of range throws in WinForms.</summary>
    public static void SetSplitter(SplitContainer split, int distance)
    {
        var size = split.Orientation == Orientation.Vertical ? split.Width : split.Height;
        var max = size - split.Panel2MinSize - split.SplitterWidth;
        if (max <= split.Panel1MinSize) return;
        split.SplitterDistance = Math.Clamp(distance, split.Panel1MinSize, max);
    }
}

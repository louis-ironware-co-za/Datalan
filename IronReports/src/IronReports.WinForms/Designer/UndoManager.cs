namespace IronReports.WinForms.Designer;

/// <summary>
/// Snapshot-based undo. After every change the designer commits the serialized report;
/// undo and redo swap whole snapshots. Simple and robust for any kind of edit.
/// </summary>
internal sealed class UndoManager
{
    private readonly LinkedList<string> _undo = new();
    private readonly Stack<string> _redo = new();
    private string _current = "";

    public int Limit { get; set; } = 200;
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Reset(string snapshot)
    {
        _undo.Clear();
        _redo.Clear();
        _current = snapshot;
    }

    /// <summary>Records a new state. Returns false when nothing changed.</summary>
    public bool Commit(string snapshot)
    {
        if (snapshot == _current) return false;
        _undo.AddLast(_current);
        if (_undo.Count > Limit) _undo.RemoveFirst();
        _redo.Clear();
        _current = snapshot;
        return true;
    }

    public string? Undo()
    {
        if (_undo.Count == 0) return null;
        _redo.Push(_current);
        _current = _undo.Last!.Value;
        _undo.RemoveLast();
        return _current;
    }

    public string? Redo()
    {
        if (_redo.Count == 0) return null;
        _undo.AddLast(_current);
        _current = _redo.Pop();
        return _current;
    }
}

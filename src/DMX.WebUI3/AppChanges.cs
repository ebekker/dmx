// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3;

public class AppChanges
{
    private readonly AppEvents _events;

    private readonly Stack<Change> _undo = new();
    private readonly Stack<Change> _redo = new();

    public AppChanges(AppEvents events)
    {
        _events = events;
    }

    public bool IsUndoEmpty => _undo.Count == 0;
    public bool IsRedoEmpty => _redo.Count == 0;

    public string UndoLabel => IsUndoEmpty ? string.Empty : $"Undo '{PeekUndo()!.Label}'";
    public string RedoLabel => IsRedoEmpty ? string.Empty : $"Redo '{PeekRedo()!.Label}'";

    public void ClearRedo() => _redo.Clear();
    public void ClearUndo() => _undo.Clear();

    public void Push(Change change)
    {
        _redo.Clear();
        _undo.Push(change);
        _events.FireChangeAdded(this, change);
    }

    public void Push(string label, Func<Task> redo, Func<Task> undo)
    {
        Push(new Change(label, redo, undo));
    }

    public Change? PeekUndo()
    {
        return _undo.TryPeek(out var change) ? change : null;
    }

    public Change? PeekRedo()
    {
        return _redo.TryPeek(out var change) ? change : null;
    }

    public async Task<Change?> ApplyUndo()
    {
        if (!_undo.TryPop(out var change))
            return null;

        try
        {
            await change.Undo();
            _redo.Push(change);
            _events.FireChangeUndo(this, change);
            return change;
        }
        catch (Exception)
        {
            // Put it back
            _undo.Push(change);
            throw;
        }
    }

    public async Task<Change?> ApplyRedo()
    {
        if (!_redo.TryPop(out var change))
            return null;

        try
        {
            await change.Redo();
            _undo.Push(change);
            _events.FireChangeRedo(this, change);
            return change;
        }
        catch (Exception)
        {
            // Put it back
            _redo.Push(change);
            throw;
        }

    }

    public class Change
    {
        public Change(string label, Func<Task> redo, Func<Task> undo)
        {
            Label = label;
            Redo = redo;
            Undo = undo;
        }

        public string Label { get; }
        public Func<Task> Redo { get; }
        public Func<Task> Undo { get; }
    }
}

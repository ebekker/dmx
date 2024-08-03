// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI2.Models;

public abstract class ActionBase
{
    public abstract string Label { get; }

    public virtual Task DoItAsync() => Task.CompletedTask;

    public virtual Task UndoItAsync() => Task.CompletedTask;
}

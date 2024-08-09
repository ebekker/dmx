// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Geometry;

namespace DMX.WebUI3;

public class AppEvents
{
    public event EventHandler? OnAppStateChanged;
    public event EventHandler<Rectangle?>? OnDiagramContainerChanged;
    public event EventHandler? OnResetToOrigin;
    public event EventHandler? OnVisualChanged;
    public event EventHandler? OnDataModelChanged;

    public event EventHandler<object>? OnDoubleClickElement;

    public event EventHandler<AppChanges.Change>? OnChangeAdded;
    public event EventHandler<AppChanges.Change>? OnChangeUndo;
    public event EventHandler<AppChanges.Change>? OnChangeRedo;

    public void FireAppStateChanged(object sender) =>
        OnAppStateChanged?.Invoke(sender, EventArgs.Empty);

    public void FireDiagramContainerChanged(object sender, Rectangle? arg) =>
        OnDiagramContainerChanged?.Invoke(sender, arg);

    public void FireResetToOrigin(object sender) =>
        OnResetToOrigin?.Invoke(sender, EventArgs.Empty);

    public void FireVisualChanged(object sender) =>
        OnVisualChanged?.Invoke(sender, EventArgs.Empty);

    public void FireDataModelChanged(object sender) =>
        OnDataModelChanged?.Invoke(sender, EventArgs.Empty);


    public void FireDoubleClickElement(object sender, object element) =>
        OnDoubleClickElement?.Invoke(sender, element);


    public void FireChangeAdded(object sender, AppChanges.Change change) =>
        OnChangeAdded?.Invoke(sender, change);

    public void FireChangeUndo(object sender, AppChanges.Change change) =>
        OnChangeUndo?.Invoke(sender, change);

    public void FireChangeRedo(object sender, AppChanges.Change change) =>
        OnChangeRedo?.Invoke(sender, change);

}

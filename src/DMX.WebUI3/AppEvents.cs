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
    public event EventHandler<AsyncEventArgs>? OnDataModelChanged;

    public event EventHandler<object>? OnDoubleClickElement;

    public event EventHandler<AppChanges.Change>? OnChangeAdded;
    public event EventHandler<AppChanges.Change>? OnChangeUndo;
    public event EventHandler<AppChanges.Change>? OnChangeRedo;

    public event EventHandler<ZoomAsyncEventArgs>? OnZoomChanged;

    public void FireAppStateChanged(object sender) =>
        OnAppStateChanged?.Invoke(sender, EventArgs.Empty);

    public void FireDiagramContainerChanged(object sender, Rectangle? arg) =>
        OnDiagramContainerChanged?.Invoke(sender, arg);

    public void FireResetToOrigin(object sender) =>
        OnResetToOrigin?.Invoke(sender, EventArgs.Empty);

    public void FireVisualChanged(object sender) =>
        OnVisualChanged?.Invoke(sender, EventArgs.Empty);

    public async Task FireDataModelChangedAsync(object sender)
    {
        var tasks = FireDataModelChanged(sender);
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    public IList<Task> FireDataModelChanged(object sender, IList<Task>? tasks = null)
    {
        tasks ??= new List<Task>();
        OnDataModelChanged?.Invoke(sender, new AsyncEventArgs(tasks));
        return tasks;
    }

    public async Task FireZoomChangedAsync(object sender, double zoom)
    {
        var tasks = new List<Task>();
        OnZoomChanged?.Invoke(sender, new(tasks)
        {
            Zoom = zoom,
        });
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }


    public void FireDoubleClickElement(object sender, object element) =>
        OnDoubleClickElement?.Invoke(sender, element);


    public void FireChangeAdded(object sender, AppChanges.Change change) =>
        OnChangeAdded?.Invoke(sender, change);

    public void FireChangeUndo(object sender, AppChanges.Change change) =>
        OnChangeUndo?.Invoke(sender, change);

    public void FireChangeRedo(object sender, AppChanges.Change change) =>
        OnChangeRedo?.Invoke(sender, change);

}

public class AsyncEventArgs : EventArgs
{
    private readonly IList<Task> _tasks;

    public AsyncEventArgs(IList<Task> tasks)
    {
        _tasks = tasks;
    }

    public void AddTask(Task t)
    {
        _tasks.Add(t);
    }
}

public class ZoomAsyncEventArgs : AsyncEventArgs
{
    public ZoomAsyncEventArgs(IList<Task> tasks) : base(tasks)
    { }

    public double Zoom { get; set; } = 1.0;
}

// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Geometry;

namespace DMX.WebUI3;

public class AppEvents
{
    public event EventHandler? OnAppStateChanged;
    public event EventHandler<Rectangle?>? OnDiagramContainerChanged;

    public void FireAppStateChanged(object sender)
    {
        OnAppStateChanged?.Invoke(sender, EventArgs.Empty);
    }

    public void FireDiagramContainerChanged(object sender, Rectangle? arg)
    {
        OnDiagramContainerChanged?.Invoke(sender, arg);
    }
}

// DMX.
// Copyright (C) Eugene Bekker.


namespace DMX.WebUI3.Events;

public class ZoomCommandEventArgs : AsyncEventArgs
{
    public ZoomCommandEventArgs(IList<Func<Task>> tasks,
        CancellationToken? cancel = null)
        : base(tasks, cancel)
    { }

    public double Zoom { get; init; } = 1.0;
}

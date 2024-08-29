// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3.Events;

public interface IAsyncEventArgs
{
    CancellationToken? Cancel { get; }

    void AddInvoke(Func<Task> task);
}

public class AsyncEventArgs : IAsyncEventArgs
{
    private readonly IList<Func<Task>> _tasks;

    public AsyncEventArgs(IList<Func<Task>> tasks,
        CancellationToken? cancel = null)
    {
        _tasks = tasks;
        Cancel = cancel;
    }

    public CancellationToken? Cancel { get; init; }

    public void AddInvoke(Func<Task> task)
    {
        _tasks.Add(task);
    }
}

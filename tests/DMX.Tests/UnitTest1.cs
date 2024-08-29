namespace DMX.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var tasks = new List<Func<Task>>();
        var ev = new AsyncEventArgs(tasks);

        ev.InvokeAsync(NoopAsync);
        ev.InvokeAsync(() => NoopAsync());
    }

    Task NoopAsync()
    {
        return Task.CompletedTask;
    }
}

public class AsyncEventTasks
{
    private List<Func<Task>>? _tasks = null;

    public bool IsEmpty => _tasks == null || _tasks.Count == 0;

    public event EventHandler? MyEvent0;
    public event EventHandler<AsyncEventArgs>? MyEvent1;

    public void Invoke(Func<Task> task)
    {
        _tasks ??= new();
        _tasks.Add(task);
    }

    public async Task InvokeTasksAsync()
    {
        if (IsEmpty)
            return;

        // We need to guarantee the same order
        // that these tasks were collected
        foreach (var t in _tasks!)
        {
            await t();
        }
    }
}

public interface IAsyncEventArgs
{
    CancellationToken? Cancel { get; }

    void InvokeAsync(Func<Task> task);
}

/// <summary>
/// Base class for all async event classes.
/// </summary>
public class AsyncEventArgs : IAsyncEventArgs
{
    private IList<Func<Task>>? _tasks;

    public AsyncEventArgs(IList<Func<Task>>? tasks, CancellationToken? cancel = null)
    {
        _tasks = tasks;
        Cancel = cancel;
    }

    public CancellationToken? Cancel { get; init; }

    public void InvokeAsync(Func<Task> task)
    {
        _tasks ??= new List<Func<Task>>();
        _tasks.Add(task);
    }
}

/// <summary>
/// Represents the method that will handle an event when the event provides data.
/// </summary>
public delegate void AsyncEventHandler<TAsyncEventArgs>(object? sender, TAsyncEventArgs ev)
        where TAsyncEventArgs : AsyncEventArgs;

public class MyEvent1Args : AsyncEventArgs
{
    public MyEvent1Args(IList<Func<Task>>? tasks,
        CancellationToken? cancel = null)
        : base(tasks, cancel)
    { }

    public required string Message1 { get; init; }
}

public class EventManager
{
    public event AsyncEventHandler<AsyncEventArgs>? MyEvent;

    public async Task FireMyEvent(AsyncEventArgs ev, CancellationToken? cancel = null)
    {

    }
}

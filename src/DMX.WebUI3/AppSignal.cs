// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3;

/// <summary>
/// Used to signal an event with some parameter
/// data during a fixed time period.
/// </summary>
public class AppSignal<TArg>
{
    public event EventHandler<TArg>? OnSignal;

    public void FireSignal(object sender, TArg arg)
    {
        OnSignal?.Invoke(sender, arg);
    }
}

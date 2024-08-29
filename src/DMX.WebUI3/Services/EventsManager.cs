// DMX.
// Copyright (C) Eugene Bekker.

using DMX.WebUI3.Events;

namespace DMX.WebUI3.Services;

public class EventsManager
{
    public event AsyncEventHandler<ZoomCommandEventArgs>? ZoomCommand;


    public async Task FireZoomCommandAsync(object sender, double zoom)
    {
        var tasks = new List<Func<Task>>();
        var ev = new ZoomCommandEventArgs(tasks) { Zoom = zoom };
        FireZoomCommand(sender, ev);

        await Task.WhenAll(tasks.Select(x => x()));
        // -- or --
        //foreach (var task in tasks)
        //{
        //    await task();
        //}
    }

    public void FireZoomCommand(object sender, ZoomCommandEventArgs ev)
    {
        ZoomCommand?.Invoke(sender, ev);
    }
}

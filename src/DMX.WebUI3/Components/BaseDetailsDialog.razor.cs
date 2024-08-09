// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3.Components;

public partial class BaseDetailsDialog
{
    [Parameter]
    public required RenderFragment DialogBody { get; set; }
    [Parameter]
    public required RenderFragment DialogButtons { get; set; }

    [Parameter]
    public AppSignal<DPoint> DragSignal { get; set; } = new();
    [Parameter]
    public AppSignal<DSize> ResizeSignal { get; set; } = new();

    public static async Task ShowAsync<TDialog>(DialogService dlg, TDialog d)
        where TDialog : BaseDetailsDialog
    {
        var result = await dlg.OpenAsync<TDialog>("foo");
    }
}

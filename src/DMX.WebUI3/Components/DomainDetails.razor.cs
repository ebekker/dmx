// DMX.
// Copyright (C) Eugene Bekker.

using Radzen;

namespace DMX.WebUI3.Components;

public partial class DomainDetails : IDisposable
{
    [Parameter]
    public required DmxDomain Domain { get; set; }
    [Parameter] public AppSignal<DPoint>? DragSignal { get; set; }
    [Parameter] public AppSignal<DSize>? ResizeSignal { get; set; }

    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (DragSignal != null)
            DragSignal.OnSignal += DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal += ResizeSignal_OnSignal;
    }

    public void Dispose()
    {
        if (DragSignal != null)
            DragSignal.OnSignal -= DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal -= ResizeSignal_OnSignal;
    }

    private void DragSignal_OnSignal(object? sender, DPoint arg)
    {
        // Do we want to save position?
        //AppState.EntityDetailsPoint = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }

    private void ResizeSignal_OnSignal(object? sender, DSize arg)
    {
        AppState.EntityDetailsSize = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }

    public static async Task<bool?> ShowAsync(DialogService dlg, DmxDomain domain,
        DPoint? initPoint = null, DSize? initSize = null)
    {
        var dragSignal = new AppSignal<DPoint>();
        var resizeSignal = new AppSignal<DSize>();
        var @params = new Dictionary<string, object>
        {
            [nameof(Domain)] = domain,
            [nameof(DragSignal)] = dragSignal,
            [nameof(ResizeSignal)] = resizeSignal,
        };
        var opts = new DialogOptions
        {
            Draggable = true,
            Resizable = true,
            Drag = x => dragSignal.FireSignal(dragSignal, x),
            Resize = x => resizeSignal.FireSignal(resizeSignal, x),
        };
        if (initPoint != null)
        {
            opts.Left = initPoint.Value.X + "px";
            opts.Top = initPoint.Value.Y + "px";
        }
        if (initSize != null)
        {
            opts.Width = initSize.Value.Width + "px";
            opts.Height = initSize.Value.Height + "px";
        }

        var result = (bool?)await dlg.OpenAsync<DomainDetails>(
            $"Domain '{domain.Name}'", @params, opts);

        return result;
    }
}

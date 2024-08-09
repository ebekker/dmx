// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3.Components;

public partial class AttributeDetails : IDisposable
{
    [Parameter] public DmxAttribute Attribute { get; set; } = default!;
    [Parameter] public AppSignal<DPoint>? DragSignal { get; set; }
    [Parameter] public AppSignal<DSize>? ResizeSignal { get; set; }

    [Inject] private IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;
    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;

    List<DmxDomain>? _domains = null;

    protected override async Task OnInitializedAsync()
    {
        if (DragSignal != null)
            DragSignal.OnSignal += DragSignal_OnSignal;
        if (ResizeSignal != null)
            ResizeSignal.OnSignal += ResizeSignal_OnSignal;

        using var db = await DbFactory.CreateDbContextAsync();
        _domains = await db.Domains.ToListAsync();
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
        AppState.AttributeDetailsPoint = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }

    private void ResizeSignal_OnSignal(object? sender, DSize arg)
    {
        AppState.AttributeDetailsSize = arg;
        AppEvents.FireAppStateChanged(sender ?? this);
    }

    public static async Task<DetailsResult?> ShowAsync(DialogService dlg, DmxAttribute attr,
        DPoint? initPoint = null, DSize? initSize = null)
    {
        var dragSignal = new AppSignal<DPoint>();
        var resizeSignal = new AppSignal<DSize>();
        var @params = new Dictionary<string, object>
        {
            [nameof(Attribute)] = attr,
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

        var result = (DetailsResult?)await dlg.OpenAsync<AttributeDetails>(
            $"Attribute '{attr.Entity.Name}.{attr.Name}'", @params, opts);

        return result;
    }

}

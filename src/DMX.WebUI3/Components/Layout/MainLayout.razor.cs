// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Geometry;

namespace DMX.WebUI3.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;
    [Inject] private AppChanges AppChanges { get; set; } = default!;

    private bool _disposedValue;

    private Rectangle? _diagramContainer;

    protected override void OnInitialized()
    {
        AppEvents.OnAppStateChanged += AppEvents_OnAppStateChanged;
        AppEvents.OnDiagramContainerChanged += AppEvents_OnDiagramContainerChanged;

        AppEvents.OnChangeAdded += AppEvents_OnChangeAdded;
        AppEvents.OnChangeUndo += AppEvents_OnChangeUndo;
        AppEvents.OnChangeRedo += AppEvents_OnChangeRedo;
    }

    protected virtual void OnDispose()
    {
        AppEvents.OnChangeRedo -= AppEvents_OnChangeRedo;
        AppEvents.OnChangeUndo -= AppEvents_OnChangeUndo;
        AppEvents.OnChangeAdded -= AppEvents_OnChangeAdded;

        AppEvents.OnAppStateChanged -= AppEvents_OnAppStateChanged;
        AppEvents.OnDiagramContainerChanged -= AppEvents_OnDiagramContainerChanged;
    }

    private void SetGrid(bool? gridLines)
    {
        AppState.GridLines = gridLines;
        AppEvents.FireVisualChanged(this);
    }

    private void SetGridSize(double size)
    {
        AppState.GridSize = size;
        AppEvents.FireVisualChanged(this);
    }

    private void AppEvents_OnAppStateChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void AppEvents_OnDiagramContainerChanged(object? sender, Rectangle? arg)
    {
        _diagramContainer = arg;
        StateHasChanged();
    }

    private void AppEvents_OnChangeRedo(object? sender, AppChanges.Change e)
    {
        StateHasChanged();
    }

    private void AppEvents_OnChangeUndo(object? sender, AppChanges.Change e)
    {
        StateHasChanged();
    }

    private void AppEvents_OnChangeAdded(object? sender, AppChanges.Change e)
    {
        StateHasChanged();
    }

    private async Task NewEntity()
    {
        await EntityDetails.ShowAsync(DialogSvc, new()
        {
            Name = "Entity1",
            Description = "First entity.",
            Attributes = new()
            {
                new()
                {
                    Name = "Attribute1",
                    IsPrimaryKey = true,
                },
                new()
                {
                    Name = "Attribute2",
                }
            }
        },
        initPoint: null, //AppState.EntityDetailsPoint,
        initSize: AppState.EntityDetailsSize);
    }

    private void NewRelationship()
    {

    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                OnDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MainLayout()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

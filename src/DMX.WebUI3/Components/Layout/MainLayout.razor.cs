// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Geometry;
using DMX.AppDB;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private IDbContextFactory<AppDbContext> DbFactory { get; set; } = default!;
    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;
    [Inject] private AppChanges AppChanges { get; set; } = default!;
    [Inject] private DbContextChangeBuilder ChangeBuilder { get; set; } = default!;

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

    private async Task NewDomain()
    {
        using var db = await DbFactory.CreateDbContextAsync();

        var allDomNames = await db.Domains
            .Select(x => x.Name).ToListAsync();
        var d = new DmxDomain()
        {
            Name = ModelTool.NextAvailableName(allDomNames, "Domain"),
        };
        var result = await DomainDetails.ShowAsync(DialogSvc, d,
            initPoint: null, //AppState.DomainDetailsPoint,
            initSize: AppState.DomainDetailsSize);

        if (result ?? false)
        {
            db.Domains.Add(d);
            var change = ChangeBuilder.Build(db)!;

            await db.SaveChangesAsync();
            AppChanges.Push(change);
            AppEvents.FireDataModelChanged(this);
        }
    }

    private Task NewEntity() => NewEntityAt();


    private async Task NewEntityAt(int? posX = null, int? posY = null)
    {
        using var db = await DbFactory.CreateDbContextAsync();

        var allEntNames = await db.Entities
            .Select(x => x.Name).ToListAsync();
        var e = new DmxEntity
        {
            Name = ModelTool.NextAvailableName(allEntNames, "Entity"),
            PosX = posX,
            PoxY = posY,
        };
        var result = await EntityDetails.ShowAsync(DialogSvc, e,
            initPoint: null, //AppState.EntityDetailsPoint,
            initSize: AppState.EntityDetailsSize)
            ?? DetailsResult.Cancel;

        if (result == DetailsResult.OK)
        {
            db.Entities.Add(e);
            var change = ChangeBuilder.Build(db)!;

            await db.SaveChangesAsync();
            AppChanges.Push(change);
            AppEvents.FireDataModelChanged(this);
        }
    }

    private async void NewRelationship()
    {
        using var db = await DbFactory.CreateDbContextAsync();

        var r = new DmxRelationship
        {
            //Parent = firstEnt,
            //Child = firstEnt,
        };
        var result = await NewRelationshipDetails.ShowAsync(DialogSvc, db, r,
            initPoint: null, //AppState.EntityDetailsPoint,
            initSize: AppState.RelationshipDetailsSize)
            ?? false;

        if (result)
        {
            db.Relationships.Add(r);
            var change = ChangeBuilder.Build(db);

            await db.SaveChangesAsync();
            AppChanges.Push(change);
            AppEvents.FireDataModelChanged(this);
        }
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

// DMX.
// Copyright (C) Eugene Bekker.

using System.Reflection;
using DMX.AppDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DMX.WebUI3.Components.Pages;

public partial class Home : IDisposable
{
    [Inject] private ILogger<Home> Logger { get; set; } = default!;
    [Inject] private IDbContextFactory<AppDbContext> DBFactory { get; set; } = default!;
    [Inject] private DialogService DialogSvc { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;
    [Inject] private AppChanges AppChanges { get; set; } = default!;
    [Inject] private DbContextChangeBuilder ChangeBuilder { get; set; } = default!;

    [Inject] private AppServices Services { get; set; } = default!;

    List<DmxDomain> _doms = [];
    List<DmxEntity> _ents = [];
    List<DmxRelationship> _rels = [];
    List<DmxShape> _shapes = [];

    protected override async Task OnInitializedAsync()
    {
        AppEvents.OnDataModelChanged += AppEvents_OnDataModelChanged;
        AppEvents.OnDoubleClickElement += AppEvents_OnDoubleClickElement;

        Services.Events.ZoomCommand += Events_ZoomCommand;

        await ComputeDetails();
    }

    public void Dispose()
    {
        Services.Events.ZoomCommand -= Events_ZoomCommand;

        AppEvents.OnDoubleClickElement -= AppEvents_OnDoubleClickElement;
        AppEvents.OnDataModelChanged -= AppEvents_OnDataModelChanged;
    }

    private void Events_ZoomCommand(object? sender, Events.ZoomCommandEventArgs ev)
    {
        //ev.AddInvoke(() =>
        //{
        //    Console.WriteLine("Zoom Command: " + ev.Zoom);
        //    return Task.CompletedTask;
        //});
    }

    async Task ZoomOut()
    {
        AppState.Zoom -= 0.1;
        await Services.Events.FireZoomCommandAsync(this, AppState.Zoom);
    }

    async Task ZoomIn()
    {
        AppState.Zoom += 0.1;
        await Services.Events.FireZoomCommandAsync(this, AppState.Zoom);
    }

    async Task ComputeDetails()
    {
        using var db = await DBFactory.CreateDbContextAsync();

        _doms = await db.Domains.ToListAsync();
        _ents = await db.Entities
            .Include(x => x.Attributes)
            .Include(x => x.RelationshipsAsChild)
            .Include(x => x.RelationshipsAsParent)
            .ToListAsync();
        _rels = await db.Relationships
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Attributes).ThenInclude(x => x.Child)
            .Include(x => x.Attributes).ThenInclude(x => x.Parent)
            .ToListAsync();
        _shapes = await db.Shapes.ToListAsync();
    }

    private void AppEvents_OnDataModelChanged(object? sender, AsyncEventArgs e)
    {
        e.AddTask(AppEvents_OnDataModelChangedAsync());
    }

    private async Task AppEvents_OnDataModelChangedAsync()
    {
        await ComputeDetails();
        StateHasChanged();
    }

    private void AppEvents_OnDoubleClickElement(object? sender, object element)
    {
        Task? t = null;

        if (element is DmxDomain d)
        {
            t = EditDomain(d);
        }
        else if(element is DmxEntity e)
        {
            t = EditEntity(e);
        }
        else if (element is DmxRelationship r)
        {
            t = EditRelationship(r);
        }

        if (t != null)
        {
            // What to do with t???
            //_ = Task.WaitAny(t);
        }
    }

    bool OriginMarkerEnabled
    {
        get => AppState.OriginMarkerColor != null;
        set
        {
            AppState.OriginMarkerColor = value
                ? "rgb(200, 0, 0, 0.2)"
                : null;
        }
    }

    async Task EditDomain(DmxDomain d)
    {
        using var db = await DBFactory.CreateDbContextAsync();

        db.Attach(d);
        var result = await DomainDetails.ShowAsync(DialogSvc, d,
            initPoint: null, //AppState.DomainDetailsPoint,
            initSize: AppState.DomainDetailsSize);

        var change = ChangeBuilder.Build(db);
        if (change == null)
            return;

        if (result ?? false)
        {
            await db.SaveChangesAsync();
            AppChanges.Push(change);
            await AppEvents.FireDataModelChangedAsync(this);
        }
        else
        {
            await change.Undo();
        }
    }

    /*
    async Task EditDomainOld(DmxDomain d)
    {
        using var db = await DBFactory.CreateDbContextAsync();

        db.Attach(d);
        var result = await DomainDetails.ShowAsync(DialogSvc, d,
            initPoint: null, //AppState.DomainDetailsPoint,
            initSize: AppState.DomainDetailsSize);

        var undoAdd = new List<object>();
        var undoDel = new List<object>();
        var undoMod = new List<(object ent, Dictionary<PropertyInfo, (object? oldVal, object? newVal)> modProps)>();

        foreach (var ent in db.ChangeTracker.Entries())
        {
            if (ent.State == EntityState.Added)
            {
                undoAdd.Add(ent.Entity);
            }
            else if (ent.State == EntityState.Deleted)
            {
                undoDel.Add(ent.Entity);
            }
            else if (ent.State == EntityState.Modified)
            {
                var modProps = new Dictionary<PropertyInfo, (object? oldVal, object? newVal)>();
                foreach (var p in ent.Properties)
                {
                    if (!p.IsModified)
                        continue;
                    var pi = p.Metadata.PropertyInfo;
                    if (pi == null)
                        throw new Exception("modification does not have a corresponding class property");
                    modProps[pi] = (p.OriginalValue, p.CurrentValue);
                }

                undoMod.Add((ent.Entity, modProps));
            }
        }

        if (result ?? false)
        {
            db.SaveChanges();
            AppChanges.Push("model changes", redo, undo);
            AppEvents.FireDataModelChanged(this);
        }
        else
        {
            await undo();
        }

        Task redo()
        {
            using var ldb = DBFactory.CreateDbContext();
            foreach (var e in undoAdd)
            {
                var ent = ldb.Attach(e);
                ent.State = EntityState.Added;
            }
            foreach (var e in undoDel)
            {
                var ent = ldb.Attach(e);
                ent.State = EntityState.Deleted;
            }
            foreach (var e in undoMod)
            {
                var ent = ldb.Attach(e.ent);
                ent.State = EntityState.Modified;
                Console.WriteLine("Redoing Mod:");
                foreach (var p in e.modProps)
                {
                    var val = p.Value.newVal;
                    p.Key.SetValue(e.ent, val);
                    Console.WriteLine($"  * {p.Key.Name} = {val}");
                }
            }

            ldb.SaveChanges();
            AppEvents.FireDataModelChanged(this);

            return Task.CompletedTask;
        }

        Task undo()
        {
            using var ldb = DBFactory.CreateDbContext();
            foreach (var e in undoAdd)
            {
                var ent = ldb.Attach(e);
                ent.State = EntityState.Deleted;
            }
            foreach (var e in undoDel)
            {
                var ent = ldb.Attach(e);
                ent.State = EntityState.Added;
            }
            foreach (var e in undoMod)
            {
                var ent = ldb.Attach(e.ent);
                ent.State = EntityState.Modified;
                Console.WriteLine("Undoing Mod:");
                foreach (var p in e.modProps)
                {
                    var val = p.Value.oldVal;
                    p.Key.SetValue(e.ent, val);
                    Console.WriteLine($"  * {p.Key.Name} = {val}");
                }
            }

            ldb.SaveChanges();
            AppEvents.FireDataModelChanged(this);

            return Task.CompletedTask;
        }
    }
    */

    async Task EditEntity(DmxEntity e)
    {
        using var db = await DBFactory.CreateDbContextAsync();

        db.Attach(e);
        var result = await EntityDetails.ShowAsync(DialogSvc, e,
            initPoint: null, //AppState.EntityDetailsPoint,
            initSize: AppState.EntityDetailsSize)
            ?? DetailsResult.Cancel;

        if (result == DetailsResult.Delete)
        {
            Logger.LogInformation("Deleting Entity");
            Services.Model.RemoveEntity(db, e);
        }

        var change = ChangeBuilder.Build(db);
        if (change == null)
            return;

        if (result == DetailsResult.OK || result == DetailsResult.Delete)
        {
            await db.SaveChangesAsync();
            AppChanges.Push(change);
            await AppEvents.FireDataModelChangedAsync(this);
        }
        else // Assume DetailsResult.Cancel
        {
            await change.Undo();
        }
    }

    async Task EditRelationship(DmxRelationship r)
    {
        using var db = await DBFactory.CreateDbContextAsync();
        db.Attach(r);
        var result = await RelationshipDetails.ShowAsync(DialogSvc, r,
            initPoint: null, //AppState.RelationshipDetailsPoint,
            initSize: AppState.RelationshipDetailsSize)
            ?? DetailsResult.Cancel;

        if (result == DetailsResult.Delete)
        {
            Services.Model.RemoveRelationship(db, r);
        }

        var change = ChangeBuilder.Build(db);
        if (change == null)
            return;

        if (result == DetailsResult.OK || result == DetailsResult.Delete)
        {
            await db.SaveChangesAsync();
            AppChanges.Push(change);
            await AppEvents.FireDataModelChangedAsync(this);
        }
        else
        {
            await change.Undo();
        }
    }
}

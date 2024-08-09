// DMX.
// Copyright (C) Eugene Bekker.

using System.Reflection;
using DMX.AppDB;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3;

public class DbContextChangeBuilder
{
    private readonly ILogger<DbContextChangeBuilder> _logger;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly AppEvents _events;
    private readonly AppChanges _changes;

    public DbContextChangeBuilder(ILogger<DbContextChangeBuilder> logger,
        IDbContextFactory<AppDbContext> dbFactory,
        AppEvents events, AppChanges changes)
    {
        _logger = logger;
        _dbFactory = dbFactory;
        _events = events;
        _changes = changes;
    }

    public AppChanges.Change Build(AppDbContext trackedChanges, string? label = null)
    {
        label ??= "data model changes";

        var undoAdd = new List<object>();
        var undoDel = new List<object>();
        var undoMod = new List<(object ent, Dictionary<PropertyInfo, (object? oldVal, object? newVal)> modProps)>();

        foreach (var ent in trackedChanges.ChangeTracker.Entries())
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

        var change = new AppChanges.Change(label, redo, undo);
        //_changes.Push(change);
        return change;

        Task redo()
        {
            using var ldb = _dbFactory.CreateDbContext();
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
                foreach (var p in e.modProps)
                {
                    var val = p.Value.newVal;
                    p.Key.SetValue(e.ent, val);
                }
            }

            ldb.SaveChanges();
            _events.FireDataModelChanged(this);

            return Task.CompletedTask;
        }

        Task undo()
        {
            using var ldb = _dbFactory.CreateDbContext();
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
                foreach (var p in e.modProps)
                {
                    var val = p.Value.oldVal;
                    p.Key.SetValue(e.ent, val);
                }
            }

            ldb.SaveChanges();
            _events.FireDataModelChanged(this);

            return Task.CompletedTask;
        }
    }
}

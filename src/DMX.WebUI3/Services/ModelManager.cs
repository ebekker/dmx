// DMX.
// Copyright (C) Eugene Bekker.


// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;

namespace DMX.WebUI3.Services;

public class ModelManager
{
    private readonly ILogger<ModelManager> _logger;

    public ModelManager(ILogger<ModelManager> logger)
    {
        _logger = logger;
    }

    public string NextAvailableName(IEnumerable<string> names,
        string prefix, int? start = 1)
    {
        var i = start;
        string nextName;
        while (names.Contains(nextName = prefix + i))
        {
            i ??= 0;
            i++;
        }
        return nextName;
    }

    public void RemoveAttribute(AppDbContext db,
        DmxEntity ent, DmxAttribute attr)
    {
        if (!ent.Attributes.Contains(attr))
            return;

        _logger.LogInformation("Removing attribute...");

        ent.Attributes.Remove(attr);
        db.Remove(attr);

        var children = db.RelationshipPairs
            .Where(x => x.Parent == attr)
            .ToList();
        foreach (var c in children)
        {
            db.Remove(c);
        }

        var parents = db.RelationshipPairs
            .Where(x => x.Child == attr)
            .Select(x => x.Relationship)
            .ToList();
        foreach (var p in parents)
        {
            RemoveRelationship(db, p);
        }

        _logger.LogInformation("Attribute removed");
    }

    public void RemoveRelationship(AppDbContext db,
        DmxRelationship rel)
    {
        _logger.LogInformation("Removing relationship...");

        foreach (var rp in rel.Attributes)
        {
            db.Remove(rp);
        }
        db.Remove(rel);

        _logger.LogInformation("Relationship removed");
    }

    public void RemoveEntity(AppDbContext db,
        DmxEntity ent)
    {
        _logger.LogInformation("Removing entity...");

        if (ent.RelationshipsAsParent?.Count > 0)
        {
            foreach (var r in ent.RelationshipsAsParent)
            {
                RemoveRelationship(db, r);
            }
        }
        if (ent.Attributes?.Count > 0)
        {
            foreach (var a in ent.Attributes)
            {
                RemoveAttribute(db, ent, a);
            }
        }
        db.Remove(ent);

        _logger.LogInformation("Entity removed");
    }
}

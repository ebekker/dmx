// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;

namespace DMX.WebUI3;

public class ModelTool
{
    public static string NextAvailableName(IEnumerable<string> names,
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

    public static void RemoveAttribute(AppDbContext db,
        DmxEntity ent, DmxAttribute attr)
    {
        if (!ent.Attributes.Contains(attr))
            return;

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
    }

    public static void RemoveRelationship(AppDbContext db,
        DmxRelationship rel)
    {
        foreach (var rp in rel.Attributes)
        {
            db.Remove(rp);
        }
        db.Remove(rel);
    }
}

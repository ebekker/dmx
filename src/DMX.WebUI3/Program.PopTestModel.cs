// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3;

public partial class Program
{
    static async Task PopulateTestModel(AppDbContext db)
    {
        var doms = await db.Domains.ToListAsync();
        var ents = await db.Entities
            .Include(x => x.Attributes).ThenInclude(x => x.Domain)
            .Include(x => x.RelationshipsAsChild)
            .ToListAsync();
        var rels = await db.Relationships
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .Include(x => x.Attributes).ThenInclude(x => x.Child)
            .Include(x => x.Attributes).ThenInclude(x => x.Parent)
            .ToListAsync();
        var shapes = await db.Shapes
            .ToListAsync();

        await AddDomains(db, doms);
        await db.SaveChangesAsync();

        await AddEntities(db, ents);
        await db.SaveChangesAsync();

        await AddRelationships(db, rels);
        await db.SaveChangesAsync();

        await AddShapes(db, shapes);
        await db.SaveChangesAsync();
    }

    static Task AddDomains(AppDbContext db, List<DmxDomain> existing)
    {
        foreach (var td in _testDoms)
        {
            if (null == existing.Find(x => string.Equals(td.Name, x.Name,
                StringComparison.OrdinalIgnoreCase)))
            {
                db.Domains.Add(td);
            }
        }

        return Task.CompletedTask;
    }

    static Task AddEntities(AppDbContext db, List<DmxEntity> existing)
    {
        foreach (var te in TestEnts(db))
        {
            if (null == existing.Find(x => string.Equals(te.Name, x.Name,
                StringComparison.OrdinalIgnoreCase)))
            {
                db.Entities.Add(te);
            }
        }

        return Task.CompletedTask;
    }

    static Task AddRelationships(AppDbContext db, List<DmxRelationship> existing)
    {
        foreach (var tr in TestRels(db))
        {
            if (null == existing.Find(x => string.Equals(tr.Name, x.Name,
                StringComparison.OrdinalIgnoreCase)))
            {
                db.Relationships.Add(tr);
            }
        }

        return Task.CompletedTask;
    }

    static Task AddShapes(AppDbContext db, List<DmxShape> existing)
    {
        foreach (var ts in _testShapes)
        {
            if (null == existing.Find(x => ts.Kind == x.Kind 
                && string.Equals(ts.Details, x.Details,
                    StringComparison.OrdinalIgnoreCase)))
            {
                db.Shapes.Add(ts);
            }
        }

        return Task.CompletedTask;
    }

    static readonly List<DmxShape> _testShapes = new()
    {
        new()
        {
            Kind = DmxShape.ShapeKind.Note,
            Details = "This is a test Note!",
            PosX = 10,
            PosY = 20,
            DimW = 100,
            DimH = 100,
        },
        new()
        {
            Kind = DmxShape.ShapeKind.Note,
            Details = "This is another test Note!",
            PosX = 150,
            PosY = 20,
            DimW = 100,
            DimH = 100,
        },
    };

    static readonly List<DmxDomain> _testDoms = new()
    {
        new() { Name = "String", },
        new() { Name = "BLOB", },
        new() { Name = "CLOB", },
        new() { Name = "Date", },
        new() { Name = "Time", },
        new() { Name = "DateTime", },
        new() { Name = "Timestamp", },
        new() { Name = "UUID", },
        new() { Name = "Integer", },
        new() { Name = "Real", },
        new() { Name = "Boolean", },
    };

    static List<DmxEntity>? _TestEnts;
    static List<DmxEntity> TestEnts(AppDbContext db) =>
        _TestEnts ??= new()
        {
            new()
            {
                Name = "Author",
                Attributes = new()
                {
                    new() {
                        Name = "AuthorId",
                        SortOrder = 100,
                        Domain = db.Domains.First(x => x.Name == "Integer"),
                    },
                    new() {
                        Name = "Label",
                        SortOrder = 200,
                        Domain = db.Domains.First(x => x.Name == "String"),
                    },
                    new() {
                        Name = "Email",
                        SortOrder = 300,
                        Domain = db.Domains.First(x => x.Name == "String"),
                    },
                }
            },
            new()
            {
                Name = "Blog",
                Attributes = new()
                {
                    new() {
                        Name = "BlogId",
                        IsPrimaryKey = true,
                        SortOrder = 100,
                        Domain = db.Domains.First(x => x.Name == "Integer"),
                    },
                    new() {
                        Name = "Title",
                        SortOrder = 200,
                        Domain = db.Domains.First(x => x.Name == "String"),
                    },
                    new() {
                        Name = "AuthorId",
                        SortOrder = 300,
                        Domain = db.Domains.First(x => x.Name == "Integer"),
                    },
                }
            },
            new()
            {
                Name = "Post",
                Attributes = new()
                {
                    new() {
                        Name = "BlogId",
                        IsPrimaryKey = true,
                        SortOrder = 100,
                        Domain = db.Domains.First(x => x.Name == "Integer")
                    },
                    new() {
                        Name = "PostDate",
                        IsPrimaryKey = true,
                        SortOrder = 200,
                        Domain = db.Domains.First(x => x.Name == "DateTime")
                    },
                    new() {
                        Name = "AuthorId",
                        SortOrder = 300,
                        Domain = db.Domains.First(x => x.Name == "Integer")
                    },
                    new() {
                        Name = "Title",
                        IsRequired = true,
                        SortOrder = 400,
                        Domain = db.Domains.First(x => x.Name == "String")
                    },
                    new() {
                        Name = "MarkupContent",
                        SortOrder = 500,
                        Domain = db.Domains.First(x => x.Name == "CLOB")
                    },
                }
            }
        };

    static List<DmxRelationship>? _TestRels;
    static List<DmxRelationship> TestRels(AppDbContext db) =>
        _TestRels ??= new()
        {
            new()
            {
                Name = "BlogAuthor",
                Child = db.Entities.First(x => x.Name == "Blog"),
                Parent = db.Entities.First(x => x.Name == "Author"),
                Attributes = new()
                {
                    new()
                    {
                        Child = db.Entities.First(x => x.Name == "Blog")
                            .Attributes.First(x => x.Name == "BlogId"),
                        Parent = db.Entities.First(x => x.Name == "Author")
                            .Attributes.First(x => x.Name == "AuthorId"),
                    }
                }
            },
            new()
            {
                Name = "PostBlog",
                Child = db.Entities.First(x => x.Name == "Post"),
                Parent = db.Entities.First(x => x.Name == "Blog"),
                Attributes = new()
                {
                    new()
                    {
                        Child = db.Entities.First(x => x.Name == "Post")
                            .Attributes.First(x => x.Name == "BlogId"),
                        Parent = db.Entities.First(x => x.Name == "Blog")
                            .Attributes.First(x => x.Name == "BlogId"),
                    }
                }
            },
            new()
            {
                Name = "PostAuthor",
                Child = db.Entities.First(x => x.Name == "Post"),
                Parent = db.Entities.First(x => x.Name == "Author"),
                Attributes = new()
                {
                    new()
                    {
                        Child = db.Entities.First(x => x.Name == "Post")
                            .Attributes.First(x => x.Name == "AuthorId"),
                        Parent = db.Entities.First(x => x.Name == "Author")
                            .Attributes.First(x => x.Name == "AuthorId"),
                    }
                }
            },
        };

}

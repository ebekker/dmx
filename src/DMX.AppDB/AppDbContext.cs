// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DMX.AppDB;

public class AppDbContext : DbContext
{
    public const string DefaultConnectionStringName = nameof(DMX);

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=./dmx-model.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly,
            // Optional filter:
            (type) => true);
    }

    public DbSet<DmxDomain> Domains { get; set; } = default!;
    public DbSet<DmxEntity> Entities { get; set; } = default!;
    public DbSet<DmxRelationship> Relationships { get; set; } = default!;
    public DbSet<DmxRelationshipPair> RelationshipPairs { get; set; } = default!;
    public DbSet<DmxShape> Shapes { get; set; } = default!;
}

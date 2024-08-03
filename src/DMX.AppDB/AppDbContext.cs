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

    public DbSet<DmxEntity> Entities { get; set; } = default!;

}

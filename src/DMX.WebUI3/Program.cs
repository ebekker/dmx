// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using DMX.WebUI3.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DMX.WebUI3;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddOptions<AppConfig>().Bind(
            builder.Configuration.GetSection(AppConfig.DefaultSectionName));

        builder.Services.AddDbContextFactory<AppDbContext>((services, optionsBuilder) =>
        {
            optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString(
                AppDbContext.DefaultConnectionStringName));
        });

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddRadzenComponents();

        builder.Services.AddScoped<AppState>();
        builder.Services.AddScoped<AppEvents>();
        builder.Services.AddScoped<AppChanges>();
        builder.Services.AddScoped<DbContextChangeBuilder>();

        var app = builder.Build();
        var log = app.Services.GetRequiredService<ILogger<Program>>();
        var cfg = app.Services.GetRequiredService<IOptions<AppConfig>>().Value;
        var dbf = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();

        if (cfg.ApplyMigrations)
        {
            log.LogInformation("Migration requested...");
            using var db = await dbf.CreateDbContextAsync();
            log.LogInformation("...migrating...");
            await db.Database.MigrateAsync();
            log.LogInformation("...migration completed.");
        }
        else if (cfg.SkipMigrationsCheck)
        {
            log.LogInformation("SKIPPING migration check");
        }
        else
        {
            log.LogInformation("Checking if migration is needed...");
            using var db = await dbf.CreateDbContextAsync();
            var migs = await db.Database.GetPendingMigrationsAsync();
            if (migs.Any())
            {
                log.LogWarning("Found {count} pending migration(s)",
                    migs.Count());
                log.LogWarning("Neither Apply nor Skip were indicated, EXITING");
                return;
            }
            log.LogInformation("...no pending migrations found");
        }

        if (cfg.PopulateTestModel)
        {
            log.LogWarning("Requested to populate test model data...");
            using var db = await dbf.CreateDbContextAsync();
            await PopulateTestModel(db);
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this
            // for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        //app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        if (cfg.SkipProgramRun)

        {
            log.LogWarning("Program Run skip requested");
        }
        else
        {
            log.LogInformation("Program Running...");
            await app.RunAsync();
        }
    }
}

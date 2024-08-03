// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DMX.AppDB;

/// <summary>
/// Supports design-time EF Core tooling, by resolving the
/// <see cref="AppDbContext"> instance and supporting dependencies.
/// </summary>
/// <remarks>
/// For details on EF Core design-time factory, see <see
/// href="https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory">here</see>.
/// This implementation constructs a minimal configuration
/// source that supports (in precedence order):
/// <list type="bullet">
/// <item>JSON <c>./appsettings.json</c> (required)</item>
/// <item>JSON <c>./_IGNORE/appsettings.LOCAL.json</c> (optional)</item>
/// <item>non-prefixed Environment Variables</item>
/// <item>command-line arguments</item>
/// </list>
/// </remarks>
internal class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public DesignTimeDbContextFactory()
    {
        LogInfo($"CONSTRUCTED: {nameof(DesignTimeDbContextFactory)} (DTDbCF)");
    }

    public AppDbContext CreateDbContext(string[] args)
    {
        try
        {
            LogInfo("CWD: " + Directory.GetCurrentDirectory());
            var cfgBase = Directory.GetCurrentDirectory();
            var cfgPath1 = Path.Combine(cfgBase, "appsettings.json");
            var cfgPath2 = Path.Combine(cfgBase, "_IGNORE/appsettings.LOCAL.json");

            var cb = new ConfigurationBuilder();
            cb.AddJsonFile(cfgPath1, optional: false);
            cb.AddJsonFile(cfgPath2, optional: true);
            cb.AddEnvironmentVariables();
            cb.AddCommandLine(args);

            var sc = new ServiceCollection();
            sc.AddLogging();
            sc.AddSingleton(cb.Build());
            sc.AddDbContext<AppDbContext>((services, optionsBuilder) =>
            {
                var cfg = services.GetRequiredService<IConfigurationRoot>();
                var con = cfg.GetConnectionString(AppDbContext.DefaultConnectionStringName);
                var asm = typeof(AppDbContext).Assembly.FullName;

                LogInfo($"Connection Str: [{con}]");
                LogInfo($"Migrations ASM: [{asm}]");

                optionsBuilder.UseSqlite(con,
                    builder => builder.MigrationsAssembly(asm));
            });

            var sp = sc.BuildServiceProvider();
            return sp.GetRequiredService<AppDbContext>();
        }
        catch (Exception ex)
        {
            LogErr("Failed to create design-time DbContext:");
            LogErr(ex);
            throw;
        }
    }

    protected const string LogPrefix = "[DTDbCF] ";

    protected virtual void LogInfo(object fmt, params object[] args)
    {
        if (args.Length == 0)
            Console.WriteLine($"{LogPrefix} {fmt}");
        else
            Console.WriteLine($"{LogPrefix} {fmt}", args);
    }

    protected virtual void LogErr(object fmt, params object[] args)
    {
        if (args.Length == 0)
            Console.Error.WriteLine($"{LogPrefix} {fmt}");
        else
            Console.Error.WriteLine($"{LogPrefix} {fmt}", args);
    }
}

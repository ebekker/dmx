// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using DMX.WebApp.Components;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContextFactory<AppDbContext>((services, optionsBuilder) =>
        {
            optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString(
                AppDbContext.DefaultConnectionStringName));
        });

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this
            // for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseAntiforgery();

        //app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}

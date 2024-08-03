// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using DMX.WebUI2.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace DMX.WebUI2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddDbContextFactory<AppDbContext>((services, optionsBuilder) =>
        {
            optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString(
                AppDbContext.DefaultConnectionStringName));
        });

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddFluentUIComponents();

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

        app.UseStaticFiles();
        app.UseAntiforgery();

        //app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}

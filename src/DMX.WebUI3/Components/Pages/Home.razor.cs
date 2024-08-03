// DMX.
// Copyright (C) Eugene Bekker.

using DMX.AppDB;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3.Components.Pages;

public partial class Home
{
    [Inject] private IDbContextFactory<AppDbContext> DBFactory { get; set; } = default!;

    List<DmxDomain> _doms = new()
    {
        new() { Name = "Domain1", },
        new() { Name = "Domain2", },
        new() { Name = "Domain3", },
    };

    List<DmxEntity> _ents = new()
    {
        new() { Name = "Entity1" },
        new() { Name = "Entity2" },
        new() { Name = "Entity3" },
        new() { Name = "Entity4" },
    };

    List<DmxRelationship> _rels = new()
    {
        new() { Name = "Rel1" },
        new() { Name = "Rel2" },
        new() { Name = "Rel3" },
        new() { Name = "Rel4" },
    };

    protected override async Task OnInitializedAsync()
    {
        var db = await DBFactory.CreateDbContextAsync();


    }

}

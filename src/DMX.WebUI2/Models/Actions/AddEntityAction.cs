// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI2.Models.Actions;

public class AddEntityAction : ActionBase
{
    public AddEntityAction(string? name = null)
    {
        Name = name ?? "Entity1";
    }

    public string Name { get; set; }

    public override string Label => $"""
        Added Entity "{Name}"
        """;
}

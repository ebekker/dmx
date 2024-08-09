// DMX.
// Copyright (C) Eugene Bekker.

using System.Runtime.CompilerServices;
using Blazor.Diagrams.Core.Models;

namespace DMX.WebUI3.Components.Models;

public class EntityNodeModel : NodeModel
{
    public event EventHandler? OnUpdate;

    public EntityNodeModel(DmxEntity entity, BPoint? position = null) : base(position)
    {
        Entity = entity;
    }

    public DmxEntity Entity { get; private set; }

    public void Update(DmxEntity entity, int? x, int? y)
    {
        Entity = entity;
        SetPosition(x ?? Position.X, y ?? Position.Y);
        RefreshAll();
        Refresh();
        OnUpdate?.Invoke(this, EventArgs.Empty);
    }
}

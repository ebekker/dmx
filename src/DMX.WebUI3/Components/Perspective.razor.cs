// DMX.
// Copyright (C) Eugene Bekker.

using System.Text.Json;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Castle.Core;
using DMX.AppDB;
using DMX.WebUI3.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace DMX.WebUI3.Components;

public partial class Perspective : IDisposable
{
    [Inject] private IDbContextFactory<AppDbContext> DBFactory { get; set; } = default!;
    [Inject] private AppState AppState { get; set; } = default!;
    [Inject] private AppEvents AppEvents { get; set; } = default!;
    [Inject] private AppChanges AppChanges { get; set; } = default!;

    private bool? _gridLines;
    private double _gridSize = 20;

    private BlazorDiagramOptions DiagramOptions { get; set; } = new();
    private BlazorDiagram Diagram { get; set; } = null!;

    private bool _disposedValue;

    protected override async Task OnInitializedAsync()
    {
        AppEvents.OnResetToOrigin += AppEvents_OnResetToOrigin;
        AppEvents.OnVisualChanged += AppEvents_OnVisualChanged;
        AppEvents.OnDataModelChanged += AppEvents_OnDataModelChanged;

        _gridLines = AppState.GridLines;
        _gridSize = AppState.GridSize;

        DiagramOptions.AllowMultiSelection = true;
        DiagramOptions.Zoom.Enabled = false;
        DiagramOptions.Links.DefaultRouter = new NormalRouter();
        DiagramOptions.Links.DefaultPathGenerator = new SmoothPathGenerator();
        DiagramOptions.GridSize = _gridLines != null ? (int)(_gridSize) : null;

        Diagram = new BlazorDiagram(DiagramOptions);

        Diagram.Changed += Diagram_Changed;
        Diagram.ContainerChanged += Diagram_ContainerChanged;

        Diagram.PointerDoubleClick += Diagram_PointerDoubleClick;

        Diagram.RegisterComponent<EntityNodeModel, EntityNodeWidget>();

        await LoadModel();
    }

    protected virtual void OnDispose()
    {
        UnloadModel();

        Diagram.PointerDoubleClick -= Diagram_PointerDoubleClick;

        Diagram.ContainerChanged -= Diagram_ContainerChanged;
        Diagram.Changed -= Diagram_Changed;

        AppEvents.OnDataModelChanged -= AppEvents_OnDataModelChanged;
        AppEvents.OnVisualChanged -= AppEvents_OnVisualChanged;
        AppEvents.OnResetToOrigin -= AppEvents_OnResetToOrigin;
    }

    private void AppEvents_OnDataModelChanged(object? sender, EventArgs e)
    {
        Console.Error.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        Console.Error.WriteLine("Updating Perspective with DataModel changes...");
        // What do do with this task?
        var task = LoadModel(true);
    }

    private void Diagram_PointerDoubleClick(
        Blazor.Diagrams.Core.Models.Base.Model? model,
        Blazor.Diagrams.Core.Events.PointerEventArgs arg)
    {
        if (model == null)
            return;

        if (model is EntityNodeModel enm)
        {
            AppEvents.FireDoubleClickElement(this, enm.Entity);
        }
        else if (model is RelationshipLinkModel rlm)
        {
            AppEvents.FireDoubleClickElement(this, rlm.Relationship);
        }
        else if (model is LinkLabelModel llm && llm.Parent is RelationshipLinkModel rlm2)
        {
            AppEvents.FireDoubleClickElement(this, rlm2.Relationship);
        }
    }

    async Task LoadModel(bool refresh = false)
    {
        var defX = 1;
        var defY = 1;

        using var db = await DBFactory.CreateDbContextAsync();

        var ents = await db.Entities
            .Include(x => x.Attributes)
                .ThenInclude(x => x.Domain)
            .Include(X => X.RelationshipsAsChild)
                .ThenInclude(x => x.Attributes)
            .ToListAsync();
        var rels = await db.Relationships
            .Include(x => x.Child)
            .Include(x => x.Parent)
            .ToListAsync();

        var entToNode = new Dictionary<DmxEntity, EntityNodeModel>();

        var allNodes = Diagram.Nodes.OfType<EntityNodeModel>().ToList();
        var allLinks = Diagram.Links.OfType<RelationshipLinkModel>().ToList();

        foreach (var e in ents.OrderBy(x => x.ZOrder ?? 0))
        {
            if (refresh)
            {
                var nm = allNodes.FirstOrDefault(x => x.Entity.Id == e.Id);
                if (nm != null)
                {
                    Console.WriteLine("Refreshing Existing Entity: " + e.Name);
                    nm.Update(e, e.PosX, e.PoxY);
                    allNodes.Remove(nm);
                    entToNode.Add(e, nm);
                    nm.Refresh();
                    continue;
                }
            }

            var posX = e.PosX ?? (defX++ * 50);
            var posY = e.PoxY ?? (defY++ * 50);

            var node = new EntityNodeModel(e, position: new BPoint(posX, posY))
            {
                Title = e.Name
            };

            Diagram.Nodes.Add(node);
            node.Moved += Node_Moved;

            entToNode.Add(e, node);
        }

        if (refresh && allNodes.Count > 0)
        {
            foreach (var n in allNodes)
            {
                n.Moved -= Node_Moved;
                Diagram.Nodes.Remove(n);
            }
        }

        foreach (var r in rels)
        {
            var c = entToNode[r.Child];
            var p = entToNode[r.Parent];

            if (refresh)
            {
                var lm = allLinks.FirstOrDefault(x => x.Relationship.Id == r.Id);
                if (lm != null)
                {
                    lm.Update(r);
                    if (lm.Source is not EntityIntersectionAnchor cAnch || cAnch.Node != c)
                    {
                        lm.SetSource(new EntityIntersectionAnchor(c));
                    }
                    if (lm.Target is not EntityIntersectionAnchor pAnch || pAnch.Node != p)
                    {
                        lm.SetTarget(new EntityIntersectionAnchor(p));
                    }
                    lm.Labels.Clear();
                    lm.AddLabel(r.Name);
                    allLinks.Remove(lm);
                    lm.Refresh();
                    continue;
                }
            }

            //var cport = c.AddPort(PortAlignment.Top);
            //var cport = c.AddPort(new PortModel(c, PortAlignment.Top, new BPoint(5, 5), new BSize(15, 15)));
            //var pport = p.AddPort(PortAlignment.Bottom);
            //var link = Diagram.Links.Add(new LinkModel(cport, pport));
            //link.Router = new OrthogonalRouter(); // Only works with Ports
            //link.PathGenerator = new StraightPathGenerator();

            //var canchor = new SinglePortAnchor(cport);
            //var panchor = new SinglePortAnchor(pport);
            //var link = Diagram.Links.Add(new LinkModel(canchor, panchor));
            //link.PathGenerator = new StraightPathGenerator();

            // Custom Ortho Router for Entity Anchors
            //var canchor = new EntityAnchor(c);
            //var panchor = new EntityAnchor(p);
            //var link = Diagram.Links.Add(new RelationshipLinkModel(r, canchor, panchor));
            //link.Router = new OrthoAnchorRouter();
            //link.PathGenerator = new StraightPathGenerator();
            //link.SourceMarker = LinkMarker.Circle;
            //link.TargetMarker = LinkMarker.Arrow;

            var cAnchor = new EntityIntersectionAnchor(c);
            var pAnchor = new EntityIntersectionAnchor(p);
            var link = Diagram.Links.Add(new RelationshipLinkModel(r, cAnchor, pAnchor));
            link.AddLabel(r.Name);
            link.PathGenerator = new StraightPathGenerator();
            link.SourceMarker = LinkMarker.Circle;
            link.TargetMarker = LinkMarker.Arrow;

            // Bad experiment
            //var cAnchor = new EntityAnchor(c);
            //var pAnchor = new EntityAnchor(p);
            //var cAlign = EntityAnchor.GetAlignment(r, c, cAnchor, pAnchor, false);
            //var pAlign = EntityAnchor.GetAlignment(r, c, cAnchor, pAnchor, true);
            //var cPort = c.AddPort(cAlign);
            //var pPort = p.AddPort(pAlign);
            //var link = Diagram.Links.Add(new LinkModel(cPort, pPort));
            //link.Router = new OrthogonalRouter(); // Only works with Ports
            //link.PathGenerator = new StraightPathGenerator();
        }

        if (refresh && allLinks.Count > 0)
        {
            foreach (var l in allLinks)
            {
                Diagram.Links.Remove(l);
            }
        }

        Diagram.Refresh();
    }

    void UnloadModel()
    {
        foreach (var n in Diagram.Nodes)
        {
            if (n is EntityNodeModel)
                n.Moved -= Node_Moved;
        }
    }

    void BuildTestModel()
    {
        var node1 = new NodeModel(position: new BPoint(50, 50))
        {
            Title = "Node 1",
        };
        var node2 = new NodeModel(position: new BPoint(200, 100))
        {
            Title = "Node 2",
        };
        Console.WriteLine($"BEF: node1.Order={node1.Order}; node2.Order={node2.Order}");
        node1 = Diagram.Nodes.Add(node1);
        node2 = Diagram.Nodes.Add(node2);
        Console.WriteLine($"AFT: node1.Order={node1.Order}; node2.Order={node2.Order}");
        node1.Order = 10;
        Console.WriteLine($"AFT2: node1.Order={node1.Order}; node2.Order={node2.Order}");
        node2.Order = 10;
        Console.WriteLine($"AFT3: node1.Order={node1.Order}; node2.Order={node2.Order}");
        node1.Order = 20;
        Console.WriteLine($"AFT4: node1.Order={node1.Order}; node2.Order={node2.Order}");

        var leftPort = node2.AddPort(PortAlignment.Left);
        var rightPort = node2.AddPort(PortAlignment.Right);

        // The connection point will be the intersection of
        // a line going from the target to the center of the source
        var sourceAnchor = new ShapeIntersectionAnchor(node1);
        // The connection point will be the port's position
        var targetAnchor = new SinglePortAnchor(leftPort);
        var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
    }

    private void Node_Moved(Blazor.Diagrams.Core.Models.Base.MovableModel model)
    {
        if (model is EntityNodeModel enm)
        {
            var oldX = enm.Entity.PosX;
            var oldY = enm.Entity.PoxY;
            var newX = (int)model.Position.X;
            var newY = (int)model.Position.Y;

            Task redo()
            {
                using var db = DBFactory.CreateDbContext();
                db.Attach(enm.Entity);
                enm.Entity.PosX = newX;
                enm.Entity.PoxY = newY;
                db.SaveChanges();
                model.Position = new(newX, newY);
                model.Refresh();
                return Task.CompletedTask;
            }

            Task undo()
            {
                using var db = DBFactory.CreateDbContext();
                db.Attach(enm.Entity);
                enm.Entity.PosX = oldX;
                enm.Entity.PoxY = oldY;
                db.SaveChanges();
                model.Position = new(oldX!.Value, oldY!.Value);
                model.Refresh();
                return Task.CompletedTask;
            }

            redo();

            AppChanges.Push("entity moved", redo, undo);
        }
    }

    private void AppEvents_OnResetToOrigin(object? sender, EventArgs e)
    {
        Diagram.SetPan(0, 0);
    }

    private void AppEvents_OnVisualChanged(object? sender, EventArgs e)
    {
        Console.WriteLine("Visual Changed!");

        _gridLines = AppState.GridLines;
        _gridSize = AppState.GridSize;
        DiagramOptions.GridSize = _gridLines != null ? (int)(_gridSize) : null;

        Diagram.Refresh();
    }

    private void Diagram_ContainerChanged()
    {
        //Console.WriteLine($"Container Changed {DateTime.Now}:");
        //Console.WriteLine(JsonSerializer.Serialize(Diagram.Container,
        //    AppGlobals.WriteIndentedOptions));

        AppEvents.FireDiagramContainerChanged(this, Diagram.Container);
    }

    private void Diagram_Changed()
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                OnDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Perspective()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

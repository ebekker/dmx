// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Blazor.Diagrams;

using DPoint = Blazor.Diagrams.Core.Geometry.Point;
using System.Text.Json;

namespace DMX.WebUI3.Components;

public partial class Perspective : IDisposable
{
    [Inject] private AppEvents AppEvents { get; set; } = default!;

    private BlazorDiagram Diagram { get; set; } = null!;

    private bool _disposedValue;

    protected override void OnInitialized()
    {
        var options = new BlazorDiagramOptions
        {
            AllowMultiSelection = true,
            Zoom =
            {
                Enabled = false,
            },
            Links =
            {
                DefaultRouter = new NormalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator()
            },
        };

        Diagram = new BlazorDiagram(options);

        Diagram.Changed += Diagram_Changed;
        Diagram.ContainerChanged += Diagram_ContainerChanged;

        var node1 = new NodeModel(position: new DPoint(50, 50))
        {
            Title = "Node 1",
        };
        var node2 = new NodeModel(position: new DPoint(200, 100))
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

    protected virtual void OnDispose()
    {
        Diagram.ContainerChanged -= Diagram_ContainerChanged;
        Diagram.Changed -= Diagram_Changed;
    }

    private void Diagram_ContainerChanged()
    {
        Console.WriteLine($"Container Changed {DateTime.Now}:");
        Console.WriteLine(JsonSerializer.Serialize(Diagram.Container,
            AppGlobals.WriteIndentedOptions));

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

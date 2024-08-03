// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;

namespace DMX.WebUI2.Components;

public partial class Perspective
{
    private BlazorDiagram Diagram { get; set; } = null!;

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

        var node1 = new NodeModel(position: new Point(50, 50))
        {
            Title = "Node 1",
        };
        var node2 = new NodeModel(position: new Point(200, 100))
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
}

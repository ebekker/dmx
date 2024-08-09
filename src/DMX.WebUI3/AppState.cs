// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3;

public class AppState
{

    public DPoint? DomainDetailsPoint { get; set; }
    public DSize DomainDetailsSize { get; set; } = new(500, 500);

    public DPoint? EntityDetailsPoint { get; set; }
    public DSize EntityDetailsSize { get; set; } = new(500, 500);

    public DPoint? AttributeDetailsPoint { get; set; }
    public DSize AttributeDetailsSize { get; set; } = new(500, 500);

    public DPoint? RelationshipDetailsPoint { get; set; }
    public DSize RelationshipDetailsSize { get; set; } = new(500, 500);

    public string? OriginMarkerColor = "rgb(200, 0, 0, 0.2)";

    public bool? GridLines { get; set; } = true;
    public double GridSize { get; set; } = 20;

}

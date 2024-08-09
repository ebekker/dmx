// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace DMX.WebUI3.Components.Models;

public class EntityAnchor : Anchor
{
    public NodeModel Node { get; }

    public EntityAnchor(NodeModel model)
        : base(model)
    {
        Node = model;
    }

    public override BPoint? GetPosition(BaseLinkModel link, BPoint[] route)
    {
        if (Node.Size == null)
        {
            return null;
        }

        if (link is not RelationshipLinkModel relLink)
        {
            return GetDefaultPosition(link, route);
        }

        bool isParent = link.Target == this;

        BPoint center = Node.GetBounds()!.Center;
        BPoint? point = route.Length == 0
            ? Anchor.GetOtherPosition(link, isParent)
            : route[isParent ? (^1) : ((Index)0)];

        if (point == null)
        {
            return null;
        }

        // Compute a line from our center to the other node
        Line line = new Line(point, center);

        // Find all points on our shape that intersect that line
        var intersection = Node.GetShape().GetIntersectionsWithLine(line);

        // Return the closest point on that shape to the other node
        return Anchor.GetClosestPointTo(intersection, point);
    }

    public PortAlignment GetAlignment(BaseLinkModel link, BPoint[]? route = null) =>
        link is not RelationshipLinkModel relLink
            ? PortAlignment.Top : GetAlignment(
                relLink.Relationship, Node, relLink.Source, relLink.Target,
                link.Target == this, route);

    public static PortAlignment GetAlignment(DmxRelationship.EntityEdge edge)
    {
        return edge switch
        {
            DmxRelationship.EntityEdge.North => PortAlignment.Top,
            DmxRelationship.EntityEdge.South => PortAlignment.Bottom,
            DmxRelationship.EntityEdge.East => PortAlignment.Right,
            DmxRelationship.EntityEdge.West => PortAlignment.Left,
            _ => PortAlignment.Top,
        };
    }

    public static PortAlignment GetAlignment(DmxRelationship rel, NodeModel Node,
        Anchor source, Anchor target,
        bool isParentTarget, BPoint[]? route = null)
    {
        if (Node.Size == null)
        {
            return PortAlignment.Top;
        }

        if (isParentTarget && rel.ParentEdge != DmxRelationship.EntityEdge.Unspecified)
        {
            return GetAlignment(rel.ParentEdge);
        }
        if (!isParentTarget && rel.ChildEdge != DmxRelationship.EntityEdge.Unspecified)
        {
            return GetAlignment(rel.ChildEdge);
        }

        var nodeBounds = Node.GetBounds()!;
        BPoint center = nodeBounds.Center;
        BPoint? point = (route?.Length ?? 0) == 0
            ? (isParentTarget ? source : target).GetPlainPosition()
            : route![isParentTarget ? (^1) : ((Index)0)];

        if (point == null)
        {
            return PortAlignment.Top;
        }

        // Compute a line from our center to the other node
        Line line = new Line(point, center);

        // Find all points on our shape that intersect that line
        var intersection = Node.GetShape().GetIntersectionsWithLine(line);

        // Return the closest point on that shape to the other node
        var closestPoint = Anchor.GetClosestPointTo(intersection, point);

        if (closestPoint == null)
            return PortAlignment.Top;

        if (closestPoint.X == nodeBounds.Left)
        {
            if (closestPoint.Y == nodeBounds.Top)
                return PortAlignment.TopLeft;
            if (closestPoint.Y == nodeBounds.Bottom)
                return PortAlignment.BottomLeft;
            return PortAlignment.Left;
        }

        if (closestPoint.X == nodeBounds.Right)
        {
            if (closestPoint.Y == nodeBounds.Top)
                return PortAlignment.TopRight;
            if (closestPoint.Y == nodeBounds.Bottom)
                return PortAlignment.BottomRight;
            return PortAlignment.Right;
        }

        if (closestPoint.Y == nodeBounds.Bottom)
            return PortAlignment.Bottom;

        return PortAlignment.Top;
    }

    public bool IsVertical(BaseLinkModel link, BPoint[]? route)
    {
        if (Node.Size == null)
        {
            return false;
        }

        if (link is not RelationshipLinkModel relLink)
        {
            return false;
        }

        bool isParent = link.Target == this;

        var nodeBounds = Node.GetBounds()!;
        BPoint center = nodeBounds.Center;
        BPoint? point = route?.Length == 0
            ? Anchor.GetOtherPosition(link, isParent)
            : route![isParent ? (^1) : ((Index)0)];

        if (point == null)
        {
            return false;
        }

        // Compute a line from our center to the other node
        Line line = new Line(point, center);

        // Find all points on our shape that intersect that line
        var intersection = Node.GetShape().GetIntersectionsWithLine(line);

        // Return the closest point on that shape to the other node
        var closestPoint = Anchor.GetClosestPointTo(intersection, point);

        return closestPoint != null
            && (closestPoint.X == nodeBounds.Left
                || closestPoint.X == nodeBounds.Right);
    }


    /// Default implementation from <see cref="ShapeIntersectionAnchor" />.
    public BPoint? GetDefaultPosition(BaseLinkModel link, BPoint[] route)
    {
        if (Node.Size == null)
        {
            return null;
        }

        bool flag = link.Target == this;
        BPoint center = Node.GetBounds()!.Center;
        BPoint? point = ((route.Length == 0)
            ? Anchor.GetOtherPosition(link, flag)
            : route[flag ? (^1) : ((Index)0)]);

        if (point == null)
        {
            return null;
        }

        Line line = new Line(point, center);

        var intersection = Node.GetShape().GetIntersectionsWithLine(line);
        return Anchor.GetClosestPointTo(intersection, point);
    }

    public override BPoint? GetPlainPosition()
    {
        return Node.GetBounds()?.Center ?? null;
    }
}

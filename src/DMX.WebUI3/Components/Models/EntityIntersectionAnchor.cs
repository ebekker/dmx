// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Anchors;

namespace DMX.WebUI3.Components.Models;

public class EntityIntersectionAnchor : Anchor
{
    public EntityIntersectionAnchor(EntityNodeModel model) : base(model)
    {
        Node = model;
    }

    public EntityNodeModel Node { get; }

    public override BPoint? GetPosition(BaseLinkModel link, BPoint[] route)
    {
        if (Node.Size == null)
            return null;

        var isTarget = link.Target == this;
        var nodeBounds = Node.GetBounds()!;
        var nodeCenter = nodeBounds.Center;
        BPoint? pt;
        if (route.Length > 0)
        {
            pt = route[isTarget ? ^1 : 0];
        }
        else
        {
            pt = GetOtherPosition(link, isTarget);
        }

        if (pt is null) return null;

        var line = new BLine(pt, nodeCenter);
        var intersections = Node.GetShape().GetIntersectionsWithLine(line);
        var pos = GetClosestPointTo(intersections, pt);

        if (pos is not null && link is RelationshipLinkModel relLink)
        {
            var isChild = Node.Entity == relLink.Relationship.Child;
            var offset = isChild
                ? relLink.Relationship.ChildEdgeOffset
                : relLink.Relationship.ParentEdgeOffset;

            if (offset is not null)
            {
                if (pos.X == nodeBounds.Left || pos.X == nodeBounds.Right)
                {
                    pos = pos with { Y = pos.Y + offset.Value };
                }
                else if (pos.Y == nodeBounds.Top || pos.Y == nodeBounds.Bottom)
                {
                    pos = pos with { X = pos.X + offset.Value };
                }
            }
        }

        return pos;
    }

    public override BPoint? GetPlainPosition() => Node.GetBounds()?.Center ?? null;
}

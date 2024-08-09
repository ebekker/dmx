// DMX.
// Copyright (C) Eugene Bekker.

using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Radzen;

namespace DMX.WebUI3.Components.Models;

public class RelationshipLinkModel : LinkModel
{
    public RelationshipLinkModel(DmxRelationship rel, Anchor source, Anchor target)
        : base(source, target)
    {
        Relationship = rel;
    }

    public DmxRelationship Relationship { get; private set; }


    public void Update(DmxRelationship relationship)
    {
        Relationship = relationship;
        RefreshLinks();
        Refresh();
    }
}

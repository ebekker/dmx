// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMX.AppDB.Models;

public class DmxRelationship
{
    public virtual Guid Id { get; set; }

    public virtual string Name { get; set; } = default!;
    public virtual string? Description { get; set; }
    public virtual string? Memo { get; set; }

    public virtual DmxEntity Child { get; set; } = default!;
    public virtual DmxEntity Parent { get; set; } = default!;


    // Visual Stuff
    public virtual EntityEdge ChildEdge { get; set; }
    public virtual EntityEdge ParentEdge { get; set; }
    public virtual int? ChildEdgeOffset { get; set; }
    public virtual int? ParentEdgeOffset { get; set; }

    public virtual List<DmxRelationshipPair> Attributes { get; set; } = default!;

    public enum EntityEdge
    {
        Unspecified = 0,
        North = 1,
        South = 2,
        East = 3,
        West = 4,
    }

    internal class ModelConfig : IEntityTypeConfiguration<DmxRelationship>
    {
        public void Configure(EntityTypeBuilder<DmxRelationship> builder)
        {
            builder.ToTable("rel");
        }
    }
}

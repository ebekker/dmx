// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMX.AppDB.Models;

public class DmxEntity
{
    public virtual Guid Id { get; set; }

    public virtual string Name { get; set; } = default!;
    public virtual string? Description { get; set; }
    public virtual string? Memo { get; set; }

    // Visual Stuff
    public virtual int? PosX { get; set; }
    public virtual int? PoxY { get; set; }
    public virtual int? ZOrder { get; set; }

    public virtual List<DmxAttribute> Attributes { get; set; } = default!;

    public virtual List<DmxRelationship> RelationshipsAsChild { get; set; } = default!;
    public virtual List<DmxRelationship> RelationshipsAsParent { get; set; } = default!;

    internal class ModelConfig : IEntityTypeConfiguration<DmxEntity>
    {
        public void Configure(EntityTypeBuilder<DmxEntity> builder)
        {
            builder.ToTable("ent");
            builder.HasMany(x => x.RelationshipsAsChild)
                .WithOne(x => x.Child);
            builder.HasMany(x => x.RelationshipsAsParent)
                .WithOne(x => x.Parent);
        }
    }
}

// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMX.AppDB.Models;

public class DmxRelationshipPair
{
    public virtual Guid Id { get; set; }

    public virtual DmxRelationship Relationship { get; set; } = default!;

    public virtual DmxAttribute Child { get; set; } = default!;
    public virtual DmxAttribute Parent { get; set; } = default!;

    internal class ModelConfig : IEntityTypeConfiguration<DmxRelationshipPair>
    {
        public void Configure(EntityTypeBuilder<DmxRelationshipPair> builder)
        {
            builder.ToTable("rel_pair");
        }
    }
}

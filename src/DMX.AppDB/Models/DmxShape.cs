// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DMX.AppDB.Models;
internal class DmxShape
{
    public virtual Guid Id { get; set; }

    public virtual ShapeKind Kind { get; set; }

    public virtual string? Details { get; set; }

    // Visual Stuff
    public virtual int? PosX { get; set; }
    public virtual int? PoxY { get; set; }
    public virtual int? PosZ { get; set; } // Order within layer

    public virtual int? DimW { get; set; }
    public virtual int? DimH { get; set; }

    public enum ShapeKind
    {
        Note = 0,
    }

    internal class ModelConfig : IEntityTypeConfiguration<DmxShape>
    {
        public void Configure(EntityTypeBuilder<DmxShape> builder)
        {
            builder.ToTable("shape");
        }
    }
}

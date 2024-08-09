// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMX.AppDB.Models;

public class DmxAttribute
{
    public virtual Guid Id { get; set; }

    public virtual DmxEntity Entity { get; set; } = default!;

    public virtual string Name { get; set; } = default!;
    public virtual string? Description { get; set; }
    public virtual string? Memo { get; set; }

    public virtual DmxDomain? Domain { get; set; }

    public virtual bool IsPrimaryKey { get; set; }
    public virtual bool IsRequired { get; set; }

    public virtual int SortOrder { get; set; } = 0;

    internal class ModelConfig : IEntityTypeConfiguration<DmxAttribute>
    {
        public void Configure(EntityTypeBuilder<DmxAttribute> builder)
        {
            builder.ToTable("att");
        }
    }
}

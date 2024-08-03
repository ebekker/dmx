// DMX.
// Copyright (C) Eugene Bekker.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMX.AppDB.Models;

public class DmxDomain
{
    public virtual Guid Id { get; set; }

    public virtual string Name { get; set; } = default!;
    public virtual string? Description { get; set; }
    public virtual string? Memo { get; set; }

    internal class ModelConfig : IEntityTypeConfiguration<DmxDomain>
    {
        public void Configure(EntityTypeBuilder<DmxDomain> builder)
        {
            builder.ToTable("dom");
        }
    }
}

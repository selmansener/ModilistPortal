
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Data.EntityConfigurations.TenantDomain
{
    internal class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(Tenant)}_Seq");

            builder.Property(x => x.Type).HasDefaultValue(TenantType.None).IsRequired().HasConversion<string>();
        }
    }
}

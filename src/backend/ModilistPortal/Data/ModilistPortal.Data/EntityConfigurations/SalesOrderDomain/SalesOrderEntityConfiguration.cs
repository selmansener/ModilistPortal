
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.SalesOrderDomain;

namespace ModilistPortal.Data.EntityConfigurations.SalesOrderDomain
{
    internal class SalesOrderEntityConfiguration : IEntityTypeConfiguration<SalesOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrder> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(SalesOrder)}_Seq");

            builder.HasIndex(x => new { x.TenantId, x.MarketplaceSalesOrderId, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);
        }
    }
}

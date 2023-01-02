
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.SalesOrderDomain;

namespace ModilistPortal.Data.EntityConfigurations.SalesOrderDomain
{
    internal class DeliveryAddressEntityConfiguration : IEntityTypeConfiguration<DeliveryAddress>
    {
        public void Configure(EntityTypeBuilder<DeliveryAddress> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(DeliveryAddress)}_Seq");

            builder.HasOne(x => x.SalesOrder)
                .WithOne(x => x.DeliveryAddress)
                .HasForeignKey<DeliveryAddress>(x => x.SalesOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.ShipmentDomain;

namespace ModilistPortal.Data.EntityConfigurations.ShipmentDomain
{
    internal class ShipmentEntityConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(Shipment)}_Seq");

            builder.HasOne(x => x.SalesOrder)
                .WithOne(x => x.Shipment)
                .HasForeignKey<Shipment>(x => x.SalesOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

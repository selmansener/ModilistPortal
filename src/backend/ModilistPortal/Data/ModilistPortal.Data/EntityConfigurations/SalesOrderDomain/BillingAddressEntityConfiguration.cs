
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.SalesOrderDomain;

namespace ModilistPortal.Data.EntityConfigurations.SalesOrderDomain
{
    internal class BillingAddressEntityConfiguration : IEntityTypeConfiguration<BillingAddress>
    {
        public void Configure(EntityTypeBuilder<BillingAddress> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(BillingAddress)}_Seq");

            builder.HasOne(x => x.SalesOrder)
                .WithOne(x => x.BillingAddress)
                .HasForeignKey<BillingAddress>(x => x.SalesOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

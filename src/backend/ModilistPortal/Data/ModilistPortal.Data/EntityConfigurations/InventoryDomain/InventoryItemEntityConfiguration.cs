
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.InventoryDomain;

namespace ModilistPortal.Data.EntityConfigurations.InventoryDomain
{
    internal class InventoryItemEntityConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(InventoryItem)}_Seq");

            builder.HasOne(x => x.Product)
                .WithOne(x => x.Inventory)
                .HasForeignKey<InventoryItem>(x => x.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

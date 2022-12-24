
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Data.EntityConfigurations.ProductDomain
{
    internal class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(Product)}_Seq");

            builder.HasIndex(x => new { x.TenantId, x.SKU, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            builder.HasIndex(x => new { x.TenantId, x.Barcode, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            builder.Property(x => x.State).IsRequired().HasDefaultValue(ProductState.None).HasConversion<string>();

            builder.HasOne(x => x.Brand)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BrandId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

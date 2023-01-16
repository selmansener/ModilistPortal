
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.EntityConfigurations.ProductDomain
{
    internal class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(ProductImage)}_Seq");

            builder.Property(x => x.Url).HasMaxLength(4000);
        }
    }
}

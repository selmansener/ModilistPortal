
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.EntityConfigurations.ProductDomain
{
    internal class BrandEntityConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(Brand)}_Seq");

            builder.HasIndex(x => new { x.Name, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            builder.Property(x => x.LogoUrl).HasMaxLength(2500);
            builder.Property(x => x.DocumentUrl).HasMaxLength(2500);
        }
    }
}

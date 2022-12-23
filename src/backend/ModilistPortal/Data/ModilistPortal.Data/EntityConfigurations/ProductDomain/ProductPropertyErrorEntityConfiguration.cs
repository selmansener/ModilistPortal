
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.EntityConfigurations.ProductDomain
{
    internal class ProductPropertyErrorEntityConfiguration : IEntityTypeConfiguration<ProductPropertyError>
    {
        public void Configure(EntityTypeBuilder<ProductPropertyError> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(ProductPropertyError)}_Seq");

            builder.HasOne(x => x.ProductExcelRow)
                .WithMany(x => x.ErrorMappings)
                .HasForeignKey(x => x.ProductExcelRowId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Errors).IsRequired().HasConversion<string>(
                v => v.Count > 0 ? string.Join(",", v) : string.Empty,
                v => !string.IsNullOrEmpty(v) ? v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>()
            );
        }
    }
}

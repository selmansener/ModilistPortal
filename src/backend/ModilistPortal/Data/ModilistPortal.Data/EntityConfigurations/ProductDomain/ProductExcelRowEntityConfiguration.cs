using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Data.EntityConfigurations.ProductDomain
{
    public class ProductExcelRowEntityConfiguration : IEntityTypeConfiguration<ProductExcelRow>
    {
        public void Configure(EntityTypeBuilder<ProductExcelRow> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(ProductExcelRow)}_Seq");

            builder.HasOne(x => x.ProductExcelUpload)
                .WithMany(x => x.Rows)
                .HasForeignKey(x => x.ProductExcelUploadId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ProductExcelUploadId, x.RowId, x.DeletedAt })
                .IsUnique()
                .IsClustered(false);

            builder.Property(x => x.State).IsRequired().HasDefaultValue(ProductExcelRowState.None).HasConversion<string>();
        }
    }
}

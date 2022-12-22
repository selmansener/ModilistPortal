using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.EntityConfigurations.ProductDomain
{
    internal class ProductExcelUploadEntityConfiguration : IEntityTypeConfiguration<ProductExcelUpload>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProductExcelUpload> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo($"{nameof(ProductExcelUpload)}_Seq");

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(x => x.ContentType).HasMaxLength(100);
            builder.Property(x => x.Url).HasMaxLength(1000);
        }
    }
}

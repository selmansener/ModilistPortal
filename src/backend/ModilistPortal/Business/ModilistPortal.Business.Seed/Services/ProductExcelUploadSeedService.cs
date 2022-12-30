using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Business.Seed.Configuration;
using ModilistPortal.Business.Seed.Services.Base;
using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.Seed.Services
{
    internal class ProductExcelUploadSeedService : BaseSeedService
    {
        public ProductExcelUploadSeedService(ModilistPortalDbContext dbContext) 
            : base(dbContext)
        {
        }

        protected override ImmutableList<SeedServiceType> Dependencies => ImmutableList.Create(SeedServiceType.Accounts);

        public override async Task Execute(CancellationToken cancellationToken)
        {
            var tenants = await _dbContext.Tenants.ToListAsync();

            var faker = new Faker();

            var excels = new List<ProductExcelUpload>();
            foreach (var tenant in tenants)
            {
                for (int i = 0; i < 115; i++)
                {
                    var productExcelUpload = new ProductExcelUpload(tenant.Id, Guid.NewGuid(),  $"product-excel-{i}", "xls", "https://image.com", "application/octet-stream", faker.Random.Long(min: 100000, max:2000000));
                    excels.Add(productExcelUpload);
                }
            }

            await _dbContext.AddRangeAsync(excels);

            var categories = faker.Commerce.Categories(10);

            var productExcelRows = new List<ProductExcelRow>();
            foreach (var productExcelUpload in excels)
            {
                for (int i = 0; i < 25; i++)
                {
                    var productExcelRow = new ProductExcelRow(productExcelUpload.Id, i + 2, faker.Commerce.ProductName(), faker.Random.Replace("##-###-###"), faker.Random.Replace("##-#####"), "Mavi", faker.PickRandom(categories), faker.Commerce.Price(), faker.Commerce.Price(), faker.Random.Int(min: 10, max: 500).ToString());

                    productExcelRows.Add(productExcelRow);
                }
            }

            await _dbContext.AddRangeAsync(productExcelRows);
        }
    }
}

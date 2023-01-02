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
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.Seed.Services
{
    internal class ProductsSeedService : BaseSeedService
    {
        protected override ImmutableList<SeedServiceType> Dependencies => ImmutableList.Create(SeedServiceType.Accounts, SeedServiceType.Brands);

        public ProductsSeedService(ModilistPortalDbContext dbContext)
            : base(dbContext)
        {
        }

        public override async Task Execute(CancellationToken cancellationToken)
        {
            var faker = new Faker();
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(cancellationToken);

            var brands = await _dbContext.Brands.ToListAsync(cancellationToken);
            var brandIds = brands.Select(b => b.Id).ToList();
            var productCategories = faker.Commerce.Categories(10);

            var productStates = Enum.GetValues<ProductState>().Where(x => x != ProductState.None);

            var products = new List<Product>();
            foreach (var productState in productStates)
            {
                for (int i = 0; i < 50; i++)
                {
                    var product = new Product(faker.Commerce.ProductName(), faker.Random.Replace("##-###-###"), faker.Random.Replace("##-######"), faker.PickRandom(brandIds), faker.PickRandom(productCategories), decimal.Parse(faker.Commerce.Price()), decimal.Parse(faker.Commerce.Price()), 8, tenant.Id);

                    product.GetType().GetProperty(nameof(product.State)).SetValue(product, productState);

                    products.Add(product);
                }
            }

            await _dbContext.AddRangeAsync(products);
        }
    }
}

using System.Collections.Immutable;
using System.Globalization;

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

            Dictionary<int, Guid> productVariants = new Dictionary<int, Guid>();

            for(int i = 0; i < brandIds.Count; i++)
            {
                productVariants.Add(brandIds.ElementAt(i), Guid.NewGuid());
            }

            var colors = new List<string>
            {
                "Black",
                "Silver",
                "Gray",
                "White",
                "Maroon",
                "Red",
                "Purple",
                "Fuchsia",
                "Green",
                "Lime",
                "Olive",
                "Yellow",
                "Navy",
                "Blue",
                "Teal",
                "Aqua",
            };

            var sizes = new List<string>
            {
                "3XS",
                "XXS",
                "XS",
                "M",
                "L",
                "XXL",
                "3XL",
                "35",
                "35.5",
                "36",
                "37",
                "38",
                "39",
                "40",
                "41",
                "42",
                "43",
                "44",
                "45",
                "46",
            };

            var products = new List<Product>();
            foreach (var productState in productStates)
            {
                for (int i = 0; i < 50; i++)
                {

                    int randomIndex = faker.Random.Int(0, productVariants.Count-1);
                    int randomBrandId = productVariants.Keys.ElementAt(randomIndex);

                    var product = new Product(faker.Commerce.ProductName(), faker.Random.Replace("##-###-###"), faker.Random.Replace("##-######"), randomBrandId, faker.PickRandom(productCategories), decimal.Parse(faker.Commerce.Price()), decimal.Parse(faker.Commerce.Price()), 8, tenant.Id, faker.PickRandom<Gender>(), faker.PickRandom(sizes), faker.PickRandom(colors));

                    product.GetType().GetProperty(nameof(product.State)).SetValue(product, productState);

                    product.SetGroupId(productVariants[randomBrandId]);

                    products.Add(product);
                }
            }

            await _dbContext.AddRangeAsync(products);
        }
    }
}

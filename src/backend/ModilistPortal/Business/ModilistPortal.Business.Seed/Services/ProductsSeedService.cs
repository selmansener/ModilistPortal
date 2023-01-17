﻿using System.Collections.Immutable;

using Bogus;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Business.Seed.Configuration;
using ModilistPortal.Business.Seed.Services.Base;
using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.InventoryDomain;
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
                    var product = new Product(faker.Commerce.ProductName(), faker.Random.Replace("##-###-###"), faker.Random.Replace("##-######"), faker.PickRandom(brandIds), faker.PickRandom(productCategories), decimal.Parse(faker.Commerce.Price()), decimal.Parse(faker.Commerce.Price()), 8, tenant.Id, faker.PickRandom<Gender>(), faker.PickRandom(sizes));

                    product.GetType().GetProperty(nameof(product.State)).SetValue(product, productState);

                    var colorCount = faker.Random.Int(1, 4);
                    for (int j = 0; j < colorCount; j++)
                    {
                        product.AddColor(faker.PickRandom(colors));
                    }

                    products.Add(product);
                }
            }

            await _dbContext.AddRangeAsync(products);

            var availableProducts = products.Where(x => x.State == ProductState.Available).ToList();
            var inventoryItems = new List<InventoryItem>();
            foreach (var product in availableProducts)
            {
                var inventoryItem = new InventoryItem(product.Id, faker.Random.Int(min: 100, max: 500));
                inventoryItems.Add(inventoryItem);
            }

            await _dbContext.AddRangeAsync(inventoryItems);
        }
    }
}

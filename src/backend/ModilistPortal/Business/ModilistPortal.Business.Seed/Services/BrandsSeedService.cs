using ModilistPortal.Business.Seed.Services.Base;
using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.Seed.Services
{
    internal class BrandsSeedService : BaseSeedService
    {
        public BrandsSeedService(ModilistPortalDbContext dbContext)
            : base(dbContext)
        {
        }

        public override async Task Execute(CancellationToken cancellationToken)
        {
            var brands = new List<string>
            {
                "Mavi",
                "Zara",
                "LCW",
                "Boyner",
                "Lacoste",
                "CHANEL",
                "Prada",
                "Hermes"
            };

            foreach (var brandName in brands)
            {
                var brand = new Brand(brandName);

                await _dbContext.AddAsync(brand, cancellationToken);
            }
        }
    }
}

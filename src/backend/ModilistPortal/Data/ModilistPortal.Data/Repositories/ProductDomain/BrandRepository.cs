
using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.Repositories.ProductDomain
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken);
    }

    internal class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ModilistPortalDbContext baseDb)
            : base(baseDb)
        {
        }

        public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _baseDb.Brands.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }
    }
}

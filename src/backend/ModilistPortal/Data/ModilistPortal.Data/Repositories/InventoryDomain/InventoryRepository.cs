
using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.InventoryDomain;

namespace ModilistPortal.Data.Repositories.InventoryDomain
{
    public interface IInventoryRepository : IBaseRepository<InventoryItem>
    {
        Task<InventoryItem?> GetByProductIdAsync(int productId, int tenantId, CancellationToken cancellationToken);

        Task<IEnumerable<InventoryItem>> GetAllByProductIdsAsync(int tenantId, IEnumerable<int> productIds, CancellationToken cancellationToken);
    }

    internal class InventoryRepository : BaseRepository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(ModilistPortalDbContext baseDb)
            : base(baseDb)
        {
        }

        public async Task<IEnumerable<InventoryItem>> GetAllByProductIdsAsync(int tenantId, IEnumerable<int> productIds, CancellationToken cancellationToken)
        {
            return await _baseDb.InventoryItems.Where(x => productIds.Any(productId => x.ProductId == productId && x.Product.TenantId == tenantId)).ToListAsync(cancellationToken);
        }

        public async Task<InventoryItem?> GetByProductIdAsync(int productId, int tenantId, CancellationToken cancellationToken)
        {
            return await _baseDb.InventoryItems.FirstOrDefaultAsync(x => x.ProductId == productId && x.Product.TenantId == tenantId, cancellationToken);
        }
    }
}

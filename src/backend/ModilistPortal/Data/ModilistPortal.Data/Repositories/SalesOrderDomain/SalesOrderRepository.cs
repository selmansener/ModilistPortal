
using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.SalesOrderDomain;

namespace ModilistPortal.Data.Repositories.SalesOrderDomain
{
    public interface ISalesOrderRepository : IBaseRepository<SalesOrder> 
    {
        Task<bool> DoesExistsByMarketplaceIdAsync(int marketplaceSalesOrderId, int tenantId, CancellationToken cancellationToken);
    }

    internal class SalesOrderRepository : BaseRepository<SalesOrder>, ISalesOrderRepository
    {
        public SalesOrderRepository(ModilistPortalDbContext baseDb)
            : base(baseDb)
        {
        }

        public async Task<bool> DoesExistsByMarketplaceIdAsync(int marketplaceSalesOrderId, int tenantId, CancellationToken cancellationToken)
        {
            return await _baseDb.SalesOrders.AnyAsync(x => x.TenantId == tenantId && x.MarketplaceSalesOrderId == marketplaceSalesOrderId, cancellationToken);
        }
    }
}

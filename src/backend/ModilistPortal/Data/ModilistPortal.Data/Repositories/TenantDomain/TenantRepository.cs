
using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.TenantDomain;

namespace ModilistPortal.Data.Repositories.TenantDomain
{
    public interface ITenantRepository : IBaseRepository<Tenant>
    {
        Task<Tenant?> GetByAccountId(Guid accountId, CancellationToken cancellationToken);
    }

    internal class TenantRepository : BaseRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(ModilistPortalDbContext baseDb) : base(baseDb)
        {
        }

        public async Task<Tenant?> GetByAccountId(Guid accountId, CancellationToken cancellationToken)
        {
            return await _baseDb.Tenants.FirstOrDefaultAsync(x => x.Accounts.Any(x => x.Id == accountId), cancellationToken);
        }
    }
}

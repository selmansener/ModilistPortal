
using ModilistPortal.Business.Seed.Services.Base;
using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.TenantDomain;

namespace ModilistPortal.Business.Seed.Services
{
    internal class TenantsSeedService : BaseSeedService
    {
        public TenantsSeedService(ModilistPortalDbContext dbContext)
            : base(dbContext)
        {
        }

        public override async Task Execute(CancellationToken cancellationToken)
        {
            var tenant = new Tenant("Modilist", null, "6222283819", "Başkent Vergi Dairesi", "05316573356", "info@modilist.com", "Ankara", "Çankaya", Infrastructure.Shared.Enums.TenantType.IncorporatedCompany);

            await _dbContext.AddAsync(tenant, cancellationToken);
        }
    }
}

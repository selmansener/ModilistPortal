using ModilistPortal.Business.Seed.Data;
using ModilistPortal.Business.Seed.Services.Base;
using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Business.Seed.Services
{
    internal class AccountsSeedService : BaseSeedService
    {
        private readonly SeedData _seedData;

        public AccountsSeedService(ModilistPortalDbContext dbContext, SeedData seedData)
            : base(dbContext)
        {
            _seedData = seedData;
        }

        public override async Task Execute(CancellationToken cancellationToken)
        {
            foreach (var user in _seedData.Accounts)
            {
                var account = new Account(user.Id);

                await _dbContext.AddAsync(account, cancellationToken);
            }
        }
    }
}

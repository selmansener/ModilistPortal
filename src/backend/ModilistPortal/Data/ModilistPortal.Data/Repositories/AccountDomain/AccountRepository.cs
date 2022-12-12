using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Data.Repositories.AccountDomain
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<Account?> GetByMail(string mail, CancellationToken cancellationToken);
    }

    internal class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(ModilistPortalDbContext baseDb)
            : base(baseDb)
        {
        }

        public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _baseDb.Accounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Account?> GetByMail(string mail, CancellationToken cancellationToken)
        {
            return await GetAll().FirstOrDefaultAsync(account => account.Email == mail, cancellationToken);
        }
    }
}

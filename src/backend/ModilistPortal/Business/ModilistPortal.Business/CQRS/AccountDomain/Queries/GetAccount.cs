using Mapster;

using MediatR;

using ModilistPortal.Business.CQRS.AccountDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Business.CQRS.AccountDomain.Queries
{
    public class GetAccount : IRequest<AccountDTO>
    {
        public Guid Id { get; set; }
    }

    internal class GetAccountHandler : IRequestHandler<GetAccount, AccountDTO>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDTO> Handle(GetAccount request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByIdAsync(request.Id, cancellationToken);

            if (account == null)
            {
                throw new AccountNotFoundException(request.Id);
            }

            return account.Adapt<AccountDTO>();
        }
    }
}

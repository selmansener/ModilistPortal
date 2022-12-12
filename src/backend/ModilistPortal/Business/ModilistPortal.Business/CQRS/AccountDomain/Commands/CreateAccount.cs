using FluentValidation;

using Mapster;

using MediatR;

using ModilistPortal.Business.CQRS.AccountDomain.DTOs;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Business.CQRS.AccountDomain.Commands
{
    public class CreateAccount : IRequest<AccountDTO>
    {
        public CreateAccount(Guid id, string email)
        {
            Id = id;
            Email = email;
        }

        public Guid Id { get; set; }

        public string Email { get; set; }
    }

    internal class CreateAccountValidator : AbstractValidator<CreateAccount>
    {
        public CreateAccountValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
        }
    }

    internal class CreateAccountHandler : IRequestHandler<CreateAccount, AccountDTO>
    {
        private readonly IAccountRepository _accountWriteRepository;

        public CreateAccountHandler(IAccountRepository accountWriteRepository)
        {
            _accountWriteRepository = accountWriteRepository;
        }

        public async Task<AccountDTO> Handle(CreateAccount request, CancellationToken cancellationToken)
        {
            Account? account = await _accountWriteRepository.GetByIdAsync(request.Id, cancellationToken);

            if (account != null)
            {
                // TODO: change to client exception
                throw new Exception("account already exists");
            }

            account = new Account(request.Id);

            await _accountWriteRepository.AddAsync(account, cancellationToken, true);

            return account.Adapt<AccountDTO>();
        }
    }
}

using Azure.Messaging.EventGrid;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.Extensions.Options;

using ModilistPortal.Business.CQRS.AccountDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Domains.Models.AccountDomain;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Events;

namespace ModilistPortal.Business.CQRS.AccountDomain.Commands
{
    public class ActivateAccount : IRequest<AccountDTO>
    {
        public Guid AccountId { get; set; }
    }

    internal class ActivateAccountValidator : AbstractValidator<ActivateAccount>
    {
        public ActivateAccountValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }

    internal class ActivateAccountHandler : IRequestHandler<ActivateAccount, AccountDTO>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly EventGridPublisherClient _eventGridPublisherClient;

        public ActivateAccountHandler(IAccountRepository accountRepository,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> options)
        {
            _accountRepository = accountRepository;
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(options.Value);
        }

        public async Task<AccountDTO> Handle(ActivateAccount request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            account.Activate();

            await _accountRepository.UpdateAsync(account, cancellationToken);

            var accountActivatedEvent = new AccountActivated(EventPublishers.WebAPI, PublisherType.System, account.Id);

            await _eventGridPublisherClient.SendEventAsync(new EventGridEvent("AccountActivated", typeof(AccountActivated).Name, accountActivatedEvent.Version, accountActivatedEvent), cancellationToken);

            return account.Adapt<AccountDTO>();
        }
    }
}

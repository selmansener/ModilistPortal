
using FluentValidation;

using Mapster;

using MediatR;

using ModilistPortal.Business.CQRS.TenantDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.TenantDomain;
using ModilistPortal.Domains.Models.AccountDomain;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

using Newtonsoft.Json;

namespace ModilistPortal.Business.CQRS.TenantDomain.Commands
{
    public class UpsertTenant : IRequest<TenantDTO>
    {
        [JsonIgnore]
        public Guid AccountId { get; set; }

        public string Name { get; set; }

        public string TCKN { get; set; }

        public string TaxNumber { get; set; }

        public string TaxOffice { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public TenantType Type { get; set; }
    }

    internal class UpsertTenantValidator : AbstractValidator<UpsertTenant>
    {
        public UpsertTenantValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.TaxOffice).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.District).NotEmpty();
            RuleFor(x => x.Type).IsInEnum().NotEqual(TenantType.None);
            RuleFor(x => x.TCKN).NotNull().When(x => x.Type == TenantType.Individual);
            RuleFor(x => x.TaxNumber).NotNull().When(x => x.Type != TenantType.Individual && x.Type != TenantType.None);
        }
    }

    internal class UpsertTenantHandler : IRequestHandler<UpsertTenant, TenantDTO>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITenantRepository _tenantRepository;

        public UpsertTenantHandler(IAccountRepository accountRepository, ITenantRepository tenantRepository)
        {
            _accountRepository = accountRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDTO> Handle(UpsertTenant request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            Tenant? tenant = await _tenantRepository.GetByAccountId(request.AccountId, cancellationToken);

            if (tenant == null)
            {
                tenant = new Tenant(request.Name, request.TCKN, request.TaxNumber, request.TaxOffice, request.Phone, request.Email, request.City, request.District, request.Type);

                await _tenantRepository.AddAsync(tenant, cancellationToken);

                account.AssignTenant(tenant.Id);

                await _accountRepository.UpdateAsync(account, cancellationToken);
            }
            else
            {
                tenant.Update(request.Name, request.TCKN, request.TaxNumber, request.TaxOffice, request.Phone, request.Email, request.City, request.District, request.Type);

                await _tenantRepository.UpdateAsync(tenant, cancellationToken);
            }

            return tenant.Adapt<TenantDTO>();
        }
    }
}

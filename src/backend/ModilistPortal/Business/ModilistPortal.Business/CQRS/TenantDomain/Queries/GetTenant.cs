
using FluentValidation;

using Mapster;

using MediatR;

using ModilistPortal.Business.CQRS.TenantDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.TenantDomain;
using ModilistPortal.Domains.Models.TenantDomain;

namespace ModilistPortal.Business.CQRS.TenantDomain.Queries
{
    public class GetTenant : IRequest<TenantDTO>
    {
        public Guid AccountId { get; set; }
    }

    internal class GetTenantValidator : AbstractValidator<GetTenant>
    {
        public GetTenantValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }

    internal class GetTenantHandler : IRequestHandler<GetTenant, TenantDTO>
    {
        private readonly ITenantRepository _tenantRepository;

        public GetTenantHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDTO> Handle(GetTenant request, CancellationToken cancellationToken)
        {
            Tenant? tenant = await _tenantRepository.GetByAccountId(request.AccountId, cancellationToken);

            if (tenant == null)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            return tenant.Adapt<TenantDTO>();
        }
    }
}

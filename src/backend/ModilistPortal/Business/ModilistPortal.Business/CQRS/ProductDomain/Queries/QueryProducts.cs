
using DynamicQueryBuilder;
using DynamicQueryBuilder.Models;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Queries
{
    public class QueryProducts : IRequest<DQBResultDTO<QueryProductDTO>>
    {
        public QueryProducts(Guid accountId, DynamicQueryOptions dQBOptions)
        {
            AccountId = accountId;
            DQBOptions = dQBOptions;
        }

        public Guid AccountId { get; private set; }

        public DynamicQueryOptions DQBOptions { get; private set; }
    }

    internal class QueryProductsValidator : AbstractValidator<QueryProducts>
    {
        public QueryProductsValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.DQBOptions).NotEmpty();
        }
    }

    internal class QueryProductsHandler : IRequestHandler<QueryProducts, DQBResultDTO<QueryProductDTO>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductRepository _productRepository;

        public QueryProductsHandler(IAccountRepository accountRepository, IProductRepository productRepository)
        {
            _accountRepository = accountRepository;
            _productRepository = productRepository;
        }

        public async Task<DQBResultDTO<QueryProductDTO>> Handle(QueryProducts request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            if (!account.TenantId.HasValue)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            var result = await _productRepository
                .GetAllAsNoTracking()
                .Where(x => x.TenantId == account.TenantId)
                .ProjectToType<QueryProductDTO>()
                .ApplyFilters(request.DQBOptions)
                .ToListAsync(cancellationToken);

            return new DQBResultDTO<QueryProductDTO>
            {
                Count = request.DQBOptions.PaginationOption.DataSetCount,
                Data = result
            };
        }
    }
}

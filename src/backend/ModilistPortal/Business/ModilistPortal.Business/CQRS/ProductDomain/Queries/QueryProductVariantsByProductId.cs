using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class QueryProductVariantsByProductId : IRequest<DQBResultDTO<QueryProductDTO>>
    {
        public QueryProductVariantsByProductId(int productId, Guid accountId, DynamicQueryOptions dQBOptions)
        {
            ProductId = productId;
            AccountId = accountId;
            DQBOptions = dQBOptions;
        }

        public int ProductId { get; set; }
       
        public Guid AccountId { get; private set; }

        public DynamicQueryOptions DQBOptions { get; private set; }

    }

    internal class QueryProductVariantsValidator : AbstractValidator<QueryProductVariantsByProductId>
    {
        public QueryProductVariantsValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.DQBOptions).NotEmpty();
        }
    }

    internal class QueryProductVariantsHandler : IRequestHandler<QueryProductVariantsByProductId, DQBResultDTO<QueryProductDTO>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductRepository _productRepository;

        public QueryProductVariantsHandler(IAccountRepository accountRepository, IProductRepository productRepository)
        {
            _accountRepository = accountRepository;
            _productRepository = productRepository;
        }

        async Task<DQBResultDTO<QueryProductDTO>> IRequestHandler<QueryProductVariantsByProductId, DQBResultDTO<QueryProductDTO>>.Handle(QueryProductVariantsByProductId request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if(account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            if (!account.TenantId.HasValue)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if(product == null)
            {
                throw new ProductNotFoundException(request.ProductId, account.TenantId.Value);
            }

            var result = await _productRepository
                .GetAllAsNoTracking()
                .Where(x => x.TenantId == account.TenantId && x.GroupId == product.GroupId)
                .ProjectToType<QueryProductDTO>()
                .ApplyFilters(request.DQBOptions)
                .ToListAsync();

            return new DQBResultDTO<QueryProductDTO>()
            {
                Count= request.DQBOptions.PaginationOption.DataSetCount,
                Data= result
            };
        }
    }
}

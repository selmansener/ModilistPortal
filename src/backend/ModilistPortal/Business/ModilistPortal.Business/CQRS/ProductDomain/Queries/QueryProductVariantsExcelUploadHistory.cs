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
using Microsoft.Identity.Client;

using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Queries
{
    public class QueryProductVariantsExcelUploadHistory : IRequest<DQBResultDTO<ProductExcelUploadDTO>>
    {
        public QueryProductVariantsExcelUploadHistory(Guid accountId, DynamicQueryOptions dQBOptions)
        {
            AccountId = accountId;
            DQBOptions = dQBOptions;
        }

        public Guid AccountId { get; private set; }

        public DynamicQueryOptions DQBOptions { get; private set; }
    }

    internal class QueryProductVariantsExcelUploadHistoryValidator : AbstractValidator<QueryProductVariantsExcelUploadHistory>
    {
        public QueryProductVariantsExcelUploadHistoryValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.DQBOptions).NotEmpty();
        }
    }

    internal class QueryProductVariantsExcelUploadHistoryHandler : IRequestHandler<QueryProductVariantsExcelUploadHistory, DQBResultDTO<ProductExcelUploadDTO>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;

        public QueryProductVariantsExcelUploadHistoryHandler(IAccountRepository accountRepository, IProductExcelUploadRepository productExcelUploadRepository)
        {
            _accountRepository = accountRepository;
            _productExcelUploadRepository = productExcelUploadRepository;
        }

        public async Task<DQBResultDTO<ProductExcelUploadDTO>> Handle(QueryProductVariantsExcelUploadHistory request, CancellationToken cancellationToken)
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

            var data = await _productExcelUploadRepository
               .GetAllAsNoTracking()
               .Where(x => x.TenantId == account.TenantId && x.IsVariantExcel == true)
               .ProjectToType<ProductExcelUploadDTO>()
               .ApplyFilters(request.DQBOptions)
               .ToListAsync(cancellationToken);

            return new DQBResultDTO<ProductExcelUploadDTO>
            {
                Data = data,
                Count = request.DQBOptions.PaginationOption.DataSetCount
            };
        }
    }
}

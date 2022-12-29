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
using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Queries
{
    public class QueryProductExcelUploadHistory : IRequest<DQBResultDTO<ProductExcelUploadDTO>>
    {
        public QueryProductExcelUploadHistory(Guid accountId, DynamicQueryOptions dQBOptions)
        {
            AccountId = accountId;
            DQBOptions = dQBOptions;
        }

        public Guid AccountId { get; private set; }

        public DynamicQueryOptions DQBOptions { get; private set; }
    }

    internal class QueryProductExcelUploadHistoryValidator : AbstractValidator<QueryProductExcelUploadHistory>
    {

    }

    internal class QueryProductExcelUploadHistoryHandler : IRequestHandler<QueryProductExcelUploadHistory, DQBResultDTO<ProductExcelUploadDTO>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;

        public QueryProductExcelUploadHistoryHandler(IProductExcelUploadRepository productExcelUploadRepository, IAccountRepository accountRepository)
        {
            _productExcelUploadRepository = productExcelUploadRepository;
            _accountRepository = accountRepository;
        }

        public async Task<DQBResultDTO<ProductExcelUploadDTO>> Handle(QueryProductExcelUploadHistory request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            if (!account.TenantId.HasValue)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            var data = await _productExcelUploadRepository
                .GetAllAsNoTracking()
                .Where(x => x.TenantId == account.TenantId)
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

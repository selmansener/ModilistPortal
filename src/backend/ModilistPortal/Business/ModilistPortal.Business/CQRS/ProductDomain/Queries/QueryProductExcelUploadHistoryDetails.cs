
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
    public class QueryProductExcelUploadHistoryDetails : IRequest<DQBResultDTO<QueryProductExcelRowDTO>>
    {
        public QueryProductExcelUploadHistoryDetails(int productExcelUploadId, Guid accountId, DynamicQueryOptions dQBOptions)
        {
            ProductExcelUploadId = productExcelUploadId;
            AccountId = accountId;
            DQBOptions = dQBOptions;
        }

        public Guid AccountId { get; private set; }

        public int ProductExcelUploadId { get; private set; }

        public DynamicQueryOptions DQBOptions { get; private set; }
    }

    internal class QueryProductExcelUploadHistoryDetailsValidator : AbstractValidator<QueryProductExcelUploadHistoryDetails>
    {
        public QueryProductExcelUploadHistoryDetailsValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.ProductExcelUploadId).NotEmpty();
        }
    }

    internal class QueryProductExcelUploadHistoryDetailsHandler : IRequestHandler<QueryProductExcelUploadHistoryDetails, DQBResultDTO<QueryProductExcelRowDTO>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductExcelRowRepository _productExcelRowRepository;

        public QueryProductExcelUploadHistoryDetailsHandler(IAccountRepository accountRepository, IProductExcelRowRepository productExcelRowRepository)
        {
            _accountRepository = accountRepository;
            _productExcelRowRepository = productExcelRowRepository;
        }

        public async Task<DQBResultDTO<QueryProductExcelRowDTO>> Handle(QueryProductExcelUploadHistoryDetails request, CancellationToken cancellationToken)
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

            var result = await _productExcelRowRepository
                .GetAllAsNoTracking()
                .Where(x => x.ProductExcelUpload.TenantId == account.TenantId.Value && x.ProductExcelUploadId == request.ProductExcelUploadId)
                .ProjectToType<QueryProductExcelRowDTO>()
                .ApplyFilters(request.DQBOptions)
                .ToListAsync(cancellationToken);

            return new DQBResultDTO<QueryProductExcelRowDTO>
            {
                Count = request.DQBOptions.PaginationOption.DataSetCount,
                Data = result
            };
        }
    }
}

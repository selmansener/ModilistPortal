using FluentValidation;

using Mapster;

using MediatR;

using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Queries
{
    public class GetProductExcelRow : IRequest<ProductExcelRowDTO>
    {
        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }
    }

    internal class GetProductExcelRowValidator : AbstractValidator<GetProductExcelRow>
    {
        public GetProductExcelRowValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.BlobId).NotEmpty();
            RuleFor(x => x.RowId).NotEmpty();
        }
    }

    internal class GetProductExcelRowHandler : IRequestHandler<GetProductExcelRow, ProductExcelRowDTO>
    {
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;

        public GetProductExcelRowHandler(IProductExcelUploadRepository productExcelUploadRepository)
        {
            _productExcelUploadRepository = productExcelUploadRepository;
        }

        public async Task<ProductExcelRowDTO> Handle(GetProductExcelRow request, CancellationToken cancellationToken)
        {
            var productExcelRow = await _productExcelUploadRepository.GetProductExcelRow(request.TenantId, request.BlobId, request.RowId, cancellationToken);

            if (productExcelRow == null)
            {
                throw new ProductExcelRowNotFoundException(request.TenantId, request.BlobId, request.RowId);
            }

            return productExcelRow.Adapt<ProductExcelRowDTO>();
        }
    }
}

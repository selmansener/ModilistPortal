
using FluentValidation;

using MediatR;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class UpsertProductExcelRow : IRequest
    {
        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string Price { get; set; }

        public string SalesPrice { get; set; }

        public string StockAmount { get; set; }
    }

    internal class UpsertProductExcelRowValidator : AbstractValidator<UpsertProductExcelRow>
    {
        public UpsertProductExcelRowValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.BlobId).NotEmpty();
            RuleFor(x => x.RowId).NotEmpty();
        }
    }

    internal class UpsertProductExcelRowHandler : IRequestHandler<UpsertProductExcelRow, Unit>
    {
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;

        public UpsertProductExcelRowHandler(IProductExcelUploadRepository productExcelUploadRepository)
        {
            _productExcelUploadRepository = productExcelUploadRepository;
        }

        public async Task<Unit> Handle(UpsertProductExcelRow request, CancellationToken cancellationToken)
        {
            var productExcelUpload = await _productExcelUploadRepository.GetByBlobId(request.TenantId, request.BlobId, cancellationToken);

            if (productExcelUpload == null)
            {
                throw new ProductExcelUploadNotFoundException(request.TenantId, request.BlobId);
            }

            productExcelUpload.UpsertRow(
                request.RowId,
                request.Name,
                request.SKU,
                request.Barcode,
                request.Brand,
                request.Category,
                request.Price,
                request.SalesPrice,
                request.StockAmount);

            await _productExcelUploadRepository.UpdateAsync(productExcelUpload, cancellationToken);

            return Unit.Value;
        }
    }
}

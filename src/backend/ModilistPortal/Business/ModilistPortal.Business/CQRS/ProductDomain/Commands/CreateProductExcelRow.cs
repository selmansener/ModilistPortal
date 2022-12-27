
using FluentValidation;

using MediatR;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class CreateProductExcelRow : IRequest<int>
    {
        public int ProductExcelUploadId { get; set; }

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

    internal class CreateProductExcelRowValidator : AbstractValidator<CreateProductExcelRow>
    {
        public CreateProductExcelRowValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.BlobId).NotEmpty();
            RuleFor(x => x.RowId).NotEmpty();
        }
    }

    internal class CreateProductExcelRowHandler : IRequestHandler<CreateProductExcelRow, int>
    {
        private readonly IProductExcelRowRepository _productExcelRowRepository;

        public CreateProductExcelRowHandler(IProductExcelRowRepository productExcelRowRepository)
        {
            _productExcelRowRepository = productExcelRowRepository;
        }

        public async Task<int> Handle(CreateProductExcelRow request, CancellationToken cancellationToken)
        {
            // We assume there is already a ProductExcelUpload and directly insert row.
            var productExcelRow = new ProductExcelRow(request.ProductExcelUploadId, request.RowId, request.Name, request.SKU, request.Barcode, request.Brand, request.Category, request.Price, request.SalesPrice, request.StockAmount); 

            await _productExcelRowRepository.AddAsync(productExcelRow, cancellationToken);

            return productExcelRow.Id;
        }
    }
}

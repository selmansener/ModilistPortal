
using FluentValidation;

using MediatR;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class CreateProduct : IRequest<int>
    {
        public int TenantId { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public decimal SalesPrice { get; set; }

        public int TaxRatio { get; set; }
    }

    internal class CreateProductValidator : AbstractValidator<CreateProduct>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SKU).NotEmpty();
            RuleFor(x => x.Barcode).NotEmpty();
            RuleFor(x => x.Brand).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.SalesPrice).NotEmpty();
            RuleFor(x => x.TaxRatio).NotEmpty();
        }
    }

    internal class CreateProductHandler : IRequestHandler<CreateProduct, int>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IProductRepository _productRepository;

        public CreateProductHandler(IBrandRepository brandRepository, IProductRepository productRepository)
        {
            _brandRepository = brandRepository;
            _productRepository = productRepository;
        }

        public async Task<int> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.GetByName(request.Brand, cancellationToken);

            if (brand == null)
            {
                brand = new Brand(request.Name);
                await _brandRepository.AddAsync(brand, cancellationToken);
            }

            var doesSKUExists = await _productRepository.DoesExistsWithSKU(request.TenantId, request.SKU, cancellationToken);

            if (doesSKUExists)
            {
                throw new ProductAlreadyExistsException(request.TenantId, sku: request.SKU);
            }

            var doesBarcodeExists = await _productRepository.DoesExistsWithBarcode(request.TenantId, request.Barcode, cancellationToken);

            if (doesBarcodeExists)
            {
                throw new ProductAlreadyExistsException(request.TenantId, barcode: request.Barcode);
            }

            var product = new Product(request.Name, request.SKU, request.Barcode, brand.Id, request.Category, request.Price, request.SalesPrice, 8, request.TenantId);

            await _productRepository.AddAsync(product, cancellationToken);

            return product.Id;
        }
    }
}

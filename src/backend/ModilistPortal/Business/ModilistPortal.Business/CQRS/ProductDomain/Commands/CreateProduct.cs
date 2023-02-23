using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

using MediatR;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class CreateProduct : IRequest
    {
        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public int BrandId { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public decimal SalesPrice { get; set; }
        
        public int TaxRatio { get; set;}

        public ProductState State { get; set; }//doğru mu? 

        public int TenantId { get; set; }

        public Gender Gender { get; set;}

        public string Size { get; set; }

        public string Color { get; set; }


    }

    internal class CreateProductValidator : AbstractValidator<CreateProduct>
    {
        public CreateProductValidator() 
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SKU).NotEmpty();
            RuleFor(x => x.Barcode).NotEmpty();
            RuleFor(x => x.BrandId).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.SalesPrice).NotEmpty();
            RuleFor(x => x.TaxRatio).NotEmpty();
            RuleFor(x => x.State).NotEmpty();
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
            RuleFor(x => x.Size).NotEmpty();
            RuleFor(x => x.Color).NotEmpty();

        }
    }

    internal class CreateProductHandler : IRequestHandler<CreateProduct, Unit>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.DoesExistsWithSKU(request.TenantId, request.SKU, cancellationToken);

            if (existingProduct != null)
            {
                throw new ProductAlreadyExistsException(request.TenantId, request.SKU);
            }

            var product = new Product(request.Name, request.SKU, request.Barcode, request.BrandId, request.Category, request.Price, request.SalesPrice, request.TaxRatio, request.TenantId, request.Gender, request.Size,  request.Color);

            await _productRepository.AddAsync(product, cancellationToken);

            return Unit.Value;
        }
    }  

}

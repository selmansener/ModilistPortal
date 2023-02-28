using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
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

        public string BrandName { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public decimal SalesPrice { get; set; }
        
        public Guid AccountId { get; set; }

        public Gender? Gender { get; set;}

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
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.BrandName).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.SalesPrice).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty();
            RuleFor(x => x.Size).NotEmpty();
            RuleFor(x => x.Color).NotEmpty();
        }
    }

    internal class CreateProductHandler : IRequestHandler<CreateProduct, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMediator _mediator;

        public CreateProductHandler(IProductRepository productRepository, IBrandRepository brandRepository, IAccountRepository accountRepository, IMediator mediator)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _accountRepository = accountRepository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateProduct request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
            
            if(account == null) 
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            var existingProduct = await _productRepository.DoesExistsWithSKU(account.TenantId.Value, request.SKU, cancellationToken);

            if (existingProduct)
            {
                throw new ProductAlreadyExistsException(account.TenantId.Value, request.SKU);
            }

            var existingBrand = await _brandRepository.GetByNameAsync(request.BrandName, cancellationToken);

            if (existingBrand == null)
            {
                var newBrand = new Brand(request.BrandName);

                await _brandRepository.AddAsync(newBrand, cancellationToken, saveChanges: true);
            }
                 
            var brand = await _brandRepository.GetByNameAsync(request.BrandName, cancellationToken);
                
            var product = new Product(request.Name, request.SKU, request.Barcode, brand.Id, request.Category, request.Price, request.SalesPrice, 23, account.TenantId.Value, request.Gender ?? default(Gender), request.Size, request.Color);

            product.SetGroupId(Guid.NewGuid());

            await _productRepository.AddAsync(product, cancellationToken);

            return Unit.Value;
        }
    }  

}

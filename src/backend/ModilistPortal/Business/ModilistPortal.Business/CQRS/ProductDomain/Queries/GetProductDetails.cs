
using FluentValidation;

using Mapster;

using MediatR;

using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Queries
{
    public class GetProductDetails : IRequest<ProductDetailsDTO>
    {
        public GetProductDetails(Guid accountId, int productId)
        {
            AccountId = accountId;
            ProductId = productId;
        }

        public Guid AccountId { get; private set; }

        public int ProductId { get; private set; }
    }

    internal class GetProductDetailsValidator : AbstractValidator<GetProductDetails>
    {
        public GetProductDetailsValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }

    internal class GetProductDetailsHandler : IRequestHandler<GetProductDetails, ProductDetailsDTO>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductRepository _productRepository;

        public GetProductDetailsHandler(IAccountRepository accountRepository, IProductRepository productRepository)
        {
            _accountRepository = accountRepository;
            _productRepository = productRepository;
        }

        public async Task<ProductDetailsDTO> Handle(GetProductDetails request, CancellationToken cancellationToken)
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

            var product = await _productRepository.GetByIdAsync(request.ProductId, account.TenantId.Value, cancellationToken);

            if (product == null)
            {
                throw new ProductNotFoundException(request.ProductId, account.TenantId.Value);
            }

            return product.Adapt<ProductDetailsDTO>();
        }
    }
}

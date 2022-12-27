
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class CreateBrandsIfNotExists : IRequest
    {
        public CreateBrandsIfNotExists(IReadOnlyList<string> brands)
        {
            Brands = brands;
        }

        public IReadOnlyList<string> Brands { get; set; }
    }

    internal class CreateBrandsIfNotExistsValidator : AbstractValidator<CreateBrandsIfNotExists>
    {
        public CreateBrandsIfNotExistsValidator()
        {
            RuleFor(x => x.Brands).NotEmpty();
        }
    }

    internal class CreateBrandsIfNotExistsHandler : IRequestHandler<CreateBrandsIfNotExists, Unit>
    {
        private readonly IBrandRepository _brandRepository;

        public CreateBrandsIfNotExistsHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<Unit> Handle(CreateBrandsIfNotExists request, CancellationToken cancellationToken)
        {
            var existingBrands = await _brandRepository.GetAll().Where(x => request.Brands.Contains(x.Name)).ToListAsync();

            var newBrands = new List<Brand>();
            foreach (var brand in request.Brands)
            {
                if (!existingBrands.Any(x => x.Name == brand))
                {
                    newBrands.Add(new Brand(brand));
                }
            }

            if (newBrands.Any())
            {
                await _brandRepository.AddRangeAsync(newBrands, cancellationToken);
            }

            return Unit.Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class SetRowErrors : IRequest
    {
        public int ProductExcelRowId { get; set; }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }

        public IDictionary<string, IReadOnlyList<string>> Errors { get; set; }
    }

    internal class SetRowErrorsValidator : AbstractValidator<SetRowErrors>
    {
        public SetRowErrorsValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.BlobId).NotEmpty();
            RuleFor(x => x.RowId).NotEmpty();
            RuleFor(x => x.Errors).NotEmpty();
        }
    }

    internal class SetRowErrorsHandler : IRequestHandler<SetRowErrors, Unit>
    {
        private readonly IProductExcelRowRepository _productExcelRowRepository;

        public SetRowErrorsHandler(IProductExcelRowRepository productExcelRowRepository)
        {
            _productExcelRowRepository = productExcelRowRepository;
        }

        public async Task<Unit> Handle(SetRowErrors request, CancellationToken cancellationToken)
        {
            var productExcelRow = await _productExcelRowRepository.GetByIdAsync(request.ProductExcelRowId, cancellationToken);

            productExcelRow.SetErrors(request.Errors);

            await _productExcelRowRepository.UpdateAsync(productExcelRow, cancellationToken);

            return Unit.Value;
        }
    }
}

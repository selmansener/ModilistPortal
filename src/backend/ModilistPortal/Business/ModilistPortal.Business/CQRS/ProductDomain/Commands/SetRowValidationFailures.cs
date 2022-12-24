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
    public class SetRowValidationFailures : IRequest
    {
        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }

        public IReadOnlyList<ValidationFailure> ValidationFailures { get; set; }
    }

    internal class SetRowValidationFailuresValidator : AbstractValidator<SetRowValidationFailures>
    {
        public SetRowValidationFailuresValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.BlobId).NotEmpty();
            RuleFor(x => x.RowId).NotEmpty();
            RuleFor(x => x.ValidationFailures).NotEmpty();
        }
    }

    internal class SetRowValidationFailuresHandler : IRequestHandler<SetRowValidationFailures, Unit>
    {
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;

        public SetRowValidationFailuresHandler(IProductExcelUploadRepository productExcelUploadRepository)
        {
            _productExcelUploadRepository = productExcelUploadRepository;
        }

        public async Task<Unit> Handle(SetRowValidationFailures request, CancellationToken cancellationToken)
        {
            var productExcelUpload = await _productExcelUploadRepository.GetByBlobId(request.TenantId, request.BlobId, cancellationToken);

            if (productExcelUpload == null)
            {
                throw new ProductExcelUploadNotFoundException(request.TenantId, request.BlobId);
            }

            var errors = GetErrors(request.ValidationFailures);

            productExcelUpload.SetRowValidationFailures(request.RowId, errors);

            await _productExcelUploadRepository.UpdateAsync(productExcelUpload, cancellationToken);

            return Unit.Value;
        }

        private IDictionary<string, IReadOnlyList<string>> GetErrors(IReadOnlyList<ValidationFailure> validationFailures)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            var groupedErrors = validationFailures.GroupBy(x => x.PropertyName);

            foreach (var groupedError in groupedErrors)
            {
                foreach (var error in groupedError)
                {
                    if (errors.ContainsKey(groupedError.Key))
                    {
                        errors[groupedError.Key].Add(error.ErrorCode);
                    }
                    else
                    {
                        errors.Add(groupedError.Key, new List<string>()
                            {
                                error.ErrorCode
                            });
                    }
                }
            }

            return errors.ToDictionary(x => x.Key, y => (IReadOnlyList<string>)y.Value);
        }
    }
}


using FluentValidation;

using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Infrastructure.Shared.DTOs
{
    public class BillingAddressDTO
    {
        public BillingType Type { get; private set; }

        public string FullName { get; private set; }

        public string Phone { get; private set; }

        public string Email { get; private set; }

        public string? TCKN { get; private set; }

        public string? TaxNumber { get; private set; }

        public string? TaxOffice { get; private set; }

        public AddressDTO Details { get; set; }
    }

    internal class BillingAddressDTOValidator : AbstractValidator<BillingAddressDTO>
    {
        public BillingAddressDTOValidator()
        {
            RuleFor(x => x.Type).IsInEnum().NotEqual(BillingType.None);
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.TCKN).NotEmpty().When(x => x.Type == BillingType.Individual);
            RuleFor(x => x.TaxNumber).NotEmpty().When(x => x.Type == BillingType.Corporate);
            RuleFor(x => x.TaxOffice).NotEmpty().When(x => x.Type == BillingType.Corporate);
            RuleFor(x => x.Details).NotEmpty();
        }
    }
}

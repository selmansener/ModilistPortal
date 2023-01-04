
using FluentValidation;

namespace ModilistPortal.Infrastructure.Shared.DTOs
{
    public class DeliveryAddressDTO
    {
        public string FullName { get; set; }

        public string Phone { get; set; }

        public string? Email { get; set; }

        public AddressDTO Details { get; set; }
    }

    internal class DeliveryAddressDTOValidator : AbstractValidator<DeliveryAddressDTO>
    {
        public DeliveryAddressDTOValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Phone).NotEmpty();
            RuleFor(x => x.Details).NotEmpty();
        }
    }
}

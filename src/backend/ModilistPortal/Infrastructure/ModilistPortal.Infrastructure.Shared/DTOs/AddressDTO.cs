
using FluentValidation;

namespace ModilistPortal.Infrastructure.Shared.DTOs
{
    public class AddressDTO
    {
        public string City { get; set; }

        public string Disctrict { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string FullAddress { get; set; }
    }

    internal class AddressDTOValidator : AbstractValidator<AddressDTO>
    {
        public AddressDTOValidator()
        {
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Disctrict).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.ZipCode).NotEmpty();
            RuleFor(x => x.FullAddress).NotEmpty();
        }
    }
}

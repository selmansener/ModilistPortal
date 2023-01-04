
using FluentValidation;

namespace ModilistPortal.Infrastructure.Shared.DTOs
{
    public class SalesOrderLineItemDTO
    {
        public int ProductId { get; private set; }

        public int Amount { get; private set; }

        public decimal Price { get; private set; }

        public decimal SalesPrice { get; private set; }
    }

    internal class SalesOrderLineItemDTOValidator : AbstractValidator<SalesOrderLineItemDTO>
    {
        public SalesOrderLineItemDTOValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.SalesPrice).NotEmpty();
        }
    }
}

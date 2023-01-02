using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Domains.Models.SalesOrderDomain
{
    public class SalesOrderLineItem : BaseEntity
    {
        public SalesOrderLineItem(int salesOrderId, int productId, int amount, decimal price, decimal salesPrice)
        {
            SalesOrderId = salesOrderId;
            ProductId = productId;
            Amount = amount;
            Price = price;
            SalesPrice = salesPrice;
        }

        public int SalesOrderId { get; private set; }

        public SalesOrder SalesOrder { get; private set; }

        public int ProductId { get; private set; }

        public Product Product { get; private set; }

        public int Amount { get; private set; }

        public decimal Price { get; private set; }

        public decimal SalesPrice { get; private set; }
    }

    internal class SalesOrderLineItemValidator : AbstractValidator<SalesOrderLineItem>
    {
        public SalesOrderLineItemValidator()
        {
            RuleFor(x => x.SalesOrderId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Amount).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
            RuleFor(x => x.SalesOrder).NotEmpty();
        }
    }
}

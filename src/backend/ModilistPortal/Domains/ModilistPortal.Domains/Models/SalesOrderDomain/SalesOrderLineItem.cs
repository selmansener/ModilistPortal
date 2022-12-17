using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Domains.Models.SalesOrderDomain
{
    public class SalesOrderLineItem : BaseEntity
    {
        public int SalesOrderId { get; private set; }

        public SalesOrder SalesOrder { get; private set; }

        public int ProductId { get; private set; }

        public Product Product { get; private set; }

        public int Amount { get; private set; }

        public decimal Price { get; private set; }

        public decimal SalesPrice { get; private set; }
    }
}

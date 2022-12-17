using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.SalesOrderDomain;

namespace ModilistPortal.Domains.Models.ReturnDomain
{
    public class Return : BaseEntity
    {
        public int SalesOrderId { get; private set; }

        public SalesOrder SalesOrder { get; private set; }
    }
}

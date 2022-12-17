using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class Brand : BaseEntity
    {
        public string Name { get; private set; }

        public bool IsVerified { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.TenantDomain;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class ProductExcelUpload : BaseEntity
    {
        public Guid BlobId { get; private set; }

        public string FileName { get; private set; }

        public string Extension { get; private set; }

        public int TenantId { get; private set; }

        public Tenant Tenant { get; private set; }
    }
}

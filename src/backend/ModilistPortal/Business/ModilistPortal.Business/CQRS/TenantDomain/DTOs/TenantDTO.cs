using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.CQRS.TenantDomain.DTOs
{
    public class TenantDTO
    {
        public string Name { get; set; }

        public string TCKN { get; set; }

        public string TaxNumber { get; set; }

        public string TaxOffice { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public TenantType Type { get; set; }

        public bool IsVerified { get; set; }
    }
}

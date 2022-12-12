using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Business.CQRS.AccountDomain.DTOs
{
    public class VerificationResultDTO
    {
        public string RedirectUrl { get; set; }

        public bool IsSuccess { get; set; }
    }
}

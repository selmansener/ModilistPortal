using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Interfaces
{
    public interface IClientException
    {
        int StatusCode { get; }
    }
}

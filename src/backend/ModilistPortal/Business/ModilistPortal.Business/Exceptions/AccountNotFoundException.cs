using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Infrastructure.Shared.Interfaces;

namespace ModilistPortal.Business.Exceptions
{
    internal class AccountNotFoundException : Exception, IClientException
    {
        public AccountNotFoundException(Guid id)
            : base($"Account not found with Id: {id}")
        {
            Id = id;
        }

        public Guid Id { get; private set; }

        public int Code => 404;

        public int StatusCode => 404;
    }

    internal class AccountNotFoundInternalException : Exception
    {
        public AccountNotFoundInternalException(Guid id)
            : base($"Account not found with Id: {id}")
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}

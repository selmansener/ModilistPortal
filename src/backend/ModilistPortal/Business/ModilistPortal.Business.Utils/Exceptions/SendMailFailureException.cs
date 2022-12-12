using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Business.Utils.Exceptions
{
    internal class SendMailFailureException : Exception
    {
        public SendMailFailureException(int statusCode, string reason)
        {
            StatusCode = statusCode;
            Reason = reason;
        }

        public int StatusCode { get; private set; }

        public string Reason { get; private set; }
    }
}

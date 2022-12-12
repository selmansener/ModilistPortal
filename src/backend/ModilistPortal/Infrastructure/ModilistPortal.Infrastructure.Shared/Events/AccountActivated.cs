using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class AccountActivated : BaseEvent
    {
        public AccountActivated(string publisherId, PublisherType publisherType, Guid accountId)
            : base(publisherId, publisherType)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; set; }

        public override string Version => "1.0";
    }
}

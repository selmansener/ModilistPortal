
using ModilistPortal.Infrastructure.Shared.DTOs;

namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class MarketplaceSalesOrderCreated : BaseEvent
    {
        public MarketplaceSalesOrderCreated(string publisherId, PublisherType publisherType)
            : base(publisherId, publisherType)
        {
        }

        public int MarketplaceSalesOrderId { get; set; }

        public int TenantId { get; set; }

        public DeliveryAddressDTO DeliveryAddress { get; set; }

        public BillingAddressDTO BillingAddress { get; set; }

        public IEnumerable<SalesOrderLineItemDTO> LineItems { get; set; }

        public DateTime CreatedAt { get; set; }

        public override string Version => "1.0";
    }
}

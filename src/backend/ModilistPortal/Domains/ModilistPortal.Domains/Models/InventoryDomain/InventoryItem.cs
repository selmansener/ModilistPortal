
using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Domains.Models.InventoryDomain
{
    public class InventoryItem : BaseEntity
    {
        public int ProductId { get; private set; }

        public Product Product { get; private set; }

        public int Amount { get; private set; }
    }
}

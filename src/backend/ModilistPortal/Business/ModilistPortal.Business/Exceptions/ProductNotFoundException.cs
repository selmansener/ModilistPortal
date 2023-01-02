
using ModilistPortal.Infrastructure.Shared.Interfaces;

namespace ModilistPortal.Business.Exceptions
{
    internal class ProductNotFoundException : Exception, IClientException
    {
        public ProductNotFoundException(int productId, int tenantId)
            : base($"Product not found with Id: {productId} and TenantId: {tenantId}")
        {
            TenantId = tenantId;
            ProductId = productId;
        }

        public int TenantId { get; private set; }

        public int ProductId { get; private set; }

        public int StatusCode => 404;
    }
}

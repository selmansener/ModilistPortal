
using ModilistPortal.Infrastructure.Shared.Interfaces;

namespace ModilistPortal.Business.Exceptions
{
    internal class TenantNotFoundException : Exception, IClientException
    {
        public TenantNotFoundException(Guid accountId)
            : base($"Tenant not found with AccountId: {accountId}")
        {
            AccountId = accountId;
        }

        public TenantNotFoundException(int tenantId)
            : base($"Tenant not found with Id: {tenantId}")
        {
            TenantId = tenantId;
        }

        public Guid? AccountId { get; private set; }

        public int? TenantId { get; private set; }

        public int StatusCode => 404;
    }
}

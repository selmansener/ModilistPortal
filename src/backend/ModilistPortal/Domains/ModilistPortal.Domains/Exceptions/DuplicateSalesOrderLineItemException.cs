namespace ModilistPortal.Domains.Exceptions
{
    internal class DuplicateSalesOrderLineItemException : Exception
    {
        public DuplicateSalesOrderLineItemException(int tenantId, int salesOrderId, int productId)
            : base($"SalesOrder (Id: {salesOrderId}) already has a LineItem with ProductId: {productId}. TenantId: {tenantId} ")
        {
            TenantId = tenantId;
            SalesOrderId = salesOrderId;
            ProductId = productId;
        }

        public int TenantId { get; set; }

        public int SalesOrderId { get; set; }

        public int ProductId { get; set; }
    }
}

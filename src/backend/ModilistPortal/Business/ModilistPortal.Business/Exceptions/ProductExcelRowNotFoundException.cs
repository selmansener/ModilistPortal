using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.Exceptions
{
    internal class ProductExcelRowNotFoundException : Exception
    {
        public ProductExcelRowNotFoundException(int tenantId, Guid blobId, int rowId)
            : base($"{nameof(ProductExcelRow)} not found with TenantId: {tenantId}, BlobId: {blobId}, RowId: {rowId}")
        {
            TenantId = tenantId;
            BlobId = blobId;
            RowId = rowId;
        }

        public int TenantId { get; private set; }

        public Guid BlobId { get; private set; }

        public int RowId { get; private set; }
    }
}

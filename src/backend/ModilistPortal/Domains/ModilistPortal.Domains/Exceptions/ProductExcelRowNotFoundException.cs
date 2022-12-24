
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Domains.Exceptions
{
    internal class ProductExcelRowNotFoundException : Exception
    {
        public ProductExcelRowNotFoundException(int tenantId, Guid blobId, int rowId)
            : base($"{nameof(ProductExcelRow)}  not found with RowId: {rowId}, TenantId: {tenantId}, BlobId: {blobId}")
        {
            RowId = rowId;
            TenantId = tenantId;
            BlobId = blobId;
        }

        public int RowId { get; set; }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }
    }
}

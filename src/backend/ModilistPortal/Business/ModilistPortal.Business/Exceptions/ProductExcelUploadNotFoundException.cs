
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.Exceptions
{
    public class ProductExcelUploadNotFoundException : Exception
    {
        public ProductExcelUploadNotFoundException(int tenantId, Guid blobId)
            : base($"{nameof(ProductExcelUpload)} not found with TenantId: {tenantId} and BlobId: {blobId}")
        {
            TenantId = tenantId;
            BlobId = blobId;

            Data.Add(nameof(TenantId), TenantId);
            Data.Add(nameof(BlobId), BlobId);
        }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }
    }
}

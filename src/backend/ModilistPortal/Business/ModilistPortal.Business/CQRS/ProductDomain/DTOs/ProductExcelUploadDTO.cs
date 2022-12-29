namespace ModilistPortal.Business.CQRS.ProductDomain.DTOs
{
    public class ProductExcelUploadDTO
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public string OriginalFileName { get; set; }

        public string Extension { get; set; }

        public string Url { get; set; }

        public string ContentType { get; set; }

        public long FileSizeInBytes { get; set; }

        public string FileSize { get; set; }
    }
}

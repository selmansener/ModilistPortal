using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.TenantDomain;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class ProductExcelUpload : BaseEntity
    {
        public ProductExcelUpload(int tenantId, Guid blobId, string originalFileName, string extension, string url, string contentType, int fileSizeInMB)
        {
            TenantId = tenantId;
            BlobId = blobId;
            OriginalFileName = originalFileName;
            Extension = extension;
            Url = url;
            ContentType = contentType;
            FileSizeInMB = fileSizeInMB;
        }

        public int TenantId { get; private set; }

        public Tenant Tenant { get; private set; }

        public Guid BlobId { get; private set; }

        public string OriginalFileName { get; private set; }

        public string Extension { get; private set; }

        public string Url { get; private set; }

        public string ContentType { get; private set; }

        public int FileSizeInMB { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class RawProductDataUploaded : BaseEvent
    {
        public RawProductDataUploaded(string publisherId, PublisherType publisherType, int tenantId, Guid blobId, string containerName, string blobName, string blobFullPath, string fileExtension, string timestamp)
            : base(publisherId, publisherType)
        {
            TenantId = tenantId;
            ContainerName = containerName;
            BlobName = blobName;
            BlobFullPath = blobFullPath;
            FileExtension = fileExtension;
            Timestamp = timestamp;
            BlobId = blobId;
        }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public string ContainerName { get; set; }

        public string BlobName { get; set; }

        public string BlobFullPath { get; set; }

        public string FileExtension { get; set; }

        public string Timestamp { get; set; }

        public string TimestampFormat => "yyyyMMdd";

        public override string Version => "1.0";
    }
}

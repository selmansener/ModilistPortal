using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class ProductValidationSucceeded : BaseEvent
    {
        public ProductValidationSucceeded(string publisherId, PublisherType publisherType, int tenantId, Guid blobId, int rowId)
            : base(publisherId, publisherType)
        {
            TenantId = tenantId;
            BlobId = blobId;
            RowId = rowId;
        }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }

        public override string Version => "1.0";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class ProductInsertionFailed : BaseEvent
    {
        public ProductInsertionFailed(string publisherId, PublisherType publisherType, int productExcelRowId, int tenantId, Guid blobId, int rowId, IDictionary<string, IReadOnlyList<string>> errors)
            : base(publisherId, publisherType)
        {
            TenantId = tenantId;
            BlobId = blobId;
            RowId = rowId;
            Errors = errors;
            ProductExcelRowId = productExcelRowId;
        }

        public int ProductExcelRowId { get; set; }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }

        public IDictionary<string, IReadOnlyList<string>> Errors { get; set; }

        public override string Version => "1.0";
    }
}

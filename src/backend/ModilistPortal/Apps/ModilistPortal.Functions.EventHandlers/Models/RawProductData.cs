using System.Collections.Generic;

using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Functions.EventHandlers.Models
{
    public class RawProductData
    {
        public RawProductData(int rowId)
        {
            RowId = rowId;
        }

        public int RowId { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string Price { get; set; }

        public string SalesPrice { get; set; }

        public string StockAmount { get; set; }

        public string Gender { get; set; }

        public string Size { get; set; }

        public string Colors { get; set; }
    }
}

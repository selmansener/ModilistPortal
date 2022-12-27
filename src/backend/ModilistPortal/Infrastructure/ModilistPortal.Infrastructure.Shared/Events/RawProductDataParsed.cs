namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class RawProductDataParsed : BaseEvent
    {
        public RawProductDataParsed(
            string publisherId,
            PublisherType publisherType,
            int productExcelUploadId,
            int tenantId,
            Guid blobId,
            int rowId,
            string name,
            string sKU,
            string barcode,
            string brand,
            string category,
            string price,
            string salesPrice,
            string stockAmount)
            : base(publisherId, publisherType)
        {
            TenantId = tenantId;
            BlobId = blobId;
            RowId = rowId;
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            Brand = brand;
            Category = category;
            Price = price;
            SalesPrice = salesPrice;
            StockAmount = stockAmount;
            ProductExcelUploadId = productExcelUploadId;
        }

        public int ProductExcelUploadId { get; set; }

        public int TenantId { get; set; }

        public Guid BlobId { get; set; }

        public int RowId { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string Price { get; set; }

        public string SalesPrice { get; set; }

        public string StockAmount { get; set; }

        public override string Version => "1.0";
    }
}

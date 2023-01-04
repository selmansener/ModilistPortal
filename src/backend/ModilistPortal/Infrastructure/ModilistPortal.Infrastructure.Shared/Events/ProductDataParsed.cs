﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class ProductDataParsed : BaseEvent
    {
        public ProductDataParsed(string publisherId, PublisherType publisherType, int productExcelUploadId, int tenantId, int rowId, string name, string sKU, string barcode, string brand, string category, string price, string salesPrice, string stockAmount)
            : base(publisherId, publisherType)
        {
            ProductExcelUploadId = productExcelUploadId;
            TenantId = tenantId;
            RowId = rowId;
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            Brand = brand;
            Category = category;
            Price = price;
            SalesPrice = salesPrice;
            StockAmount = stockAmount;
        }

        public int ProductExcelUploadId { get; set; }

        public int TenantId { get; set; }

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
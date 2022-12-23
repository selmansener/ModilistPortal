using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class ProductExcelRow : BaseEntity
    {
        private readonly List<ProductPropertyError> _errorMappings = new List<ProductPropertyError>();

        public ProductExcelRow(int productExcelUploadId, int rowId, string name, string sKU, string barcode, string brand, string category, string price, string salesPrice, string stockAmount)
        {
            ProductExcelUploadId = productExcelUploadId;
            RowId = rowId;
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            Brand = brand;
            Category = category;
            Price = price;
            SalesPrice = salesPrice;
            StockAmount = stockAmount;
            State = ProductExcelRowState.None;
        }

        public int ProductExcelUploadId { get; private set; }

        public ProductExcelUpload ProductExcelUpload { get; private set; }

        public int RowId { get; private set; }

        public string Name { get; private set; }

        public string SKU { get; private set; }

        public string Barcode { get; private set; }

        public string Brand { get; private set; }

        public string Category { get; private set; }

        public string Price { get; private set; }

        public string SalesPrice { get; private set; }

        public string StockAmount { get; private set; }

        public ProductExcelRowState State { get; private set; }

        public IReadOnlyList<ProductPropertyError> ErrorMappings => _errorMappings;

        public void Update(int rowId, string name, string sKU, string barcode, string brand, string category, string price, string salesPrice, string stockAmount)
        {
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
    }
}

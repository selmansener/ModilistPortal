﻿
using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class Product : BaseEntity
    {
        public Product(string name, string sKU, string barcode, int brandId, string category, decimal price, decimal salesPrice, int taxRatio, int tenantId)
        {
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            BrandId = brandId;
            Category = category;
            Price = price;
            SalesPrice = salesPrice;
            TaxRatio = taxRatio;
            State = ProductState.InReview;
            TenantId = tenantId;
        }

        public string Name { get; private set; }

        public string SKU { get; private set; }

        public string Barcode { get; private set; }

        public int BrandId { get; private set; }

        public Brand Brand { get; private set; }

        public string Category { get; private set; }

        public decimal Price { get; private set; }

        public decimal SalesPrice { get; private set; }

        public int TaxRatio { get; private set; }

        public ProductState State { get; private set; }

        public int TenantId { get; private set; }

        public Tenant Tenant { get; private set; }
    }
}

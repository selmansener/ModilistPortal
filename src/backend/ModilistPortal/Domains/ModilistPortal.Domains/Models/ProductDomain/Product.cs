
using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.InventoryDomain;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class Product : BaseEntity
    {
        private readonly List<ProductImage> _images = new List<ProductImage>();
        private readonly List<string> _colors = new List<string>();

        public Product(string name, string sKU, string barcode, int brandId, string category, decimal price, decimal salesPrice, int taxRatio, int tenantId, Gender gender, string size)
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
            Gender = gender;
            Size = size;
        }

        public string Name { get; private set; }

        public string SKU { get; private set; }

        public string Barcode { get; private set; }

        public int BrandId { get; private set; }

        public Brand Brand { get; private set; }

        public string Category { get; private set; }

        public decimal Price { get; private set; }

        public decimal SalesPrice { get; private set; }

        public string Size { get; private set; }

        public int TaxRatio { get; private set; }

        public ProductState State { get; private set; }

        public Gender Gender { get; private set; }

        public int TenantId { get; private set; }

        public Tenant Tenant { get; private set; }

        public InventoryItem? Inventory { get; private set; }

        public IReadOnlyList<ProductImage> Images => _images;

        public IReadOnlyList<string> Colors => _colors;

        public void AddImage(string name,
            string contentType,
            string url,
            string extension)
        {
            _images.Add(new ProductImage(Id, name, contentType, url, extension));
        }

        public void AddColor(string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                throw new ArgumentNullException(nameof(color));
            }

            _colors.Add(color);
        }
    }
}

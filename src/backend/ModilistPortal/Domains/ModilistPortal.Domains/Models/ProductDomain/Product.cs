
using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class Product : BaseEntity
    {
        private readonly List<ProductImage> _images = new List<ProductImage>();

        public Product(string name, string sKU, string barcode, int brandId, string category, decimal price, decimal salesPrice, int taxRatio, int tenantId, Gender gender, string size, string color)
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
            Color = color;
        }

        public Guid GroupId { get; private set; }

        public string Name { get; private set; }

        public string SKU { get; private set; }

        public string Barcode { get; private set; }

        public int Amount { get; private set; }

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

        public IReadOnlyList<ProductImage> Images => _images;

        public string Color { get; private set; }

        public void AddImage(string name,
            string contentType,
            string url,
            string extension)
        {
            _images.Add(new ProductImage(Id, name, contentType, url, extension));
        }

        /// <summary>
        /// Decreases available stock amount and returns the missing amount if it's negative.
        /// </summary>
        /// <param name="amount">Amount to decrease</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws if the amount parameter equal or less than zero.</exception>
        public int DecreaseAmount(int amount)
        {
            if (amount == default || amount < 0)
            {
                throw new ArgumentException($"amount must be greater than zero.", nameof(amount));
            }

            var remainingAmount = Amount - amount;

            if (remainingAmount > 0)
            {
                Amount = remainingAmount;
            }
            else
            {
                Amount = 0;
            }

            return remainingAmount < 0 ? Math.Abs(remainingAmount) : 0;
        }

        public void SetGroupId(Guid groupId)
        {
            this.GroupId = groupId;
        }
    }
}

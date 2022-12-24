
using ModilistPortal.Domains.Base;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class Brand : BaseEntity
    {
        private readonly List<Product> _products = new List<Product>();

        public Brand(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public string? LogoUrl { get; private set; }

        public string? DocumentUrl { get; private set; }

        public bool IsVerified { get; private set; }

        public DateTime LastVerifiedAt { get; private set; }

        public IReadOnlyList<Product> Products => _products;

        public void Verify()
        {
            IsVerified = true;
            LastVerifiedAt = DateTime.UtcNow;
        }

        public void NonVerify()
        {
            IsVerified = false;
        }
    }
}


using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Business.Exceptions
{
    public class ProductAlreadyExistsException : Exception
    {
        public ProductAlreadyExistsException(int tenantId, string? sku = null, string? barcode = null)
            : base($"{nameof(Product)} already exists with SKU or Barcode: {sku ?? barcode}")
        {
            TenantId = tenantId;
            SKU = sku;
            Barcode = barcode;
            UniqueValue = !string.IsNullOrEmpty(sku) ? UniqueValue.SKU : UniqueValue.Barcode;
        }

        public int TenantId { get; private set; }

        public string? SKU { get; private set; }

        public string? Barcode { get; private set; }

        public UniqueValue UniqueValue { get; private set; }
    }

    public enum UniqueValue
    {
        SKU = 0,
        Barcode = 1,
    }
}

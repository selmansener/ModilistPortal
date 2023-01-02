
using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.Repositories.ProductDomain
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<bool> DoesExistsWithSKU(int tenantId, string sku, CancellationToken cancellationToken);

        Task<bool> DoesExistsWithBarcode(int tenantId, string barcode, CancellationToken cancellationToken);

        Task<Product?> GetByIdAsync(int productId, int tenantId, CancellationToken cancellationToken);
    }

    internal class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ModilistPortalDbContext baseDb)
            : base(baseDb)
        {
        }

        public async Task<bool> DoesExistsWithBarcode(int tenantId, string barcode, CancellationToken cancellationToken)
        {
            return await _baseDb.Products.AnyAsync(x => x.TenantId == tenantId && x.Barcode == barcode, cancellationToken);
        }

        public async Task<bool> DoesExistsWithSKU(int tenantId, string sku, CancellationToken cancellationToken)
        {
            return await _baseDb.Products.AnyAsync(x => x.TenantId == tenantId && x.SKU == sku, cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(int productId, int tenantId, CancellationToken cancellationToken)
        {
            return await _baseDb.Products
                .Include(x => x.Brand)
                .FirstOrDefaultAsync(x => x.Id == productId && x.TenantId == tenantId, cancellationToken);
        }
    }
}

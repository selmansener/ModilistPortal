using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.Repositories.ProductDomain
{
    public interface IProductExcelUploadRepository : IBaseRepository<ProductExcelUpload>
    {
        Task<ProductExcelUpload?> GetByBlobId(int tenantId, Guid blobId, CancellationToken cancellationToken);

        Task<ProductExcelUpload?> GetByBlobIdWithErrors(int tenantId, Guid blobId, CancellationToken cancellationToken);    
    }

    internal class ProductExcelUploadRepository : BaseRepository<ProductExcelUpload>, IProductExcelUploadRepository
    {
        public ProductExcelUploadRepository(ModilistPortalDbContext baseDb) 
            : base(baseDb)
        {
        }

        public async Task<ProductExcelUpload?> GetByBlobId(int tenantId, Guid blobId, CancellationToken cancellationToken)
        {
            return await _baseDb.ProductExcelUploads
                .Include(x => x.Rows)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.BlobId == blobId);
        }

        public async Task<ProductExcelUpload?> GetByBlobIdWithErrors(int tenantId, Guid blobId, CancellationToken cancellationToken)
        {
            return await _baseDb.ProductExcelUploads
                .Include(x => x.Rows)
                .ThenInclude(x => x.ErrorMappings)
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.BlobId == blobId);
        }
    }
}

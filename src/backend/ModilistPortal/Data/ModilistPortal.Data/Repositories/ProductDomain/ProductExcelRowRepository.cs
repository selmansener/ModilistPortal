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
    public interface IProductExcelRowRepository : IBaseRepository<ProductExcelRow>
    {
        Task<ProductExcelRow?> GetByRowId(int tenantId, Guid blobId, int rowId);
    }

    internal class ProductExcelRowRepository : BaseRepository<ProductExcelRow>, IProductExcelRowRepository
    {
        public ProductExcelRowRepository(ModilistPortalDbContext baseDb) 
            : base(baseDb)
        {
        }

        public async Task<ProductExcelRow?> GetByRowId(int tenantId, Guid blobId, int rowId)
        {
            return await _baseDb.ProductExcelRows
                .FirstOrDefaultAsync(x => x.ProductExcelUpload.TenantId == tenantId && x.ProductExcelUpload.BlobId == blobId && x.RowId == rowId, cancellationToken);
        }
    }
}

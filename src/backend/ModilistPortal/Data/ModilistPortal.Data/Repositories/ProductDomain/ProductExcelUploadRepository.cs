using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Data.Repositories.ProductDomain
{
    public interface IProductExcelUploadRepository : IBaseRepository<ProductExcelUpload>
    {

    }

    internal class ProductExcelUploadRepository : BaseRepository<ProductExcelUpload>, IProductExcelUploadRepository
    {
        public ProductExcelUploadRepository(ModilistPortalDbContext baseDb) 
            : base(baseDb)
        {
        }
    }
}

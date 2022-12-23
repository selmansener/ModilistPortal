using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class ProductPropertyError : BaseEntity
    {
        private readonly List<string> _errors = new List<string>();

        public ProductPropertyError(int productExcelRowId, ProductPropertyName propertyName)
        {
            ProductExcelRowId = productExcelRowId;
            PropertyName = propertyName;
        }

        public int ProductExcelRowId { get; private set; }

        public ProductExcelRow ProductExcelRow { get; private set; }

        public ProductPropertyName PropertyName { get; private set; }

        public IReadOnlyList<string> Errors => _errors;

        public void AddError(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                throw new ArgumentNullException(nameof(error));
            }

            _errors.Add(error);
        }
    }
}

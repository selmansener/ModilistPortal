using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mapster;

using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.CQRS.ProductDomain.DTOs
{
    public class QueryProductExcelRowDTO
    {
        public int Id { get; set; }

        public int RowId { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public Gender Gender { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }

        public string Price { get; set; }

        public string SalesPrice { get; set; }

        public string StockAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<ProductErrorMappingsDTO> ErrorMappings { get; set; }
    }

    internal class QueryProductExcelRowDTOMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<ProductExcelRow, QueryProductExcelRowDTO>()
                .Map(dest => dest.ErrorMappings, src => src.ErrorMappings.Select(x => new ProductErrorMappingsDTO
                {
                    PropertyName = x.PropertyName,
                    Errors = x.Errors
                }));
        }
    }

    public class ProductErrorMappingsDTO
    {
        public ProductPropertyName PropertyName { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}

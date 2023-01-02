
using Mapster;

using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.CQRS.ProductDomain.DTOs
{
    public class QueryProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public int BrandId { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public decimal SalesPrice { get; set; }

        public int TaxRatio { get; set; }

        public ProductState State { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    internal class QueryProductDTOMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Product, QueryProductDTO>()
                .Map(dest => dest.Brand, src => src.Brand.Name);
        }
    }
}

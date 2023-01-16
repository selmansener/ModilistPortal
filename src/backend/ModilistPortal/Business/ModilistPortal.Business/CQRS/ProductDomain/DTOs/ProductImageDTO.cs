using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Business.CQRS.ProductDomain.DTOs
{
    public class ProductImageDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }

        public string Extension { get; set; }

        public string Url { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class ProductImage : BaseEntity
    {
        public ProductImage(int productId, string name, string contentType, string url, string extension)
        {
            Name = name;
            Extension = extension;
            ContentType = contentType;
            Url = url;
            ProductId = productId;
        }

        public string Name { get; private set; }

        public string Extension { get; private set; }

        public string ContentType { get; private set; }

        public string Url { get; private set; }

        public int ProductId { get; private set; }

        public Product Product { get; private set; }
    }
}

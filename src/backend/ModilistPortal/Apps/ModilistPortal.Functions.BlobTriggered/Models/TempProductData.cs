using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Functions.BlobTriggered.Models
{
    internal class TempProductData
    {
        public int TenantId { get; set; }

        public List<RawProductData> Products { get; set; }
    }
}

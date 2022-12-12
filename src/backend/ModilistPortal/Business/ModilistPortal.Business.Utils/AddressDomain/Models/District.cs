using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Business.Utils.AddressDomain.Models
{
    public class District
    {
        public string Name { get; internal set; }

        public string Code { get; internal set; }

        public string CityName { get; internal set; }

        public string CityCode { get; internal set; }
    }
}

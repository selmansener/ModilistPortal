using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Configurations
{
    public class DbConnectionOptions
    {
        public string Server { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}

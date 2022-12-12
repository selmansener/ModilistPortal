using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using ModilistPortal.Business.Utils.AddressDomain;
using ModilistPortal.Business.Utils.Messages;

namespace ModilistPortal.Business.Utils.Extensions
{
    public static class BusinessUtilExtensions
    {
        public static IServiceCollection AddBusinessUtils(this IServiceCollection services)
        {
            services.AddSingleton<IAddressService, AddressService>();
            services.AddHttpClient<IMailProvider, MailProvider>();

            return services;
        }
    }
}

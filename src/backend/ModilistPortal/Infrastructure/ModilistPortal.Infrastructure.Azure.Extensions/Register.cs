using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;

namespace ModilistPortal.Infrastructure.Azure.Extensions
{
    public static class Register
    {
        public static IServiceCollection AddBlobClientFactory(this IServiceCollection services)
        {
            services.AddScoped<IBlobClientFactory, BlobClientFactory>();
            services.AddScoped<IEventGridPublisherClientFactory, EventGridPublisherClientFactory>();

            return services;
        }
    }
}

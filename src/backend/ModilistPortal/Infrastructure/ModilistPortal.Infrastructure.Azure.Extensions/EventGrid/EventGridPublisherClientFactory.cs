using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;
using Azure;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;

namespace ModilistPortal.Infrastructure.Azure.Extensions.EventGrid
{
    public interface IEventGridPublisherClientFactory
    {
        EventGridPublisherClient GetClient(EventGridClientOptions options);
    }

    internal class EventGridPublisherClientFactory : IEventGridPublisherClientFactory
    {
        public EventGridPublisherClient GetClient(EventGridClientOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(options.Url))
            {
                throw new ArgumentNullException(nameof(options.Url));
            }

            if (string.IsNullOrEmpty(options.Secret))
            {
                throw new ArgumentNullException(nameof(options.Secret));
            }

            return new EventGridPublisherClient(
                new Uri(options.Url),
                new AzureKeyCredential(options.Secret));
        }
    }
}

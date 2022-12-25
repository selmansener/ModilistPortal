using System;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    internal class ProductInsertionFailedHandler
    {
        [FunctionName(nameof(ProductInsertionFailedHandler))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var productInsertionFailed = JsonConvert.DeserializeObject<ProductInsertionFailed>(eventGridEvent.Data.ToString());

            try
            {
                // TODO: set product excel row errors
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

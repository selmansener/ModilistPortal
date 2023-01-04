using System;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using MediatR;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using ModilistPortal.Business.CQRS.SalesOrderDomain.Commands;
using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class CreateSalesOrderHandler
    {
        private readonly IMediator _mediator;

        public CreateSalesOrderHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(CreateSalesOrderHandler))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger logger, CancellationToken cancellationToken)
        {
            var salesOrderCreated = JsonConvert.DeserializeObject<MarketplaceSalesOrderCreated>(eventGridEvent.Data.ToString());

            try
            {
                await _mediator.Send(new CreateSalesOrder(salesOrderCreated.TenantId, salesOrderCreated.MarketplaceSalesOrderId, salesOrderCreated.CreatedAt, salesOrderCreated.DeliveryAddress, salesOrderCreated.BillingAddress, salesOrderCreated.LineItems), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateSalesOrder failed for MarketplaceSalesOrder with Id: {Id}", salesOrderCreated.MarketplaceSalesOrderId);
                throw;
            }
        }
    }
}

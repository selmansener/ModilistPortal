using System;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using MediatR;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class ProductInsertionFailedHandler
    {
        private readonly IMediator _mediator;

        public ProductInsertionFailedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(ProductInsertionFailedHandler))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var productInsertionFailed = JsonConvert.DeserializeObject<ProductInsertionFailed>(eventGridEvent.Data.ToString());

            try
            {
                await _mediator.Send(new SetRowErrors
                {
                    ProductExcelRowId = productInsertionFailed.ProductExcelRowId,
                    BlobId = productInsertionFailed.BlobId,
                    RowId = productInsertionFailed.RowId,
                    TenantId = productInsertionFailed.TenantId,
                    Errors = productInsertionFailed.Errors
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Inserting product data failed. AdditionalData: TenantId: {TenantId}, BlobId: {BlobId}, RowId: {RowId}", productInsertionFailed.TenantId, productInsertionFailed.BlobId, productInsertionFailed.RowId);
                throw;
            }
        }
    }
}

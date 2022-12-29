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
    public class CreateProductHandler
    {
        private readonly IMediator _mediator;

        public CreateProductHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(CreateProductHandler))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger logger, CancellationToken cancellationToken)
        {
            var productDataParsed = JsonConvert.DeserializeObject<ProductDataParsed>(eventGridEvent.Data.ToString());

            try
            {
                await _mediator.Send(new CreateProductFromExcel(
                    productDataParsed.ProductExcelUploadId,
                    productDataParsed.TenantId,
                    productDataParsed.RowId,
                    productDataParsed.Name,
                    productDataParsed.SKU,
                    productDataParsed.Barcode,
                    productDataParsed.Brand,
                    productDataParsed.Category,
                    productDataParsed.Price,
                    productDataParsed.SalesPrice,
                    productDataParsed.StockAmount), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Creating product failed.");
            }
        }
    }
}

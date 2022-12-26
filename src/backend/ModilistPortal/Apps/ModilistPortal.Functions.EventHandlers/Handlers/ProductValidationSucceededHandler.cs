using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using MediatR;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.CQRS.ProductDomain.Queries;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Enums;
using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

using NPOI.SS.Formula.Functions;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class ProductValidationSucceededHandler
    {
        private readonly EventGridPublisherClient _eventGridPublisherClient;
        private readonly string _barcodeIndexName = "IX_Products_TenantId_Barcode_DeletedAt";
        private readonly string _skuIndexName = "IX_Products_TenantId_SKU_DeletedAt";
        private readonly IMediator _mediator;

        public ProductValidationSucceededHandler(
            IMediator mediator,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> eventGridOptions)
        {
            _mediator = mediator;
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
        }

        [FunctionName(nameof(ProductValidationSucceededHandler))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var productValidationSucceeded = JsonConvert.DeserializeObject<ProductValidationSucceeded>(eventGridEvent.Data.ToString());

            try
            {
                var productExcelRow = await _mediator.Send(new GetProductExcelRow
                {
                    TenantId = productValidationSucceeded.TenantId,
                    BlobId = productValidationSucceeded.BlobId,
                    RowId = productValidationSucceeded.RowId,
                });

                var productId = await _mediator.Send(new CreateProduct
                {
                    TenantId = productValidationSucceeded.TenantId,
                    Name = productExcelRow.Name,
                    SKU = productExcelRow.SKU,
                    Barcode = productExcelRow.Barcode,
                    Brand = productExcelRow.Brand,
                    Category = productExcelRow.Category,
                    Price = decimal.Parse(productExcelRow.Price),
                    SalesPrice = decimal.Parse(productExcelRow.SalesPrice),
                });

            }
            catch (Exception ex) when (ex is ProductAlreadyExistsException alreadyExistsException)
            {
                await TriggerProductInsertionFailedEvent(productValidationSucceeded, "AlreadyExists", cancellationToken, alreadyExistsException.UniqueValue == UniqueValue.SKU ? ProductPropertyName.SKU : ProductPropertyName.Barcode);

                throw;
            }
            catch (Exception ex) when (ex is DbUpdateException dbUpdateException)
            {
                // TODO: This is really ugly but keep it for now.
                if (dbUpdateException?.InnerException is SqlException sqlException && sqlException != null)
                {
                    if (sqlException.Number == 2601)
                    {
                        if (sqlException.Message.Contains(_skuIndexName))
                        {
                            await TriggerProductInsertionFailedEvent(productValidationSucceeded, "AlreadyExists", cancellationToken, ProductPropertyName.SKU);
                        }
                        else if (sqlException.Message.Contains(_barcodeIndexName))
                        {
                            await TriggerProductInsertionFailedEvent(productValidationSucceeded, "AlreadyExists", cancellationToken, ProductPropertyName.Barcode);
                        }
                        else
                        {
                            await TriggerProductInsertionFailedEvent(productValidationSucceeded, sqlException.Message, cancellationToken, ProductPropertyName.None);
                        }
                    }
                    else
                    {
                        await TriggerProductInsertionFailedEvent(productValidationSucceeded, sqlException.Message, cancellationToken, ProductPropertyName.None);
                    }
                }
                else
                {
                    await TriggerProductInsertionFailedEvent(productValidationSucceeded, dbUpdateException.Message, cancellationToken, ProductPropertyName.None);
                }

                throw;
            }
            catch (Exception ex)
            {
                await TriggerProductInsertionFailedEvent(productValidationSucceeded, ex.Message, cancellationToken, ProductPropertyName.None);
                log.LogError(ex, "Create product failed. AdditionalInfo: TenantId: {TenantId}, BlobId: {BlobId}, RowId: {RowId}", productValidationSucceeded.TenantId, productValidationSucceeded.BlobId, productValidationSucceeded.RowId);
                throw;
            }
        }

        private async Task TriggerProductInsertionFailedEvent(ProductValidationSucceeded productValidationSucceeded, string reason, CancellationToken cancellationToken, ProductPropertyName productPropertyName = ProductPropertyName.None)
        {
            var errors = new Dictionary<string, List<string>>
            {
                { 
                    productPropertyName.ToString(), new List<string>
                    {
                        reason
                    }
                }
            };

            var productInsertionFailed = new ProductInsertionFailed(EventPublishers.EventHandlers, PublisherType.System, productValidationSucceeded.TenantId, productValidationSucceeded.BlobId, productValidationSucceeded.RowId, errors.ToDictionary(x => x.Key, y => (IReadOnlyList<string>)y.Value));


            var productInsertionFailedEvent = new EventGridEvent(
                nameof(ProductInsertionFailed),
                nameof(ProductInsertionFailed),
                productInsertionFailed.Version,
                productInsertionFailed);

            await _eventGridPublisherClient.SendEventAsync(productInsertionFailedEvent, cancellationToken);
        }
    }
}

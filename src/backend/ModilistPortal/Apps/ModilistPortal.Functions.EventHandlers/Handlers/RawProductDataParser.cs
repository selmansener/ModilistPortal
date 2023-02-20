// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Functions.EventHandlers.Models;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Configurations;
using ModilistPortal.Infrastructure.Shared.Constants;
using ModilistPortal.Infrastructure.Shared.Enums;
using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class RawProductDataParser
    {
        private readonly IMediator _mediator;
        private readonly EventGridPublisherClient _eventGridPublisherClient;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
        };

        public RawProductDataParser(
            IBlobClientFactory blobClientFactory,
            IOptions<StorageConnectionStrings> options,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> eventGridOptions,
            IMediator mediator)
        {
            _blobServiceClient = blobClientFactory.GetClient(options.Value.AppStorage);
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
            _mediator = mediator;
        }

        [FunctionName(nameof(RawProductDataParser))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger logger, CancellationToken cancellationToken)
        {
            var rawProductDataUploaded = JsonConvert.DeserializeObject<RawProductDataUploaded>(eventGridEvent.Data.ToString());

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(rawProductDataUploaded.ContainerName);

                var blobClient = containerClient.GetBlobClient(rawProductDataUploaded.BlobFullPath);

                var blobContentResult = await blobClient.DownloadContentAsync(cancellationToken);

                var tempProductData = blobContentResult.Value.Content.ToObjectFromJson<TempProductData>(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });

                var brands = tempProductData.Products.Select(x => x.Brand).Distinct().ToList();

                // To prevent race condition we create brands here.
                await _mediator.Send(new CreateBrandsIfNotExists((IReadOnlyList<string>)brands));

                var tasks = new List<Task>();
                foreach (var rawProductData in tempProductData.Products)
                {
                    var productDataParsed = new ProductDataParsed(
                        EventPublishers.EventHandlers,
                        PublisherType.System,
                        rawProductDataUploaded.ProductExcelUploadId,
                        rawProductDataUploaded.TenantId,
                        rawProductData.RowId,
                        rawProductData.Name,
                        rawProductData.SKU,
                        rawProductData.Barcode,
                        rawProductData.Brand,
                        rawProductData.Category,
                        rawProductData.Price,
                        rawProductData.SalesPrice,
                        rawProductData.StockAmount,
                        rawProductData.Gender,
                        rawProductData.Size,
                        rawProductData.Color);

                    var productDataParsedEvent = new EventGridEvent(
                        nameof(ProductDataParsed),
                        nameof(ProductDataParsed),
                        productDataParsed.Version,
                        productDataParsed);

                    tasks.Add(_eventGridPublisherClient.SendEventAsync(productDataParsedEvent, cancellationToken));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Parsing raw product file {name} failed", rawProductDataUploaded.BlobName);
                throw;
            }
        }
    }
}

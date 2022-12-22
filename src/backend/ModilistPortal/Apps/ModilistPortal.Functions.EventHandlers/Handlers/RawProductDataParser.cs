// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Configurations;
using ModilistPortal.Infrastructure.Shared.Events;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using ModilistPortal.Functions.EventHandlers.Models;
using Azure.Storage.Blobs.Models;
using ModilistPortal.Infrastructure.Shared.Constants;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class RawProductDataParser
    {
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
            IOptions<EventGridClientOptions> eventGridOptions)
        {
            _blobServiceClient = blobClientFactory.GetClient(options.Value.AppStorage);
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
        }

        [FunctionName(nameof(RawProductDataParser))]
        public async Task RunAsync([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var rawProductDataUploaded = JsonConvert.DeserializeObject<RawProductDataUploaded>(eventGridEvent.Data.ToString());

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(rawProductDataUploaded.ContainerName);

                var blobClient = containerClient.GetBlobClient(rawProductDataUploaded.BlobFullPath);

                var blobContentResult = await blobClient.DownloadContentAsync(cancellationToken);

                var tempProductData = blobContentResult.Value.Content.ToObjectFromJson<TempProductData>();

                BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(StorageContainerNames.PARSED_PRODUCT_DATA);
                await container.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.None);

                var tasks = new List<Task>();
                foreach (var _rawProductData in tempProductData.Products)
                {
                    var task = Task.Factory.StartNew(async (data) =>
                    {
                        var rawProductData = (RawProductData)data;

                        var blobFullPath = $"{rawProductDataUploaded.TenantId}/{rawProductDataUploaded.Timestamp}/{rawProductDataUploaded.BlobId}/{rawProductData.RowId}.json";
                        BlobClient jsonBlobClient = container.GetBlobClient(blobFullPath);

                        var productDataJson = JsonConvert.SerializeObject(rawProductData, Formatting.Indented, _jsonSerializerSettings);

                        await jsonBlobClient.UploadAsync(BinaryData.FromString(productDataJson), cancellationToken);
                        jsonBlobClient.SetHttpHeaders(new BlobHttpHeaders
                        {
                            ContentType = "application/json",
                            ContentEncoding = "utf8"
                        });

                        var rawProductDataParsed = new RawProductDataParsed(
                            EventPublishers.EventHandlers,
                            PublisherType.System,
                            rawProductDataUploaded.TenantId,
                            rawProductDataUploaded.BlobId,
                            rawProductData.RowId,
                            rawProductData.Name,
                            rawProductData.SKU,
                            rawProductData.Barcode,
                            rawProductData.Brand,
                            rawProductData.Category,
                            rawProductData.Price,
                            rawProductData.SalesPrice,
                            rawProductData.StockAmount);

                        var rawProductDataParsedEvent = new EventGridEvent(
                           nameof(RawProductDataParsed),
                           nameof(RawProductDataParsed),
                           rawProductDataParsed.Version,
                           rawProductDataParsed);

                        await _eventGridPublisherClient.SendEventAsync(rawProductDataParsedEvent, cancellationToken);

                    }, _rawProductData, cancellationToken);
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Parsing raw product file {name} failed", rawProductDataUploaded.BlobName);
                throw;
            }
        }
    }
}

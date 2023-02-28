// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using ModilistPortal.Functions.EventHandlers.Models;
using ModilistPortal.Infrastructure.Shared.Events;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Shared.Configurations;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json.Serialization;
using System.IO;
using ModilistPortal.Infrastructure.Shared.Constants;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using System.Linq;
using NPOI.SS.Util;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class ProductExcelParser
    {
        private readonly EventGridPublisherClient _eventGridPublisherClient;
        private readonly BlobServiceClient _blobServiceClient;
        private static Type _productDataType = typeof(RawProductData);
        private readonly IDictionary<int, MethodInfo> _productPropertyMap = new Dictionary<int, MethodInfo>
        {
                { 0, _productDataType.GetProperty("Name").GetSetMethod() },
                { 1, _productDataType.GetProperty("SKU").GetSetMethod() },
                { 2, _productDataType.GetProperty("Barcode").GetSetMethod() },
                { 3, _productDataType.GetProperty("Brand").GetSetMethod() },
                { 4, _productDataType.GetProperty("Category").GetSetMethod() },
                { 5, _productDataType.GetProperty("Gender").GetSetMethod() },
                { 6, _productDataType.GetProperty("Size").GetSetMethod() },
                { 7, _productDataType.GetProperty("Color").GetSetMethod() },
                { 8, _productDataType.GetProperty("Price").GetSetMethod() },
                { 9, _productDataType.GetProperty("SalesPrice").GetSetMethod() },
                { 10, _productDataType.GetProperty("StockAmount").GetSetMethod() },
        };

        public ProductExcelParser(
            IBlobClientFactory blobClientFactory,
            IOptions<StorageConnectionStrings> options,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> eventGridOptions)
        {
            _blobServiceClient = blobClientFactory.GetClient(options.Value.AppStorage);
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
        }

        [FunctionName(nameof(ProductExcelParser))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var productExcelUploaded = JsonConvert.DeserializeObject<ProductExcelUploaded>(eventGridEvent.Data.ToString());
            
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(productExcelUploaded.ContainerName);

                var blobClient = containerClient.GetBlobClient(productExcelUploaded.BlobFullPath);

                var blobContentResult = await blobClient.DownloadContentAsync(cancellationToken);

                var stream = blobContentResult.Value.Content.ToStream();

                var tempProductData = new TempProductData
                {
                    TenantId = productExcelUploaded.TenantId,
                    Products = new List<RawProductData>()
                };

                ReadExcel(stream, tempProductData, productExcelUploaded.FileExtension);

                var productDataJson = JsonConvert.SerializeObject(tempProductData, Formatting.Indented, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });

                var fullBlobPath = productExcelUploaded.BlobFullPath.Replace(productExcelUploaded.FileExtension, "json");
                var blobName = fullBlobPath.Split("/").LastOrDefault();

                BlobClient jsonBlobClient = containerClient.GetBlobClient(fullBlobPath);
                await jsonBlobClient.UploadAsync(BinaryData.FromString(productDataJson), cancellationToken);
                jsonBlobClient.SetHttpHeaders(new BlobHttpHeaders
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf8"
                });

                var rawProductDataUploaded = new RawProductDataUploaded(
                    EventPublishers.EventHandlers,
                    PublisherType.System,
                    productExcelUploaded.ProductExcelUploadId,
                    productExcelUploaded.TenantId,
                    productExcelUploaded.BlobId,
                    StorageContainerNames.PRODUCT_EXCEL_UPLOADS,
                    blobName,
                    fullBlobPath,
                    "json",
                    productExcelUploaded.Timestamp,
                    productExcelUploaded.GroupId);

                var rawProductDataUploadedEvent = new EventGridEvent(
                    nameof(RawProductDataUploaded),
                    nameof(RawProductDataUploaded),
                    rawProductDataUploaded.Version,
                    rawProductDataUploaded);

                await _eventGridPublisherClient.SendEventAsync(rawProductDataUploadedEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Parsing excel file {name} failed", productExcelUploaded.BlobName);
                throw;
            }
        }

        public void ReadExcel(Stream stream, TempProductData tempProductData, string fileExtension)
        {
            IWorkbook workbook = fileExtension == "xls" ? new HSSFWorkbook(stream) : new XSSFWorkbook(stream);
            workbook.MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;
            // We assume there is one sheet atleast
            ISheet sheet = workbook.GetSheetAt(0);

            var rowsLength = sheet.LastRowNum + 1;
            // This must be hardcoded since we have to read empty cells.
            var columnCount = 8;

            // Skip first row because it's headers
            for (int i = 1; i < rowsLength; i++)
            {
                var row = sheet.GetRow(i);

                var rawProductData = new RawProductData(i + 1);

                for (int j = 0; j < columnCount; j++)
                {
                    // var cell = row.Cells[j];

                    var cell = row.GetCell(j, MissingCellPolicy.RETURN_BLANK_AS_NULL);

                    if (cell == null)
                    {
                        continue;
                    }

                    var cellValue = cell.CellType == CellType.String ? cell.StringCellValue : cell.CellType == CellType.Numeric ? cell.NumericCellValue.ToString() : "InvalidType";

                    _productPropertyMap[j].Invoke(rawProductData, new[] { cellValue });
                }

                tempProductData.Products.Add(rawProductData);
            }
        }
    }
}

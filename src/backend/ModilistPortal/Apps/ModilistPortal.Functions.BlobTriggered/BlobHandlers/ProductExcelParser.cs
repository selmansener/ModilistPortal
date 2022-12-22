using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModilistPortal.Functions.BlobTriggered.Models;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Shared.Configurations;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ModilistPortal.Functions.BlobTriggered.BlobHandlers
{
    internal class ProductExcelParser
    {
        private readonly BlobServiceClient _blobServiceClient;
        private static Type _productDataType = typeof(RawProductData);
        private readonly IDictionary<int, MethodInfo> _productPropertyMap = new Dictionary<int, MethodInfo>
        {
                { 0, _productDataType.GetProperty("Name").GetSetMethod() },
                { 1, _productDataType.GetProperty("SKU").GetSetMethod() },
                { 2, _productDataType.GetProperty("Barcode").GetSetMethod() },
                { 3, _productDataType.GetProperty("Brand").GetSetMethod() },
                { 4, _productDataType.GetProperty("Category").GetSetMethod() },
                { 5, _productDataType.GetProperty("Price").GetSetMethod() },
                { 6, _productDataType.GetProperty("SalesPrice").GetSetMethod() },
                { 7, _productDataType.GetProperty("StockAmount").GetSetMethod() }
        };

        public ProductExcelParser(IBlobClientFactory blobClientFactory, IOptions<StorageConnectionStrings> options)
        {
            _blobServiceClient = blobClientFactory.GetClient(options.Value.AppStorage);
        }

        [FunctionName(nameof(ProductExcelParser))]
        public async Task RunAsync([BlobTrigger("product-excel-uploads/{name}", Connection = "StorageConnectionString")] Stream productExcelFile, string name, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                var tenantIdAsString = name.Split('-').FirstOrDefault();

                if (string.IsNullOrEmpty(tenantIdAsString))
                {
                    throw new Exception("Parsing TenantId from file name failed.");
                }

                if (!int.TryParse(tenantIdAsString, out int tenantId))
                {
                    throw new Exception("Parsing TenantId from file name failed.");
                }

                var fileExtension = name.Split('.').LastOrDefault();

                if (string.IsNullOrEmpty(fileExtension))
                {
                    throw new Exception("Parsing file extension failed");
                }

                var tempProductData = new TempProductData
                {
                    TenantId = tenantId,
                    Products = new List<RawProductData>()
                };

                ReadExcel(productExcelFile, tempProductData, fileExtension);

                var productDataJson = JsonConvert.SerializeObject(tempProductData, Formatting.Indented, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                });

                BlobContainerClient container = _blobServiceClient.GetBlobContainerClient("raw-product-data");
                await container.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.None);

                var blobName = name.Replace(fileExtension, "json");

                BlobClient blobClient = container.GetBlobClient(blobName);
                await blobClient.UploadAsync(BinaryData.FromString(productDataJson), cancellationToken);
                blobClient.SetHttpHeaders(new BlobHttpHeaders
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf8"
                });
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Parsing excel file {name} failed", name);
                throw;
            }
        }

        public void ReadExcel(Stream stream, TempProductData tempProductData, string fileExtension)
        {
            IWorkbook workbook = fileExtension == "xls" ? new HSSFWorkbook(stream) : new XSSFWorkbook(stream);

            // We assume there is one sheet atleast
            ISheet sheet = workbook.GetSheetAt(0);

            var rowsEnumerator = sheet.GetRowEnumerator();

            // start enumeration
            rowsEnumerator.MoveNext();
            // skip first row
            rowsEnumerator.MoveNext();
            do
            {
                var row = (IRow)rowsEnumerator.Current;

                var rawProductData = new RawProductData();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    var cell = row.Cells[i];

                    var cellValue = cell.CellType == CellType.String ? cell.StringCellValue : cell.CellType == CellType.Numeric ? cell.NumericCellValue.ToString() : "InvalidType";

                    _productPropertyMap[i].Invoke(rawProductData, new[] { cellValue });
                }

                tempProductData.Products.Add(rawProductData);

            } while (rowsEnumerator.MoveNext());
        }
    }
}

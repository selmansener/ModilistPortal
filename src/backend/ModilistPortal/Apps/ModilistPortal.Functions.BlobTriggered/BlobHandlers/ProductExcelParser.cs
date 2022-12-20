using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Azure.Storage.Blobs;

using ExcelDataReader;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;

namespace ModilistPortal.Functions.BlobTriggered.BlobHandlers
{
    internal class ProductExcelParser
    {
        private readonly IBlobClientFactory _blobClientFactory;
        private readonly BlobServiceClient _blobServiceClient;

        [FunctionName(nameof(ProductExcelParser))]
        public void Run([BlobTrigger("product-excel-uploads/{name}", Connection = "StorageConnectionString")] Stream productExcelFile, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {productExcelFile.Length} Bytes");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var tenantIdAsString = name.Split('-').FirstOrDefault();

            if (string.IsNullOrEmpty(tenantIdAsString))
            {
                throw new Exception("Parsing TenantId from file name failed.");
            }

            if (!int.TryParse(tenantIdAsString, out int tenantId))
            {
                throw new Exception("Parsing TenantId from file name failed.");
            }

            // TODO: check if tenant exists.


        }

        // TODO: didn't work because of encoding problem. Find another excel reader package.

        //public void ReadExcel(Stream fileStream)
        //{
        //    using (var reader = ExcelReaderFactory.CreateReader(fileStream))
        //    {
        //        // Choose one of either 1 or 2:

        //        // 1. Use the reader methods
        //        do
        //        {
        //            while (reader.Read())
        //            {
        //                // reader.GetDouble(0);
        //            }
        //        } while (reader.NextResult());

        //        // 2. Use the AsDataSet extension method
        //        var result = reader.AsDataSet();

        //        var mapper = GetPropertyIndexMapper();

        //        var rawProductDataList = new List<RawProductData>();

        //        for (int i = 0; i < result.Tables.Count; i++)
        //        {
        //            var columns = result.Tables[i].Columns;
        //            var rows = result.Tables[i].Rows;

        //            for (int j = 1; j < rows.Count; j++)
        //            {
        //                var rawProductData = new RawProductData();

        //                for (int x = 0; x < columns.Count; x++)
        //                {
        //                    var column = columns[x];
        //                    var value = rows[j][column];

        //                    mapper[x].Invoke(rawProductData, new[] { value });
        //                }

        //                rawProductDataList.Add(rawProductData);
        //            }
        //        }
        //        // The result of each spreadsheet is in result.Tables
        //    }
        //}

        public IDictionary<int, MethodInfo> GetPropertyIndexMapper()
        {
            var type = typeof(RawProductData);

            return new Dictionary<int, MethodInfo>
            {
                { 0, typeof(RawProductData).GetProperty("Name").GetSetMethod() },
                { 1, typeof(RawProductData).GetProperty("SKU").GetSetMethod() },
                { 2, typeof(RawProductData).GetProperty("Barcode").GetSetMethod() },
                { 3, typeof(RawProductData).GetProperty("Brand").GetSetMethod() },
                { 4, typeof(RawProductData).GetProperty("Category").GetSetMethod() },
                { 5, typeof(RawProductData).GetProperty("Price").GetSetMethod() },
                { 6, typeof(RawProductData).GetProperty("SalesPrice").GetSetMethod() },
                { 7, typeof(RawProductData).GetProperty("StockAmount").GetSetMethod() }
            };
        }

        internal class RawProductData
        {
            public string Name { get; set; }

            public string SKU { get; set; }

            public string Barcode { get; set; }

            public string Brand { get; set; }

            public string Category { get; set; }

            public string Price { get; set; }

            public string SalesPrice { get; set; }

            public string StockAmount { get; set; }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using ModilistPortal.Functions.BlobTriggered.Models;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.BlobTriggered.BlobHandlers
{
    public class RawProductDataParser
    {
        [FunctionName(nameof(RawProductDataParser))]
        public async Task RunAsync([BlobTrigger("raw-product-data/{name}", Connection = "StorageConnectionString")]Stream stream, string name, ILogger log, CancellationToken cancellationToken)
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

                if (stream == null)
                {
                    throw new Exception("Product data stream is null.");
                }

                string rawDataAsJson = null;
                using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                {
                    rawDataAsJson = await streamReader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(rawDataAsJson))
                {
                    throw new Exception("Reading product data stream failed.");
                }

                TempProductData tempProductData = JsonConvert.DeserializeObject<TempProductData>(rawDataAsJson);

                if (tempProductData == null)
                {
                    throw new Exception("Deserialization of raw product json failed.");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Parsing raw product json file {name} failed", name);

                throw;
            }
        }
    }
}

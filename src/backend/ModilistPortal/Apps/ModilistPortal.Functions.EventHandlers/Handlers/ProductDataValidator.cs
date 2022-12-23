using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using FluentValidation;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    internal class ProductDataValidator
    {
        [FunctionName(nameof(ProductDataValidator))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var rawProductDataParsed = JsonConvert.DeserializeObject<RawProductDataParsed>(eventGridEvent.Data.ToString());

            try
            {

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Validating product data failed. AdditionalData: TenantId: {TenantId}, BlobId: {BlobId}", rawProductDataParsed.TenantId, rawProductDataParsed.BlobId);
                throw;
            }
        }

        internal class ProductValidator : AbstractValidator<RawProductDataParsed>
        {
            public ProductValidator()
            {
                RuleFor(x => x.SKU).NotEmpty();
                RuleFor(x => x.Barcode).NotEmpty();
                RuleFor(x => x.Brand).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Price).NotEmpty()
                    .Must(price => decimal.TryParse(price, out decimal parsedPrice) && parsedPrice > 0).WithErrorCode("InvalidDecimal");
                RuleFor(x => x.SalesPrice).NotEmpty()
                    .Must(salesPrice => decimal.TryParse(salesPrice, out decimal parsedSalesPrice) && parsedSalesPrice > 0).WithErrorCode("InvalidDecimal");
                RuleFor(x => x.StockAmount).NotEmpty()
                    .Must(stockAmount => int.TryParse(stockAmount, out int parsedStockAmount) && parsedStockAmount > 0).WithErrorCode("InvalidInteger");
            }
        }
    }
}

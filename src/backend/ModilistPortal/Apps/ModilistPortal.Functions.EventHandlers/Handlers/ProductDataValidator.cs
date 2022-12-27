using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    public class ProductDataValidator
    {
        private readonly EventGridPublisherClient _eventGridPublisherClient;
        private readonly ProductValidator _validator = new ProductValidator();
        private readonly IMediator _mediator;

        public ProductDataValidator(
            IMediator mediator,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> eventGridOptions)
        {
            _mediator = mediator;
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
        }

        [FunctionName(nameof(ProductDataValidator))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var rawProductDataParsed = JsonConvert.DeserializeObject<RawProductDataParsed>(eventGridEvent.Data.ToString());

            try
            {
                var productExcelRowId = await _mediator.Send(new CreateProductExcelRow
                {
                    ProductExcelUploadId = rawProductDataParsed.ProductExcelUploadId,
                    TenantId = rawProductDataParsed.TenantId,
                    BlobId = rawProductDataParsed.BlobId,
                    Name = rawProductDataParsed.Name,
                    SKU = rawProductDataParsed.SKU,
                    Barcode = rawProductDataParsed.Barcode,
                    Brand = rawProductDataParsed.Brand,
                    Category = rawProductDataParsed.Category,
                    Price = rawProductDataParsed.Price,
                    RowId = rawProductDataParsed.RowId,
                    SalesPrice = rawProductDataParsed.SalesPrice,
                    StockAmount = rawProductDataParsed.StockAmount,
                }, cancellationToken);

                var validationResult = _validator.Validate(rawProductDataParsed);

                if (validationResult.Errors.Any())
                {
                    var errors = GetErrors(validationResult.Errors);

                    await _mediator.Send(new SetRowErrors
                    {
                        ProductExcelRowId = productExcelRowId,
                        TenantId = rawProductDataParsed.TenantId,
                        BlobId = rawProductDataParsed.BlobId,
                        RowId = rawProductDataParsed.RowId,
                        Errors = errors
                    });
                }
                else
                {
                    // TODO: use mapper
                    var productValidationSucceeded = new ProductValidationSucceeded(
                        EventPublishers.EventHandlers,
                        PublisherType.System,
                        productExcelRowId,
                        rawProductDataParsed.TenantId,
                        rawProductDataParsed.BlobId,
                        rawProductDataParsed.RowId,
                        rawProductDataParsed.Name,
                        rawProductDataParsed.SKU,
                        rawProductDataParsed.Barcode,
                        rawProductDataParsed.Brand,
                        rawProductDataParsed.Category,
                        rawProductDataParsed.Price,
                        rawProductDataParsed.SalesPrice,
                        rawProductDataParsed.StockAmount);

                    var productDataValidationSucceededEvent = new EventGridEvent(
                        nameof(ProductValidationSucceeded),
                        nameof(ProductValidationSucceeded),
                        productValidationSucceeded.Version,
                        productValidationSucceeded);

                    await _eventGridPublisherClient.SendEventAsync(productDataValidationSucceededEvent, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Validating product data failed. AdditionalData: TenantId: {TenantId}, BlobId: {BlobId}", rawProductDataParsed.TenantId, rawProductDataParsed.BlobId);
                throw;
            }
        }

        private IDictionary<string, IReadOnlyList<string>> GetErrors(IReadOnlyList<ValidationFailure> validationFailures)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            var groupedErrors = validationFailures.GroupBy(x => x.PropertyName);

            foreach (var groupedError in groupedErrors)
            {
                foreach (var error in groupedError)
                {
                    if (errors.ContainsKey(groupedError.Key))
                    {
                        errors[groupedError.Key].Add(error.ErrorCode);
                    }
                    else
                    {
                        errors.Add(groupedError.Key, new List<string>()
                            {
                                error.ErrorCode
                            });
                    }
                }
            }

            return errors.ToDictionary(x => x.Key, y => (IReadOnlyList<string>)y.Value);
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

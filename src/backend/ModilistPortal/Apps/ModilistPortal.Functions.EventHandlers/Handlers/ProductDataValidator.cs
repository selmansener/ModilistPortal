using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;

using FluentValidation;

using MediatR;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using ModilistPortal.Business.CQRS.ProductDomain.Commands;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Infrastructure.Shared.Events;

using Newtonsoft.Json;

namespace ModilistPortal.Functions.EventHandlers.Handlers
{
    internal class ProductDataValidator
    {
        private readonly ProductValidator _validator = new ProductValidator();
        private readonly IMediator _mediator;

        public ProductDataValidator(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(ProductDataValidator))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log, CancellationToken cancellationToken)
        {
            var rawProductDataParsed = JsonConvert.DeserializeObject<RawProductDataParsed>(eventGridEvent.Data.ToString());

            try
            {
                await _mediator.Send(new UpsertProductExcelRow
                {
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
                    // TODO: this gonna be a huge ass function, so just fire an event here and handle the rest with another handler function
                    await _mediator.Send(new SetRowValidationFailures
                    {
                        TenantId = rawProductDataParsed.TenantId,
                        BlobId = rawProductDataParsed.BlobId,
                        RowId = rawProductDataParsed.RowId,
                        ValidationFailures = validationResult.Errors
                    });
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Validating product data failed. AdditionalData: TenantId: {TenantId}, BlobId: {BlobId}", rawProductDataParsed.TenantId, rawProductDataParsed.BlobId);
                throw;
            }
        }
        
        public async Task CreateProduct(RawProductDataParsed rawProductDataParsed)
        {
            try
            {
                var productId = await _mediator.Send(new CreateProduct
                {
                    TenantId = rawProductDataParsed.TenantId,
                    Name = rawProductDataParsed.Name,
                    SKU = rawProductDataParsed.SKU,
                    Barcode = rawProductDataParsed.Barcode,
                    Brand = rawProductDataParsed.Brand,
                    Category = rawProductDataParsed.Category,
                    Price = decimal.Parse(rawProductDataParsed.Price),
                    SalesPrice = decimal.Parse(rawProductDataParsed.SalesPrice),
                });

            }
            catch (Exception ex) when (ex is ProductAlreadyExistsException alreadyExistsException)
            {

            }
            catch (Exception ex) when (ex is DbUpdateException dbUpdateException)
            {
                if (dbUpdateException?.InnerException is SqlException sqlException && sqlException != null)
                {
                    if (sqlException.Number == 2601)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

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

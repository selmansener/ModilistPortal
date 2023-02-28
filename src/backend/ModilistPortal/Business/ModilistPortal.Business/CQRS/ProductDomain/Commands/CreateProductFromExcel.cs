using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class CreateProductFromExcel : IRequest
    {
        public CreateProductFromExcel(int productExcelUploadId, int tenantId, int rowId, string name, string sKU, string barcode, string brand, string category, string price, string salesPrice, string stockAmount, string gender, string size, string color)
        {
            ProductExcelUploadId = productExcelUploadId;
            TenantId = tenantId;
            RowId = rowId;
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            Brand = brand;
            Category = category;
            Price = price;
            SalesPrice = salesPrice;
            StockAmount = stockAmount;
            Gender = gender;
            Size = size;
            Color = color;
        }

        public int ProductExcelUploadId { get; set; }

        public int TenantId { get; set; }

        public int RowId { get; set; }

        public string Name { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string Price { get; set; }

        public string SalesPrice { get; set; }

        public string StockAmount { get; set; }

        public string Gender { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }
    }

    internal class CreateProductFromExcelValidator : AbstractValidator<CreateProductFromExcel>
    {
        public CreateProductFromExcelValidator()
        {
            RuleFor(x => x.ProductExcelUploadId).NotEmpty();
            RuleFor(x => x.TenantId).NotEmpty();
        }
    }

    internal class CreateProductFromExcelHandler : IRequestHandler<CreateProductFromExcel, Unit>
    {
        private readonly string _barcodeIndexName = "IX_Products_TenantId_Barcode_DeletedAt";
        private readonly string _skuIndexName = "IX_Products_TenantId_SKU_DeletedAt";
        private readonly ProductValidator _validator = new ProductValidator();
        private readonly IProductExcelRowRepository _productExcelRowRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;

        public CreateProductFromExcelHandler(IProductExcelRowRepository productExcelRowRepository, IProductRepository productRepository, IBrandRepository brandRepository)
        {
            _productExcelRowRepository = productExcelRowRepository;
            _productRepository = productRepository;
            _brandRepository = brandRepository;
        }

        // This is a huge ass ugly handler but we have db context and transaction life cycle issues in EventHandlers project. So for now just keep it all here.
        public async Task<Unit> Handle(CreateProductFromExcel request, CancellationToken cancellationToken)
        {
            // We assume there is already a ProductExcelUpload and directly insert row.
            var productExcelRow = new ProductExcelRow(request.ProductExcelUploadId, request.RowId, request.Name, request.SKU, request.Barcode, request.Brand, request.Category, request.Price, request.SalesPrice, request.StockAmount);

            await _productExcelRowRepository.AddAsync(productExcelRow, cancellationToken, true);

            var validationResult = _validator.Validate(request);

            IDictionary<string, IReadOnlyList<string>> errors = new Dictionary<string, IReadOnlyList<string>>();
            if (validationResult.Errors.Any())
            {
                errors = GetErrors(validationResult.Errors);
                productExcelRow.SetErrors(errors);

                await _productExcelRowRepository.UpdateAsync(productExcelRow, cancellationToken);
                return Unit.Value;
            }
            else
            {
                var brand = await _brandRepository.GetByNameAsync(request.Brand, cancellationToken);

                if (brand == null)
                {
                    errors.Add(ProductPropertyName.Brand.ToString(), new List<string> { "NotFound" });
                }

                var doesSKUExists = await _productRepository.DoesExistsWithSKU(request.TenantId, request.SKU, cancellationToken);

                if (doesSKUExists)
                {
                    errors.Add(ProductPropertyName.SKU.ToString(), new List<string> { "AlreadyExists" });
                }

                var doesBarcodeExists = await _productRepository.DoesExistsWithBarcode(request.TenantId, request.Barcode, cancellationToken);

                if (doesBarcodeExists)
                {
                    errors.Add(ProductPropertyName.Barcode.ToString(), new List<string> { "AlreadyExists" });
                }

                if (brand == null || doesSKUExists || doesBarcodeExists)
                {
                    productExcelRow.SetErrors(errors);
                    await _productExcelRowRepository.UpdateAsync(productExcelRow, cancellationToken);

                    return Unit.Value;
                }

                try
                {
                    var product = new Product(
                        request.Name,
                        request.SKU,
                        request.Barcode,
                        brand.Id,
                        request.Category,
                        decimal.Parse(request.Price),
                        decimal.Parse(request.SalesPrice),
                        8,
                        request.TenantId,
                        Enum.Parse<Gender>(request.Gender),
                        request.Size,
                        request.Color);

                    product.SetGroupId(Guid.NewGuid());

                    await _productRepository.AddAsync(product, cancellationToken);
                }
                catch (Exception ex) when (ex is DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException?.InnerException is SqlException sqlException && sqlException != null)
                    {
                        if (sqlException.Number == 2601)
                        {
                            if (sqlException.Message.Contains(_skuIndexName))
                            {
                                errors.Add(ProductPropertyName.SKU.ToString(), new List<string>
                                {
                                    "AlreadyExists"
                                });

                                productExcelRow.SetErrors(errors);
                            }
                            else if (sqlException.Message.Contains(_barcodeIndexName))
                            {
                                errors.Add(ProductPropertyName.Barcode.ToString(), new List<string>
                                {
                                    "AlreadyExists"
                                });

                                productExcelRow.SetErrors(errors);
                            }
                            else
                            {
                                errors.Add(ProductPropertyName.None.ToString(), new List<string>
                                {
                                    sqlException.Message
                                });

                                productExcelRow.SetErrors(errors);
                            }
                        }
                        else
                        {
                            errors.Add(ProductPropertyName.None.ToString(), new List<string>
                            {
                                sqlException.Message
                            });

                            productExcelRow.SetErrors(errors);
                        }
                    }
                    else
                    {
                        errors.Add(ProductPropertyName.None.ToString(), new List<string>
                        {
                            dbUpdateException.Message
                        });

                        productExcelRow.SetErrors(errors);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ProductPropertyName.None.ToString(), new List<string>
                    {
                        ex.Message
                    });

                    productExcelRow.SetErrors(errors);
                }

                if (productExcelRow.ErrorMappings.Any())
                {
                    await _productExcelRowRepository.UpdateAsync(productExcelRow, cancellationToken);
                }
            }

            return Unit.Value;
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


        private class ProductValidator : AbstractValidator<CreateProductFromExcel>
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
                RuleFor(x => x.Gender).NotEmpty()
                    .Must(gender => Enum.TryParse<Gender>(gender, out _)).WithErrorCode("InvalidGender");
                RuleFor(x => x.Size).NotEmpty();
                RuleFor(x => x.Color).NotEmpty();
            }
        }
    }
}

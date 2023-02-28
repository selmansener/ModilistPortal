using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.ProductDomain
{
    public class ProductExcelRow : BaseEntity
    {
        private readonly List<ProductPropertyError> _errorMappings = new List<ProductPropertyError>();

        public ProductExcelRow(int productExcelUploadId, int rowId, string name, string sKU, string barcode, string brand, string category, string gender, string color, string size, string price, string salesPrice, string stockAmount)
        {
            ProductExcelUploadId = productExcelUploadId;
            RowId = rowId;
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            Brand = brand;
            Category = category;
            Gender = gender;
            Color = color;
            Size = size;
            Price = price;
            SalesPrice = salesPrice;
            StockAmount = stockAmount;
            State = ProductExcelRowState.None;
        }

        public int ProductExcelUploadId { get; private set; }

        public ProductExcelUpload ProductExcelUpload { get; private set; }

        public int RowId { get; private set; }

        public string? Name { get; private set; }

        public string? SKU { get; private set; }

        public string? Barcode { get; private set; }

        public string? Brand { get; private set; }

        public string? Category { get; private set; }

        public string Gender { get; private set; }

        public string Color { get; private set; }

        public string Size { get; private set; }

        public string? Price { get; private set; }

        public string? SalesPrice { get; private set; }

        public string? StockAmount { get; private set; }

        public ProductExcelRowState State { get; private set; }

        public IReadOnlyList<ProductPropertyError> ErrorMappings => _errorMappings;

        public void Update(int rowId, string name, string sKU, string barcode, string brand, string category, string gender, string color, string size, string price, string salesPrice, string stockAmount)
        {
            RowId = rowId;
            Name = name;
            SKU = sKU;
            Barcode = barcode;
            Brand = brand;
            Category = category;
            Gender = gender;
            Color = color;
            Size = size;
            Price = price;
            SalesPrice = salesPrice;
            StockAmount = stockAmount;
        }

        public void ClearErrors()
        {
            _errorMappings.Clear();
        }

        public void SetErrors(IDictionary<string, IReadOnlyList<string>> errors)
        {
            State = errors.Any(x => x.Key == "None") ? ProductExcelRowState.InsertionFailed : ProductExcelRowState.ValidationFailed;

            var propertyNames = Enum.GetNames<ProductPropertyName>();

            foreach (var property in propertyNames)
            {
                var errorMapping = _errorMappings.FirstOrDefault(x => x.PropertyName.ToString() == property);
                // if we don't have any error for that property
                if (errorMapping == null)
                {
                    // then we check if there's new one
                    if (!errors.ContainsKey(property))
                    {
                        // if there isn't, then continue
                        continue;
                    }
                    else
                    {
                        // create new error mapping and add errors
                        errorMapping = new ProductPropertyError(Id, (ProductPropertyName)Enum.Parse(typeof(ProductPropertyName), property));

                        foreach (var error in errors[property])
                        {
                            errorMapping.AddError(error);
                        }
                    }

                    _errorMappings.Add(errorMapping);
                }
                else
                {
                    // if we have errors for that property previously, we gonna override all of it, so clear the errors
                    errorMapping.ClearErrors();

                    if (errors.ContainsKey(property))
                    {
                        foreach (var error in errors[property])
                        {
                            errorMapping.AddError(error);
                        }
                    }
                }
            }
        }
    }
}

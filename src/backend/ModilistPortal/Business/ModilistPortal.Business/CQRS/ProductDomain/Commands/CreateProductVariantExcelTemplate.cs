using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.EntityFrameworkCore;

using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Data.Repositories.TenantDomain;
using ModilistPortal.Domains.Models.ReturnDomain;

//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;

using static System.Net.Mime.MediaTypeNames;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class CreateProductVariantExcelTemplate : IRequest
    {
        public CreateProductVariantExcelTemplate(Guid accountId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; set; }

    }

    internal class CreateProductVariantExcelTemplateValidator : AbstractValidator<CreateProductVariantExcelTemplate>
    {
        public CreateProductVariantExcelTemplateValidator()
        {
            RuleFor(x =>x.AccountId).NotEmpty();
        }
    }

    internal class CreateProductVariantExcelTemplateHandler : IRequestHandler<CreateProductVariantExcelTemplate>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IProductRepository _productRepository;

        public CreateProductVariantExcelTemplateHandler(IAccountRepository accountRepository, ITenantRepository tenantRepository, IProductRepository productRepository)
        {
            _accountRepository= accountRepository;
            _tenantRepository= tenantRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(CreateProductVariantExcelTemplate request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if(account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            var tenant =  _tenantRepository.GetByAccountId(request.AccountId, cancellationToken);

            if(tenant == null)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            var result = await _productRepository
               .GetAllAsNoTracking()
               .Where(x => x.TenantId == account.TenantId)
               .GroupBy(x => x.GroupId)
               .Select(x => x.Select(x => x.CreatedAt).Min())
               .ProjectToType<QueryProductDTO>()
            .ToListAsync(cancellationToken);

            //IWorkbook workbook = new XSSFWorkbook();
            //XSSFFont myFont = (XSSFFont)workbook.CreateFont();

            //myFont.FontHeightInPoints = 11;
            //myFont.FontName = "Tahoma";

            //XSSFCellStyle borderedCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            //borderedCellStyle.SetFont(myFont);
            //borderedCellStyle.BorderLeft = BorderStyle.Medium;
            //borderedCellStyle.BorderTop = BorderStyle.Medium;
            //borderedCellStyle.BorderRight = BorderStyle.Medium;
            //borderedCellStyle.BorderBottom = BorderStyle.Medium;
            //borderedCellStyle.VerticalAlignment = VerticalAlignment.Center;

            //ISheet sheet1 = workbook.CreateSheet("Products");

            //IRow headerRow = sheet1.CreateRow(0);

            var columnNames = new List<string>
            {
                "ProductId",
                "Name",
                "SKU",
                "Barcode",
                "Brand",
                "Category",
                "Size",
                "Color",
                "Price",
                "SalesPrice",
                "StockAmount"
            };

            //for(int i = 0; i < 11; i++)
            //{
            //    CreateCell(headerRow, i, columnNames.ElementAt(i), borderedCellStyle);
            //}

            //for(int i = 0; i <= result.Count; i++)
            //{
            //    IRow row = sheet1.CreateRow(i + 1);

            //    var currentProduct = result.ElementAt(i);
            //    var productProps = currentProduct.GetType().GetProperties();

            //    for (int j = 0; j < productProps.Length - 2; j++)
            //    {
            //        if(j == 0)
            //        {
            //            CreateCell(row, j, row.RowNum.ToString(), borderedCellStyle, isProductId: true); //ProductId
            //        }

            //        //CreateCell(row, j, productProps.GetV,borderedCellStyle);
            //    }
            //}

            //using (var package = new ExcelPackage())
            //{
            //    // Add a new worksheet to the workbook
            //    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            //    // Add the result to the worksheet
            //    for (int i = 0; i < result.Count; i++)
            //    {
            //        worksheet.Cells[i + 1, 1].Value = result[i];
            //    }

            //    // Save the Excel package to a stream
            //    var stream = new MemoryStream();
            //    package.SaveAs(stream);

            //    return Unit.Value;
            //}

            return Unit.Value;
        }
        //private void CreateCell(IRow CurrentRow, int CellIndex, string Value, XSSFCellStyle Style, bool isProductId = false)
        //{
        //    ICell Cell = CurrentRow.CreateCell(CellIndex);

        //    int productId = 0;

        //    if(isProductId)
        //    {
        //        Cell.SetCellValue(Int32.Parse(Value));
        //    }

        //    Cell.SetCellValue(Value);
        //    Cell.CellStyle = Style;
        //}
    }
}

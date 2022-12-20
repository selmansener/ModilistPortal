using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Data.Repositories.TenantDomain;
using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Shared.Configurations;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class UploadProductExcel : IRequest
    {
        public Guid AccountId { get; set; }

        public IFormFile File { get; set; }
    }

    internal class UploadProductExcelValidator : AbstractValidator<UploadProductExcel>
    {
        private readonly IList<string> _acceptedContentTypes = new List<string>
        {
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

        public UploadProductExcelValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.File).NotEmpty().Must(x => _acceptedContentTypes.Contains(x.ContentType)).WithMessage("InvalidFileType");
        }
    }

    internal class UploadProductExcelHandler : IRequestHandler<UploadProductExcel, Unit>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IBlobClientFactory _blobClientFactory;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;

        public UploadProductExcelHandler(ITenantRepository tenantRepository, IBlobClientFactory blobClientFactory, IOptions<StorageConnectionStrings> options, IProductExcelUploadRepository productExcelUploadRepository)
        {
            _tenantRepository = tenantRepository;
            _blobClientFactory = blobClientFactory;
            _blobServiceClient = _blobClientFactory.GetClient(options.Value.AppStorage);
            _productExcelUploadRepository = productExcelUploadRepository;
        }

        public async Task<Unit> Handle(UploadProductExcel request, CancellationToken cancellationToken)
        {
            Tenant? tenant = await _tenantRepository.GetByAccountId(request.AccountId, cancellationToken);

            if (tenant == null)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient("product-excel-uploads");
            await container.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.None);

            var lastDotIndex = request.File.FileName.LastIndexOf('.');
            var fileName = request.File.FileName.Substring(0, lastDotIndex);
            var extension = request.File.FileName.Substring(lastDotIndex + 1, request.File.FileName.Length - lastDotIndex - 1);
            var blobId = Guid.NewGuid();
            var blobName = $"{tenant.Id}-{blobId}.{extension}";

            BlobClient blobClient = container.GetBlobClient(blobName);

            await blobClient.UploadAsync(request.File.OpenReadStream(), cancellationToken);

            var productExcelUpload = new ProductExcelUpload(tenant.Id, blobId, fileName, extension, blobClient.Uri.AbsoluteUri, request.File.ContentType, (int)request.File.Length / 1024 * 1024);
            
            await _productExcelUploadRepository.AddAsync(productExcelUpload, cancellationToken);

            return Unit.Value;
        }
    }
}

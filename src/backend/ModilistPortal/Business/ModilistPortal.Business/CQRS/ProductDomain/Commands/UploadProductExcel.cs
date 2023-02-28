using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Messaging.EventGrid;
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
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Configurations;
using ModilistPortal.Infrastructure.Shared.Constants;
using ModilistPortal.Infrastructure.Shared.Events;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class UploadProductExcel : IRequest
    {
        public Guid AccountId { get; set; }

        public IFormFile File { get; set; }

        public int? ProductId { get; set; }

        public bool IsVariantExcel { get; set; } = false;
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
            RuleFor(x => x.File).NotEmpty().Must(x => _acceptedContentTypes.Contains(x.ContentType)).When(x => x.File != null, ApplyConditionTo.CurrentValidator).WithErrorCode("InvalidFileType");
        }
    }

    internal class UploadProductExcelHandler : IRequestHandler<UploadProductExcel, Unit>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IProductExcelUploadRepository _productExcelUploadRepository;
        private readonly EventGridPublisherClient _eventGridPublisherClient;
        private readonly IProductRepository _productRepository;

        public UploadProductExcelHandler(
            ITenantRepository tenantRepository,
            IBlobClientFactory blobClientFactory,
            IOptions<StorageConnectionStrings> options,
            IProductExcelUploadRepository productExcelUploadRepository,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> eventGridOptions,
            IProductRepository productRepository)
        {
            _tenantRepository = tenantRepository;
            _blobServiceClient = blobClientFactory.GetClient(options.Value.AppStorage);
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
            _productExcelUploadRepository = productExcelUploadRepository;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(UploadProductExcel request, CancellationToken cancellationToken)
        {
            Tenant? tenant = await _tenantRepository.GetByAccountId(request.AccountId, cancellationToken);

            if (tenant == null)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            Guid? groupId = null;

            if(request.ProductId != 0)
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId ?? default(int), cancellationToken);

                if (product == null)
                {
                    throw new ProductNotFoundException(request.ProductId ?? default(int), tenant.Id);
                }

                groupId = product.GroupId;
            }

            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(StorageContainerNames.PRODUCT_EXCEL_UPLOADS);
            await container.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.None);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
            var lastDotIndex = request.File.FileName.LastIndexOf('.');
            var fileName = request.File.FileName.Substring(0, lastDotIndex);
            var extension = request.File.FileName.Substring(lastDotIndex + 1, request.File.FileName.Length - lastDotIndex - 1);
            var blobId = Guid.NewGuid();
            var blobName = $"{blobId}.{extension}";
            var blobFullPath = $"{tenant.Id}/{timestamp}/{blobName}";

            BlobClient blobClient = container.GetBlobClient(blobFullPath);

            await blobClient.UploadAsync(request.File.OpenReadStream(), cancellationToken);

            var productExcelUpload = new ProductExcelUpload(tenant.Id, blobId, fileName, extension, blobClient.Uri.AbsoluteUri, request.File.ContentType, request.File.Length, request.IsVariantExcel);
            
            await _productExcelUploadRepository.AddAsync(productExcelUpload, cancellationToken);

            var productExcelUploaded = new ProductExcelUploaded(
                request.AccountId.ToString(),
                PublisherType.Account,
                productExcelUpload.Id,
                tenant.Id,
                blobId,
                StorageContainerNames.PRODUCT_EXCEL_UPLOADS,
                blobName,
                blobFullPath,
                extension,
                timestamp,
                groupId);

            var productExcelUploadedEvent = new EventGridEvent(
                nameof(ProductExcelUploaded),
                nameof(ProductExcelUploaded),
                productExcelUploaded.Version,
                productExcelUploaded);

            await _eventGridPublisherClient.SendEventAsync(productExcelUploadedEvent, cancellationToken);

            return Unit.Value;
        }
    }
}

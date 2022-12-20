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
using ModilistPortal.Data.Repositories.TenantDomain;
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
        public UploadProductExcelValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.File).NotEmpty();
        }
    }

    internal class UploadProductExcelHandler : IRequestHandler<UploadProductExcel, Unit>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IBlobClientFactory _blobClientFactory;
        private readonly BlobServiceClient _blobServiceClient;

        public UploadProductExcelHandler(ITenantRepository tenantRepository, IBlobClientFactory blobClientFactory, IOptions<StorageConnectionStrings> options)
        {
            _tenantRepository = tenantRepository;
            _blobClientFactory = blobClientFactory;
            _blobServiceClient = _blobClientFactory.GetClient(options.Value.AppStorage);
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



            return Unit.Value;
        }
    }
}


using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using FluentValidation;

using Mapster;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using ModilistPortal.Business.CQRS.ProductDomain.DTOs;
using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Domains.Models.AccountDomain;
using ModilistPortal.Domains.Models.ProductDomain;
using ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions.EventGrid;
using ModilistPortal.Infrastructure.Shared.Configurations;
using ModilistPortal.Infrastructure.Shared.Events;

namespace ModilistPortal.Business.CQRS.ProductDomain.Commands
{
    public class AddProductImages : IRequest<IEnumerable<ProductImageDTO>>
    {
        public Guid AccountId { get; set; }

        public int ProductId { get; set; }

        public IEnumerable<IFormFile> Files { get; set; }
    }

    internal class AddProductImagesValidator : AbstractValidator<AddProductImages>
    {
        public AddProductImagesValidator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Files).NotEmpty();
        }
    }

    internal class AddProductImagesHandler : IRequestHandler<AddProductImages, IEnumerable<ProductImageDTO>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IBlobClientFactory _blobClientFactory;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly EventGridPublisherClient _eventGridPublisherClient;

        public AddProductImagesHandler(
            IProductRepository productRepository,
            IBlobClientFactory blobClientFactory,
            IOptions<StorageConnectionStrings> options,
            IAccountRepository accountRepository,
            IEventGridPublisherClientFactory eventGridPublisherClientFactory,
            IOptions<EventGridClientOptions> eventGridOptions)
        {
            _productRepository = productRepository;
            _blobClientFactory = blobClientFactory;
            _blobServiceClient = _blobClientFactory.GetClient(options.Value.AppStorage);
            _accountRepository = accountRepository;
            _eventGridPublisherClient = eventGridPublisherClientFactory.GetClient(eventGridOptions.Value);
        }

        public async Task<IEnumerable<ProductImageDTO>> Handle(AddProductImages request, CancellationToken cancellationToken)
        {
            Account? account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

            if (account == null)
            {
                throw new AccountNotFoundException(request.AccountId);
            }

            if (!account.TenantId.HasValue)
            {
                throw new TenantNotFoundException(request.AccountId);
            }

            Product? product = await _productRepository.GetByIdAsync(request.ProductId, account.TenantId.Value, cancellationToken);

            if (product == null)
            {
                throw new ProductNotFoundException(request.ProductId, account.TenantId.Value);
            }

            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient("products");
            await container.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            foreach (var file in request.Files)
            {
                var extension = file.FileName.Split('.').Last();
                
                var fileName = Guid.NewGuid().ToString();
                var fullFileName = $"{fileName}.{extension}";

                BlobClient blobClient = container.GetBlobClient(fullFileName);
                
                await blobClient.UploadAsync(file.OpenReadStream(), new BlobUploadOptions 
                { 
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType,
                    }
                }, cancellationToken);

                product.AddImage(fullFileName, file.ContentType, blobClient.Uri.AbsoluteUri, extension);
            }

            await _productRepository.UpdateAsync(product, cancellationToken, true);

            var imageUrls = product.Images.Select(x => x.Url);

            ProductImagesChanged productImagesChanged = new ProductImagesChanged(EventPublishers.WebAPI, PublisherType.Account, product.Id, imageUrls);

            var productImagesChangedEvent = new EventGridEvent(
                nameof(ProductImagesChanged),
                nameof(ProductImagesChanged),
                productImagesChanged.Version,
                productImagesChanged);

            await _eventGridPublisherClient.SendEventAsync(productImagesChangedEvent, cancellationToken);

            return product.Images.Select(x => x.Adapt<ProductImageDTO>());
        }
    }
}

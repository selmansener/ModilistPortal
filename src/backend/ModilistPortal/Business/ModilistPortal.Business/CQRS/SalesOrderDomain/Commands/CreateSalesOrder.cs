
using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using ModilistPortal.Business.Exceptions;
using ModilistPortal.Data.Repositories.InventoryDomain;
using ModilistPortal.Data.Repositories.SalesOrderDomain;
using ModilistPortal.Data.Repositories.TenantDomain;
using ModilistPortal.Domains.Models.SalesOrderDomain;
using ModilistPortal.Infrastructure.Shared.DTOs;

namespace ModilistPortal.Business.CQRS.SalesOrderDomain.Commands
{
    public class CreateSalesOrder : IRequest
    {
        public CreateSalesOrder(int tenantId, int marketplaceSalesOrderId, DateTime marketplaceOrderCreatedAt, DeliveryAddressDTO deliveryAddress, BillingAddressDTO billingAddress, IEnumerable<SalesOrderLineItemDTO> lineItems)
        {
            TenantId = tenantId;
            MarketplaceSalesOrderId = marketplaceSalesOrderId;
            MarketplaceOrderCreatedAt = marketplaceOrderCreatedAt;
            DeliveryAddress = deliveryAddress;
            BillingAddress = billingAddress;
            LineItems = lineItems;
        }

        public int TenantId { get; private set; }

        public int MarketplaceSalesOrderId { get; private set; }

        public DateTime MarketplaceOrderCreatedAt { get; private set; }

        public DeliveryAddressDTO DeliveryAddress { get; private set; }

        public BillingAddressDTO BillingAddress { get; private set; }

        public IEnumerable<SalesOrderLineItemDTO> LineItems { get; private set; }
    }

    internal class CreateSalesOrderValidator : AbstractValidator<CreateSalesOrder>
    {
        public CreateSalesOrderValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.MarketplaceSalesOrderId).NotEmpty();
            RuleFor(x => x.MarketplaceOrderCreatedAt).NotEmpty();
            RuleFor(x => x.DeliveryAddress).NotEmpty();
            RuleFor(x => x.BillingAddress).NotEmpty();
            RuleFor(x => x.LineItems).NotEmpty();
        }
    }

    internal class CreateSalesOrderHandler : IRequestHandler<CreateSalesOrder, Unit>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<CreateSalesOrderHandler> _logger;

        public CreateSalesOrderHandler(
            ITenantRepository tenantRepository,
            ISalesOrderRepository salesOrderRepository,
            IInventoryRepository inventoryRepository,
            ILogger<CreateSalesOrderHandler> logger)
        {
            _tenantRepository = tenantRepository;
            _salesOrderRepository = salesOrderRepository;
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateSalesOrder request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);

            if (tenant == null)
            {
                throw new TenantNotFoundException(request.TenantId);
            }

            var doesExists = await _salesOrderRepository.DoesExistsByMarketplaceIdAsync(request.MarketplaceSalesOrderId, request.TenantId, cancellationToken);

            if (doesExists)
            {
                // If order already created then just ignore it, but investigate why we got to this point.
                _logger.LogError($"SalesOrder with MarketplaceSalesOrderId: {request.MarketplaceSalesOrderId} already created for Tenant with TenantId: {request.TenantId}");

                return Unit.Value;
            }

            var salesOrder = new SalesOrder(request.TenantId, request.MarketplaceSalesOrderId, request.MarketplaceOrderCreatedAt);

            var inventoryItems = await _inventoryRepository.GetAllByProductIdsAsync(request.TenantId, request.LineItems.Select(x => x.ProductId), cancellationToken);
            foreach (var lineItem in salesOrder.LineItems)
            {
                salesOrder.AddLineItem(lineItem.ProductId, lineItem.Amount, lineItem.Price, lineItem.SalesPrice);

                var inventoryItem = inventoryItems.FirstOrDefault(x => x.ProductId == lineItem.ProductId);

                if (inventoryItem == null)
                {
                    _logger.LogCritical($"InventoryItem not found with ProductId: {lineItem.ProductId} and TenantId: {request.TenantId}");
                    // Don't break the loop but investigate why there isn't any InventoryItem for that product.
                    continue;
                }

                var missingAmount = inventoryItem.DecreaseAmount(lineItem.Amount);

                if (missingAmount > 0)
                {
                    // TODO: Throw by tenant configuration. We can implement tenant specific configurations, like don't create order if inventory is out of stock.
                }
            }

            await _salesOrderRepository.AddAsync(salesOrder, cancellationToken, saveChanges: true);

            var deliverAddress = new DeliveryAddress(
                salesOrder.Id,
                request.DeliveryAddress.FullName,
                request.DeliveryAddress.Phone,
                request.DeliveryAddress.Email,
                request.DeliveryAddress.Details.City,
                request.DeliveryAddress.Details.Disctrict,
                request.DeliveryAddress.Details.ZipCode,
                request.DeliveryAddress.Details.FullAddress);

            var billingAddress = new BillingAddress(
                salesOrder.Id,
                request.BillingAddress.Type,
                request.BillingAddress.FullName,
                request.BillingAddress.Phone,
                request.BillingAddress.Email,
                request.BillingAddress.TCKN,
                request.BillingAddress.TaxNumber,
                request.BillingAddress.TaxOffice,
                request.BillingAddress.Details.City,
                request.BillingAddress.Details.Disctrict,
                request.BillingAddress.Details.ZipCode,
                request.BillingAddress.FullName);

            salesOrder.AssignDeliveryAddress(deliverAddress);
            salesOrder.AssignBillingAddress(billingAddress);

            await _salesOrderRepository.UpdateAsync(salesOrder, cancellationToken);

            return Unit.Value;
        }
    }
}

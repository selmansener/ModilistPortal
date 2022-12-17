﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.ShipmentDomain;
using ModilistPortal.Domains.Models.TenantDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.SalesOrderDomain
{
    public class SalesOrder : BaseEntity
    {
        private readonly List<SalesOrderLineItem> _lineItems = new List<SalesOrderLineItem>();

        public int TenantId { get; private set; }

        public Tenant Tenant { get; private set; }

        public SalesOrderState State { get; private set; }

        public DeliveryAddress? DeliveryAddress { get; private set; }

        public BillingAddress? BillingAddress { get; private set; }

        public Shipment Shipment { get; private set; }

        public string? InvoiceUrl { get; private set; }

        public IReadOnlyList<SalesOrderLineItem> LineItems => _lineItems;
    }
}
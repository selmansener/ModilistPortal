
using ModilistPortal.Domains.Base;
using ModilistPortal.Infrastructure.Shared.Models;

namespace ModilistPortal.Domains.Models.SalesOrderDomain
{
    public class DeliveryAddress : BaseEntity
    {
        public DeliveryAddress(int salesOrderId, string fullName, string phone, string? email, string city, string district, string zipCode, string fullAddress)
        {
            SalesOrderId = salesOrderId;
            FullName = fullName;
            Phone = phone;
            Email = email;
            Address = new Address(city, district, "Turkey", zipCode, fullAddress);
        }

        public int SalesOrderId { get; private set; }

        public SalesOrder SalesOrder { get; private set; }

        public string FullName { get; private set; }

        public string Phone { get; private set; }

        public string? Email { get; private set; }

        public Address Address { get; private set; }
    }
}

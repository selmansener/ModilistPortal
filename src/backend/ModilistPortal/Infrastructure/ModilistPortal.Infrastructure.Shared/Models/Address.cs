namespace ModilistPortal.Infrastructure.Shared.Models
{
    public class Address : ValueObject<Address>
    {
        public Address(string city, string disctrict, string country, string zipCode, string fullAddress)
        {
            City = city;
            Disctrict = disctrict;
            Country = country;
            ZipCode = zipCode;
            FullAddress = fullAddress;
        }

        public string City { get; private set; }

        public string Disctrict { get; private set; }

        public string Country { get; private set; }

        public string ZipCode { get; private set; }

        public string FullAddress { get; private set; }
    }
}

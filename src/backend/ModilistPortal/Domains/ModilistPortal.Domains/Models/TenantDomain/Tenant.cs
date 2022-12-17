
using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.AccountDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.TenantDomain
{
    public class Tenant : BaseEntity
    {
        private readonly List<Account> _accounts = new List<Account>();

        protected Tenant() { }

        public Tenant(string name, string? tckn, string? taxNumber, string taxOffice, string phone, string email, string city, string district, TenantType type)
        {
            Name = name;
            TCKN = tckn;
            TaxNumber = taxNumber;
            TaxOffice = taxOffice;
            Phone = phone;
            Email = email;
            City = city;
            District = district;
            Type = type;
            IsVerified = false;
        }

        public string Name { get; private set; }

        public string? TCKN { get; private set; }

        public string? TaxNumber { get; private set; }

        public string TaxOffice { get; private set; }

        public string Phone { get; private set; }

        public string Email { get; private set; }

        public string City { get; private set; }

        public string District { get; private set; }

        public TenantType Type { get; private set; }

        public bool IsVerified { get; private set; }

        public IReadOnlyList<Account> Accounts => _accounts;

        public void Update(string name, string? tckn, string? taxNumber, string taxOffice, string phone, string email, string city, string district, TenantType type)
        {
            Name = name;
            TCKN = tckn;
            TaxNumber = taxNumber;
            TaxOffice = taxOffice;
            Phone = phone;
            Email = email;
            City = city;
            District = district;
            Type = type;
            IsVerified = false;
        }
    }
}

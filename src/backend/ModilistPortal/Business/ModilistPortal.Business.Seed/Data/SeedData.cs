using System.Collections.Immutable;

using ModilistPortal.Domains.Models.AccountDomain;

namespace ModilistPortal.Business.Seed.Data
{
    internal class SeedData
    {
        public ImmutableList<Account> Accounts { get; private set; }
    }
}

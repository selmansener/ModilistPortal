
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ModilistPortal.Domains.Models.AccountDomain;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Data.EntityConfigurations.AccountDomain
{
    internal class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.State).HasDefaultValue(AccountState.None).IsRequired().HasConversion<string>();
        }
    }
}

using ModilistPortal.Domains.Base;
using ModilistPortal.Infrastructure.Shared.Enums;

namespace ModilistPortal.Domains.Models.AccountDomain
{
    public class Account : BaseEntity
    {
        public Account(Guid id)
        {
            Id = id;
        }

        public new Guid Id { get; private set; }

        public string? Email { get; private set; }

        public AccountState State { get; private set; }

        public DateTime? ActivatedAt { get; private set; }

        public DateTime? DeactivatedAt { get; private set; }

        public DateTime? BlockedAt { get; private set; }

        public DateTime? VerifiedAt { get; private set; }

        public bool IsVerified { get; private set; }

        public void Activate()
        {
            if (State == AccountState.Active)
            {
                throw new InvalidOperationException("Account is already Active");
            }

            State = AccountState.Active;
            ActivatedAt = DateTime.UtcNow;
        }

        public void Verify()
        {
            IsVerified = true;
            VerifiedAt = DateTime.UtcNow;
        }
    }
}

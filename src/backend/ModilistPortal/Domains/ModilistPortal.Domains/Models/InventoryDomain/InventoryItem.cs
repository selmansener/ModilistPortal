
using ModilistPortal.Domains.Base;
using ModilistPortal.Domains.Models.ProductDomain;

namespace ModilistPortal.Domains.Models.InventoryDomain
{
    public class InventoryItem : BaseEntity
    {
        public int ProductId { get; private set; }

        public Product Product { get; private set; }

        public int Amount { get; private set; }

        /// <summary>
        /// Decreases available stock amount and returns the missing amount if it's negative.
        /// </summary>
        /// <param name="amount">Amount to decrease</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws if the amount parameter equal or less than zero.</exception>
        public int DecreaseAmount(int amount)
        {
            if (amount == default || amount < 0)
            {
                throw new ArgumentException($"amount must be greater than zero.", nameof(amount));
            }

            var remainingAmount = Amount - amount;

            if (remainingAmount > 0)
            {
                Amount = remainingAmount;
            }
            else
            {
                Amount = 0;
            }

            return remainingAmount < 0 ? Math.Abs(remainingAmount) : 0;
        }
    }
}

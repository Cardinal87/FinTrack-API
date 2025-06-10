
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Entities
{   
    /// <summary>
    /// presents User's account
    /// </summary>
    public class Account : Entity
    {
        private Account()
        {

        }
        
        public Account(Guid userId)
        {
            UserId = userId;
        }

        public decimal Balance { get; private set; } = 0;
        public Guid UserId { get; private set; }
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

        /// <summary>
        /// debit money from balance
        /// </summary>
        /// <param name="amount">amount that will be debited</param>
        /// <exception cref="InvalidOperationException">amount param is greater than balance</exception>
        public void Debit(decimal amount)
        {
            if (Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }
            Balance -= amount;
        }


        /// <summary>
        /// top up the balance
        /// </summary>
        /// <param name="amount">amount that will be credited to the balance</param>
        public void TopUp(decimal amount)
        {
            Balance += amount;
        }
    }
}


using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Entities
{

    /// <summary>
    /// presents p2p transaction
    /// </summary>
    public class Transaction : Entity
    {
        private Transaction()
        {

        }
        
        public Transaction(decimal amount, Guid fromAccountId, Guid toAccountId)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount is negative or zero");
            }
            if (fromAccountId == toAccountId)
            {
                throw new ArgumentException("The destination account is the same as the source account");
            }
            
            Amount = amount;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Date = DateTime.UtcNow;
        }

        public decimal Amount { get; }
        public Guid FromAccountId { get;}
        public Guid ToAccountId { get;}
        public DateTime Date { get; }

    }
}

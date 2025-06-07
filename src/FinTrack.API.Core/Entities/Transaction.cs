
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Entities
{

    /// <summary>
    /// presents p2p transaction
    /// </summary>
    public class Transaction : Entity
    {
        public Transaction(decimal amount, Guid fromAccountId, Guid toAccountId, DateTime date)
        {
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


using FinTrack.API.Core.Common;
using FinTrack.API.Core.Exceptions;

namespace FinTrack.API.Core.Entities
{

    /// <summary>
    /// presents p2p transaction
    /// </summary>
    public class Transaction : Entity
    {
        public Transaction(decimal amount, Guid fromAccountId, Guid toAccountId, DateTime time)
        {
            if (amount <= 0)
            {
                throw new InvalidTransactionException("Amount is negative or zero");
            }
            if (fromAccountId == toAccountId)
            {
                throw new InvalidTransactionException("The destination account ID is the same as the source account ID");
            }
            if (time > DateTime.UtcNow)
            {
                throw new InvalidTransactionException("Incorrect date of transaction");
            }

            Amount = amount;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Date = time;
        }

        public decimal Amount { get; }
        public Guid FromAccountId { get;}
        public Guid ToAccountId { get;}
        public DateTime Date { get; }

    }
}

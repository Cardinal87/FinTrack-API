
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Exceptions;

namespace FinTrack.API.Core.Entities
{

    /// <summary>
    /// Represents peer-to-peer financial transaction between two accounts.
    /// </summary>
    /// <remarks>
    /// Responsibilities:
    /// <list type="bullet">
    /// <item>Stores all transaction info</item>
    /// <item>Emmutable</item>
    /// </list>
    /// transaction must be immutable
    /// </remarks>
    public class Transaction : Entity
    {
        /// <summary>
        /// Creates immutable transaction
        /// </summary>
        /// <param name="amount">amount of the transaction</param>
        /// <param name="fromAccountId">source account id</param>
        /// <param name="toAccountId">destination account id</param>
        /// <param name="date">date of the transaction</param>
        /// <exception cref="InvalidTransactionException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="date"/> is later than <see cref="DateTime.UtcNow"/></item>
        /// <item><paramref name="fromAccountId"/>is equal to <paramref name="toAccountId"/></item>
        /// </list>
        /// </exception>
        /// <exception cref="IncorrectAmountException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="amount"/>is negative or zero</item>
        /// </list>
        /// </exception> 

        public Transaction(decimal amount, Guid fromAccountId, Guid toAccountId, DateTime date)
        {
            if (amount <= 0)
            {
                throw new IncorrectAmountException(amount, "Amount is negative or zero");
            }
            if (fromAccountId == toAccountId)
            {
                throw new InvalidTransactionException("The destination account ID is the same as the source account ID");
            }
            if (date > DateTime.UtcNow)
            {
                throw new InvalidTransactionException("Incorrect date of transaction");
            }
            Amount = amount;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Date = date;
        }

        /// <summary>
        /// Amount of the transaction
        /// </summary>
        /// <remarks>
        /// Must be positive and must not be equal to zero 
        /// </remarks>
        public decimal Amount { get; }

        /// <summary>
        /// Source account ID
        /// </summary>
        public Guid FromAccountId { get;}

        /// <summary>
        /// Destination account ID
        /// </summary>
        public Guid ToAccountId { get;}

        /// <summary>
        /// Transaction timestamp in UTC
        /// </summary>
        /// <remarks>
        /// Must be less or equal to <see cref="DateTime.UtcNow"/>
        /// </remarks>
        public DateTime Date { get; }

    }
}

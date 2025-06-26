using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for attempt to assign an incorrect transaction
    /// </summary>
    /// <remarks>
    /// Cases:
    /// <list type="bullet">
    /// <item>Outgoing: Transaction.FromAccountId != Account.Id</item>
    /// <item>Incoming: Transaction.ToAccountId != Account.Id</item>
    /// </list>
    /// </remarks>
    public class TransactionOwnershipException : DomainException
    {
        /// <summary>
        /// Id of destination account
        /// </summary>
        public Guid AccountId { get; }


        /// <summary>
        /// Transaction's reference by id to destination or source account
        /// </summary>
        public Guid TransactionRefId { get; }
        
        public TransactionOwnershipException(string message,
                                             Guid accountId,
                                             Guid transactionRefId) : base(message)
        {
            AccountId = accountId;
            TransactionRefId = transactionRefId;
        }
    }
}

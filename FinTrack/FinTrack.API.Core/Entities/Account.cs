
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Exceptions;
namespace FinTrack.API.Core.Entities
{   
    /// <summary>
    /// presents User's account
    /// </summary>
    public class Account : Entity
    {
        private readonly List<Transaction> _outgoingTransactions = new();
        private readonly List<Transaction> _incomingTransactions = new();
        
        private Account()
        {

        }
        
        internal Account(Guid userId)
        {
            UserId = userId;
        }

        public decimal Balance { get; private set; } = 0;
        public Guid UserId { get; private set; }
        public IReadOnlyCollection<Transaction> OutgoingTransactions => _outgoingTransactions.AsReadOnly();
        public IReadOnlyCollection<Transaction> IncomingTransactions => _incomingTransactions.AsReadOnly();

        /// <summary>
        /// debit money from balance
        /// </summary>
        /// <param name="amount">amount that will be debited</param>
        /// <exception cref="InsufficientFundsException">amount param is greater than balance</exception>
        /// <exception cref="ArgumentException">amount if negative or equal to zero</exception>
        public void Debit(decimal amount)
        {
            if (Balance < amount)
            {  
                throw new InsufficientFundsException(Balance, amount);
            }
            if (amount <= 0)
            {
                throw new ArgumentException("amount must be positive");
            }
            Balance -= amount;
        }


        /// <summary>
        /// top up the balance
        /// </summary>
        /// <param name="amount">amount that will be credited to the balance</param>
        /// <exception cref="ArgumentException">amount if negative or equal to zero</exception>
        public void TopUp(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("amount must be positive");
            }
            Balance += amount;
        }


        /// <summary>
        /// Add transaction to list of outgoing transactions
        /// </summary>
        /// <param name="transaction">
        /// <see cref="Transaction"/> entity
        /// </param>
        /// <exception cref="InvalidTransactionException">
        /// Transaction source account id and this account id does not match
        /// </exception>
        internal void AddOutgoingTransaction(Transaction transaction)
        {
            if (transaction.FromAccountId != Id)
            {
                throw new InvalidTransactionException("The transaction does not originate from this account");
            }
            _outgoingTransactions.Add(transaction);
        }
        /// <summary>
        /// Add transaction to list of incoming transactions
        /// </summary>
        /// <param name="transaction">
        /// <see cref="Transaction"/> entity
        /// </param>
        /// <exception cref="InvalidTransactionException">
        /// Transaction destination account id and this account id does not match
        /// </exception>
        internal void AddIncomingTransaction(Transaction transaction)
        {
            if (transaction.ToAccountId != Id)
            {
                throw new InvalidTransactionException("The transaction is not addressed to this account");
            }
            _incomingTransactions.Add(transaction);
        }
    }
}


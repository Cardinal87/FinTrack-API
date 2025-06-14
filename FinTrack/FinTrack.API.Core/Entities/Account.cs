
using FinTrack.API.Core.Common;
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
        /// <exception cref="InvalidOperationException">amount param is greater than balance</exception>
        public void Debit(decimal amount)
        {
            if (Balance < amount)
            {  
                throw new InvalidOperationException("Insufficient funds");
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
        public void TopUp(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("amount must be positive");
            }
            Balance += amount;
        }

        internal void AddOutgoingTransaction(Transaction transaction)
        {
            if (transaction.FromAccountId != Id)
            {
                throw new ArgumentException("Transaction not for this account");
            }
            _outgoingTransactions.Add(transaction);
        }

        internal void AddIncomingTransaction(Transaction transaction)
        {
            if (transaction.ToAccountId != Id)
            {
                throw new ArgumentException("Transaction not for this account");
            }
            _incomingTransactions.Add(transaction);
        }
    }
}




using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;

namespace FinTrack.API.Core.Services
{
    /// <summary>
    /// Service for handling transactions
    /// </summary>
    public class TransferService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransferService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }


        /// <summary>
        /// Handles transaction between accounts
        /// </summary>
        /// <param name="amount">amount of the transaction</param>
        /// <param name="toAccountId">id of the destination account</param>
        /// <param name="fromAccountId">id of the source account</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">
        /// Source or destination accounts does not exists
        /// </exception>
        /// <exception cref="InsufficientFundsException">
        /// Amount of the transaction is greater than the source account balance
        /// </exception>
        /// <exception cref="IncorrectAmountException">
        /// Amount of the transaction is negative or equal to zero
        /// </exception>
        async public Task<Guid> HandleTransactionAsync(decimal amount, Guid toAccountId, Guid fromAccountId)
        {
            var toAccount = await _accountRepository.GetByIdAsync(toAccountId);
            var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);

            if (toAccount == null || fromAccount == null)
            {
                throw new EntityNotFoundException("One of the accounts does not exist");
            }
            var transaction = new Transaction(amount, fromAccountId, toAccountId, DateTime.UtcNow);

            fromAccount.Debit(amount);
            fromAccount.AddOutgoingTransaction(transaction);

            toAccount.TopUp(amount);
            toAccount.AddIncomingTransaction(transaction);


            _transactionRepository.Add(transaction);
            await _transactionRepository.SaveChangesAsync();

            await _accountRepository.UpdateAsync(toAccount);
            await _accountRepository.UpdateAsync(fromAccount);

            await _accountRepository.SaveChangesAsync();
            return transaction.Id;
        }
    }
}



using FinTrack.API.Core.Entities;
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

        async public Task<Guid> HandleTransactionAsync(decimal amount, Guid toAccountId, Guid fromAccountId)
        {
            var toAccount = await _accountRepository.GetByIdAsync(toAccountId);
            var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);

            if (toAccount == null || fromAccount == null)
            {
                throw new ArgumentException("One of accounts does not exist");
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

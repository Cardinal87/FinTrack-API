using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Exceptions;
using MediatR;

namespace FinTrack.API.Application.UseCases.Account.DebitBalance
{

    /// <summary>
    /// Handles balance debit
    /// </summary>
    /// <remarks>
    /// Steps:
    /// <para>1. Check if the account with given id exists</para>
    /// <para>2. Use entity method to debit balance</para>
    /// <para>3. Use repository to update entity and save changes</para>
    /// 
    /// <para>
    /// Exceptions:
    /// <para> - <see cref="ArgumentException"/>: debit amount is negative or equal to zero</para>
    /// <para> - <see cref="ArgumentNullException"/>: account with given id does not exists</para>
    /// <para> - <see cref="InsufficientFundsException"/>: debit amount is greater than balance of the account</para>
    /// 
    /// </para>
    /// </remarks>                                               
    internal class DebitAccountHandler : IRequestHandler<DebitBalanceCommand, decimal>
    {
        private readonly IAccountRepository _accountRepository;

        public DebitAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<decimal> Handle(DebitBalanceCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                throw new ArgumentNullException("account with given id does not exist");
            }

            account.Debit(request.amount);
            await _accountRepository.UpdateAsync(account);
            await _accountRepository.SaveChangesAsync();

            return account.Balance;
        }
    }
}

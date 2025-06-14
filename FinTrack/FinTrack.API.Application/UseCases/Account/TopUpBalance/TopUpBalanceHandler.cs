
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Account.TopUpBalance
{

    /// <summary>
    /// Handles account deposits
    /// </summary>
    /// <remarks>
    /// Steps:
    /// <para>1. Check if account with given id exists</para>
    /// <para>2. Top up balance using entity method</para>
    /// <para>3. Update entity in repository and save changes</para>
    /// 
    /// <para>
    /// Exceptions:
    /// <para> - <see cref="ArgumentException"/>: amount of the deposit is negative or equal to zero</para>
    /// <para> - <see cref="ArgumentNullException"/>: account does not exist</para>
    /// </para>
    /// </remarks>
    internal class TopUpBalanceHandler : IRequestHandler<TopUpBalanceCommand, decimal>
    {
        private readonly IAccountRepository _accountRepository;

        public TopUpBalanceHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<decimal> Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account with given id does not exist");
            }
            account.TopUp(request.amount);

            
            await _accountRepository.UpdateAsync(account);
            await _accountRepository.SaveChangesAsync();
            return account.Balance;
        }
    }
}

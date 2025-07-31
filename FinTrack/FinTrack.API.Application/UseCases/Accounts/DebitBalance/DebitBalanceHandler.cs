using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Exceptions;
using MediatR;
using FinTrack.API.Application.Common;

namespace FinTrack.API.Application.UseCases.Accounts.DebitBalance
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
    internal class DebitBalanceHandler : IRequestHandler<DebitBalanceCommand, ValueResult<decimal>>
    {
        private readonly IAccountRepository _accountRepository;

        public DebitBalanceHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<decimal>> Handle(DebitBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(request.accountId);
                if (account == null)
                {
                    return ValueResult<decimal>.Fail(OperationStatusMessages.NotFound);
                }

                account.Debit(request.amount);
                await _accountRepository.UpdateAsync(account);
                await _accountRepository.SaveChangesAsync();

                return ValueResult<decimal>.Ok(account.Balance, OperationStatusMessages.Ok);
            }
            catch (IncorrectAmountException)
            {
                return ValueResult<decimal>.Fail(OperationStatusMessages.BadRequest);
            }
            catch (InsufficientFundsException)
            {
                return ValueResult<decimal>.Fail(OperationStatusMessages.BadRequest);
            }
        }
    }
}

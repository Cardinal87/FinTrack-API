using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Exceptions;
using MediatR;
using FinTrack.API.Application.Common;

namespace FinTrack.API.Application.UseCases.Accounts.DebitBalance
{

   
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

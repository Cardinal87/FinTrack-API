
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.TopUpBalance
{

    
    internal class TopUpBalanceHandler : IRequestHandler<TopUpBalanceCommand, ValueResult<decimal>>
    {
        private readonly IAccountRepository _accountRepository;

        public TopUpBalanceHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<decimal>> Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(request.accountId);
                if (account == null)
                {
                    return ValueResult<decimal>.Fail(OperationStatusMessages.NotFound);
                }
                account.TopUp(request.amount);


                await _accountRepository.UpdateAsync(account);
                await _accountRepository.SaveChangesAsync();
                return ValueResult<decimal>.Ok(account.Balance, OperationStatusMessages.Ok);
            }
            catch (IncorrectAmountException)
            {
                return ValueResult<decimal>.Fail(OperationStatusMessages.BadRequest);
            }

        }
    }
}

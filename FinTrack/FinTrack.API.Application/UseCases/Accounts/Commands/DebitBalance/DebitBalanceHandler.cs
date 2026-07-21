using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Exceptions;
using MediatR;
using FinTrack.API.Application.Common;

namespace FinTrack.API.Application.UseCases.Accounts.Commands.DebitBalance
{

   
    internal class DebitBalanceHandler : IRequestHandler<DebitBalanceCommand, ValueResult<decimal>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DebitBalanceHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        async public Task<ValueResult<decimal>> Handle(DebitBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(request.accountId);
                if (account == null)
                {
                    return ValueResult<decimal>.Fail(OperationStatusMessages.NotFound, "account was not found");
                }

                account.Debit(request.amount);
                await _accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();

                return ValueResult<decimal>.Ok(account.Balance, OperationStatusMessages.Ok);
            }
            catch (IncorrectAmountException)
            {
                return ValueResult<decimal>.Fail(OperationStatusMessages.BadRequest, "amount is incorrect");
            }
            catch (InsufficientFundsException)
            {
                return ValueResult<decimal>.Fail(OperationStatusMessages.BadRequest, "insufficient funds to debit balance");
            }
        }
    }
}

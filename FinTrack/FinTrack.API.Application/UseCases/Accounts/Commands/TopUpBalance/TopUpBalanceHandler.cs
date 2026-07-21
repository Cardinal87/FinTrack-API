using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.Commands.TopUpBalance
{

    
    internal class TopUpBalanceHandler : IRequestHandler<TopUpBalanceCommand, ValueResult<decimal>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TopUpBalanceHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        async public Task<ValueResult<decimal>> Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(request.accountId);
                if (account == null)
                {
                    return ValueResult<decimal>.Fail(OperationStatusMessages.NotFound, "account was not found");
                }
                account.TopUp(request.amount);


                await _accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();
                return ValueResult<decimal>.Ok(account.Balance, OperationStatusMessages.Ok);
            }
            catch (IncorrectAmountException)
            {
                return ValueResult<decimal>.Fail(OperationStatusMessages.BadRequest, "ammont is not correct");
            }

        }
    }
}

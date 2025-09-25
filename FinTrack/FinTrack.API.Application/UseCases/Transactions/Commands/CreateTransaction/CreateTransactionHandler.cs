using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Services;
using MediatR;
namespace FinTrack.API.Application.UseCases.Transactions.Commands.CreateTransaction
{
    
    /// <summary>
    /// Handles trasaction between accounts
    /// </summary>
    /// <remarks>
    /// Use core service for handle transaction
    /// </remarks>
    internal class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, ValueResult<Guid>>
    {
        private TransferService _transferService;
        private IAccountRepository _accountRepository;

        public CreateTransactionHandler(TransferService transferService, IAccountRepository accountRepository)
        {
            _transferService = transferService;
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userAccountIds = await _accountRepository.GetAccountIdsByUserIdAsync(request.userId);

                if (!userAccountIds.Contains(request.fromAccountId))
                {
                    return ValueResult<Guid>.Fail(OperationStatusMessages.Forbidden, "you have not got access to this account");
                }
                var guid = await _transferService.HandleTransactionAsync(request.amount,
                                                              request.toAccountId,
                                                              request.fromAccountId);

                return ValueResult<Guid>.Ok(guid, OperationStatusMessages.Created);
            }
            catch (IncorrectAmountException)
            {
                return ValueResult<Guid>.Fail(OperationStatusMessages.BadRequest, "amonut is incorrect");
            }
            catch (InsufficientFundsException)
            {
                return ValueResult<Guid>.Fail(OperationStatusMessages.BadRequest, "insufficient funds to create transaction");
            }
            catch (EntityNotFoundException)
            {
                return ValueResult<Guid>.Fail(OperationStatusMessages.NotFound, "one of the accounts were not found");
            }
            
        }
    }
}

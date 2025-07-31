using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Services;
using MediatR;
namespace FinTrack.API.Application.UseCases.Transactions.CreateTransaction
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

        public CreateTransactionHandler(TransferService transferService)
        {
            _transferService = transferService;
        }

        async public Task<ValueResult<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var guid = await _transferService.HandleTransactionAsync(request.amount,
                                                              request.toAccountId,
                                                              request.fromAccountId);

                return ValueResult<Guid>.Ok(guid, OperationStatusMessages.Created);
            }
            catch (IncorrectAmountException)
            {
                return ValueResult<Guid>.Fail(OperationStatusMessages.BadRequest);
            }
            catch (EntityNotFoundException)
            {
                return ValueResult<Guid>.Fail(OperationStatusMessages.NotFound);
            }
        }
    }
}

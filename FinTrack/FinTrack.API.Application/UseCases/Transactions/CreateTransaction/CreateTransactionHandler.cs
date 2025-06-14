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
    internal class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, Guid>
    {
        private TransferService _transferService;

        public CreateTransactionHandler(TransferService transferService)
        {
            _transferService = transferService;
        }

        async public Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var guid = await _transferService.HandleTransactionAsync(request.amount,
                                                          request.toAccountId,
                                                          request.fromAccountId);

            return guid;
        }
    }
}

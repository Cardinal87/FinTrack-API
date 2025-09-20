using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.Queries.GetAllTransactions
{
    internal class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsQuery, ValueResult<IReadOnlyCollection<Transaction>>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetAllTransactionsHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        async public Task<ValueResult<IReadOnlyCollection<Transaction>>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository.GetAllAsync(request.pageNumber, request.pageSize);
            return ValueResult<IReadOnlyCollection<Transaction>>.Ok(transactions.ToList().AsReadOnly(), OperationStatusMessages.Ok);
        }
    }
}

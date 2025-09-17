using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.Queries.GetTransactionsByDate
{
    class GetTransactionsByDateHandler 
        : IRequestHandler<GetTransactionsByDateQuery, ValueResult<IReadOnlyCollection<Transaction>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public GetTransactionsByDateHandler(ITransactionRepository transactionRepository,
                                            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<IReadOnlyCollection<Transaction>>> Handle(GetTransactionsByDateQuery request, CancellationToken cancellationToken)
        {
            if (request.roles.Contains(UserRoles.Admin))
            {
                var transactions = await _transactionRepository.GetByDateAsync(request.date, request.pageNumber, request.pageSize);
                return ValueResult<IReadOnlyCollection<Transaction>>.Ok(transactions.ToList(), OperationStatusMessages.Ok);
            }
            var userAccoutsIds = await _accountRepository.GetAccountIdsByUserIdAsync(request.userId);

            var allowed = await _transactionRepository.GetByDateAsync(request.date, userAccoutsIds, request.pageNumber, request.pageSize);
            return ValueResult<IReadOnlyCollection<Transaction>>.Ok(allowed.ToList(), OperationStatusMessages.Ok);
        }
    }
}

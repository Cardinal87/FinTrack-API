
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.GetTransactionsByTimeInterval
{
    class GetTransactionsByTimeIntervalHandler
        : IRequestHandler<GetTransactionsByTimeIntervalCommand, ValueResult<IReadOnlyCollection<Transaction>>>
    {

        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public GetTransactionsByTimeIntervalHandler(ITransactionRepository transactionRepository,
                                            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<IReadOnlyCollection<Transaction>>> Handle(GetTransactionsByTimeIntervalCommand request, CancellationToken cancellationToken)
        {
            if (request.roles.Contains(UserRoles.Admin))
            {
                var transactions = await _transactionRepository.GetFromToDateAsync(request.from, request.to, request.pageNumber, request.pageSize);
                return ValueResult<IReadOnlyCollection<Transaction>>.Ok(transactions.ToList().AsReadOnly(), OperationStatusMessages.Ok);
            }
            var userAccoutsIds = await _accountRepository.GetAccountIdsByUserIdAsync(request.userId);

            var allowed = await _transactionRepository.GetFromToDateAsync(request.from, request.to, userAccoutsIds, request.pageNumber, request.pageSize);
            return ValueResult<IReadOnlyCollection<Transaction>>.Ok(allowed.ToList().AsReadOnly(), OperationStatusMessages.Ok);
        }
    }
}


using FinTrack.API.Application.Common;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Common;
using MediatR;
using FinTrack.API.Core.Entities;

namespace FinTrack.API.Application.UseCases.Transactions.Queries.GetAccountTransactions
{
    internal class GetAccountTransactionsHandler
        : IRequestHandler<GetAccountTransactionsQuery, ValueResult<IReadOnlyCollection<Transaction>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public GetAccountTransactionsHandler(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        
        
        async public Task<ValueResult<IReadOnlyCollection<Transaction>>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                return ValueResult<IReadOnlyCollection<Transaction>>.Fail(OperationStatusMessages.NotFound, "account was not found");
            }

            if (request.roles.Contains(UserRoles.Admin)
                || account.UserId == request.userId)
            {
                var result = await _transactionRepository.GetAccountTransactionsAsync(account.Id, request.pageNumber, request.pageSize);
                return ValueResult<IReadOnlyCollection<Transaction>>.Ok(result.ToList().AsReadOnly(), OperationStatusMessages.Ok);
            }
            return ValueResult<IReadOnlyCollection<Transaction>>.Fail(OperationStatusMessages.Forbidden, "you have not got access to this account");
        }
    }
}

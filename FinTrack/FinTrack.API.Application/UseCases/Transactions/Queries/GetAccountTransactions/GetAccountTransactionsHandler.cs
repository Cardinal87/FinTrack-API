
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

        public GetAccountTransactionsHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        
        async public Task<ValueResult<IReadOnlyCollection<Transaction>>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                return ValueResult<IReadOnlyCollection<Transaction>>.Fail(OperationStatusMessages.NotFound);
            }

            if (request.roles.Contains(UserRoles.Admin)
                || account.UserId == request.userId)
            {
                var combined = account.IncomingTransactions.Concat(account.OutgoingTransactions).ToList();
                return ValueResult<IReadOnlyCollection<Transaction>>.Ok(combined.AsReadOnly(), OperationStatusMessages.Ok);
            }
            return ValueResult<IReadOnlyCollection<Transaction>>.Fail(OperationStatusMessages.Forbidden);
        }
    }
}

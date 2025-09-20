

using FinTrack.API.Application.Common;
using FinTrack.API.Application.UseCases.Transactions.Queries.GetAllTransactions;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.Queries.GetAllAccounts
{
    internal class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, ValueResult<IReadOnlyCollection<Account>>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAllAccountsHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<IReadOnlyCollection<Account>>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _accountRepository.GetAllAsync(request.pageNumber, request.pageSize);
            return ValueResult<IReadOnlyCollection<Account>>.Ok(transactions.ToList().AsReadOnly(), OperationStatusMessages.Ok);
        }
    }
}

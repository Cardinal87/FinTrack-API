

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Common;
using MediatR;
using FinTrack.API.Application.Common;

namespace FinTrack.API.Application.UseCases.Accounts.GetAccount
{
    internal class GetAccountHandler : IRequestHandler<GetAccountCommand, ValueResult<Account>>
    {
        private readonly IAccountRepository _accountRepository;
        public GetAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<ValueResult<Account>> Handle(GetAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                return ValueResult<Account>.Fail(OperationStatusMessages.NotFound);
            }
            if (request.roles.Contains(UserRoles.Admin))
            {
                return ValueResult<Account>.Ok(account, OperationStatusMessages.Ok);
            }
            if (request.userId != account?.UserId)
            {
                return ValueResult<Account>.Fail(OperationStatusMessages.Forbidden);
            }
            return ValueResult<Account>.Ok(account, OperationStatusMessages.Ok); ;
            
        }
    }
}

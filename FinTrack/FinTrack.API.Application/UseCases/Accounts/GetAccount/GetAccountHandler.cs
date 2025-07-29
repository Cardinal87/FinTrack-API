

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.GetAccount
{
    internal class GetAccountHandler : IRequestHandler<GetAccountCommand, Account?>
    {
        private readonly IAccountRepository _accountRepository;
        public GetAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<Account?> Handle(GetAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (request.roles.Contains(UserRoles.Admin))
            {
                return account;
            }
            if (request.userId != account?.UserId)
            {
                throw new ArgumentException("access denied");
            }
            return account;
            
        }
    }
}

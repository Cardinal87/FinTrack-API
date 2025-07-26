

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
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
            var account = await _accountRepository.GetByIdAsync(request.guid);
            return account;
        }
    }
}

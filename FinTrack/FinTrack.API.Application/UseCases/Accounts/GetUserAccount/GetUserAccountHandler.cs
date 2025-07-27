

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.GetUserAccount
{
    internal class GetUserAccountHandler : IRequestHandler<GetUserAccountCommand, Account?>
    {
        private readonly IAccountRepository _accountRepository;

        public GetUserAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<Account?> Handle(GetUserAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                return null;
            }
            if (account.UserId != request.userId)
            {
                throw new ArgumentException($"account with id {request.accountId} does not belong to user {request.userId}");
            }
            return account;
        }
    }
}

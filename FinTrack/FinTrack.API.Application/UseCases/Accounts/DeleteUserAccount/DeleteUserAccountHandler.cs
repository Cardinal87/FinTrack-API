
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DeleteUserAccount
{
    internal class DeleteUserAccountHandler : IRequestHandler<DeleteUserAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteUserAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task Handle(DeleteUserAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if (account == null)
            {
                throw new KeyNotFoundException($"account with id {request.accountId} does not exits");
            }
            if (account.UserId != request.userId)
            {
                throw new ArgumentException($"account with id {request.accountId} does not belong to user {request.userId}");
            }
            await _accountRepository.DeleteAsync(request.accountId);
            await _accountRepository.SaveChangesAsync();
        }
    }
}

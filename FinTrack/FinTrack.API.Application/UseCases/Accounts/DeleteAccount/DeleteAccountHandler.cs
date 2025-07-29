

using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DeleteAccount
{
    /// <summary>
    /// Handler deleting account
    /// </summary>
    /// <remarks>
    /// Steps:
    /// <para>1. Use repository to delete account and save changes</para>
    /// 
    /// <para>
    /// Exceptions:
    /// <para><see cref="KeyNotFoundException"/> - account with given id does not exist</para>
    /// </para>
    /// </remarks>
    internal class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if(account == null)
            {
                throw new KeyNotFoundException($"accoun with id {request.accountId} does not exist");
            }
            if (request.roles.Contains(UserRoles.Admin) ||
                request.userId == account.UserId)
            {
                await _accountRepository.DeleteAsync(request.accountId);
                await _accountRepository.SaveChangesAsync();
                return;
            }
            throw new ArgumentException("access denied");


        }
    }
}

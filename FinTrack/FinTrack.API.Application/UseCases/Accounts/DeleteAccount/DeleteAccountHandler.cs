

using FinTrack.API.Core.Interfaces;
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
    /// <para><see cref="KeyNotFoundException"/> - user with given id does not exist</para>
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
            await _accountRepository.DeleteAsync(request.userId);
            await _accountRepository.SaveChangesAsync();


        }
    }
}



using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Common;
using MediatR;
using FinTrack.API.Application.Common;

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
    internal class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, Result>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteAccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        async public Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.accountId);
            if(account == null)
            {
                return Result.Fail(OperationStatusMessages.NotFound);
            }
            if (request.roles.Contains(UserRoles.Admin) ||
                request.userId == account.UserId)
            {
                await _accountRepository.DeleteAsync(request.accountId);
                await _accountRepository.SaveChangesAsync();
                return Result.Ok(OperationStatusMessages.NoContent);
            }
            return Result.Fail(OperationStatusMessages.Forbidden);


        }
    }
}

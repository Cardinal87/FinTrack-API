
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DeleteUserAccount
{
    /// <summary>
    /// Represents MediatR command for deleting user's account.
    /// </summary>
    /// <param name="userId">
    /// Id of the existing user
    /// </param>
    /// <param name="accountId">
    /// Id of the user's account
    /// </param>
    public record DeleteUserAccountCommand(Guid userId, Guid accountId) : IRequest;
}

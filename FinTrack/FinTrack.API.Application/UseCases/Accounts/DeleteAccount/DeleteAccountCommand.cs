

using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DeleteAccount
{
    /// <summary>
    /// Represents MediatR command for deleting account.
    /// </summary>
    /// <param name="accountId">
    /// Id of the existing account
    /// </param>
    /// <param name="userGuid">
    /// Id of the user that invokes command
    /// </param>
    /// <param name="roles">
    /// User roles
    /// </param>
    public record DeleteAccountCommand(Guid userId, IReadOnlyCollection<string> roles, Guid accountId) : IRequest<Result>;
    
}

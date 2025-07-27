

using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.GetUserAccount
{
    /// <summary>
    /// Represents MediatR command for getting user's account.
    /// </summary>
    /// <param name="userId">
    /// Id of the existing user
    /// </param>
    /// <param name="accountId">
    /// Id of the user's account
    /// </param>
    public record GetUserAccountCommand(Guid userId, Guid accountId) : IRequest<Account?>;
    
}



using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.GetAccount
{

    /// <summary>
    /// Represents MediatR command for getting account by id.
    /// Returns <see cref="ValueResult{T}"/> with <see cref="Account"/> if it exists and
    /// user has permission to get it
    /// else returns <see langword="null"/>.
    /// </summary>
    /// <param name="accountId">Account id</param>
    /// <param name="userId">Id of the user that invokes command</param>
    /// <param name="roles">User roles</param>
    public record GetAccountCommand(Guid userId, IReadOnlyCollection<string> roles, Guid accountId) : IRequest<ValueResult<Account>>;
}

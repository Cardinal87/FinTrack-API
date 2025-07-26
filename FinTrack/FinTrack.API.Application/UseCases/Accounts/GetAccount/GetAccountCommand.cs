

using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.GetAccount
{

    /// <summary>
    /// Represents MediatR command for getting account by id.
    /// Returns <see cref="Account"/> if exists
    /// else returns <see langword="null"/>.
    /// </summary>
    /// <param name="guid">Account id</param>
    public record GetAccountCommand(Guid guid) : IRequest<Account?>;
}

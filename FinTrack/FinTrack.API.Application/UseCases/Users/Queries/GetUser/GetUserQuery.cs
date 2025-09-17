using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Queries.GetUser
{
    /// <summary>
    /// Represents MediatR command for getting user by id.
    /// Returns <see cref="ValueResult{T}"/> with <see cref="User"/> if it exists
    /// else returns <see langword="null"/>.
    /// </summary>
    /// <param name="guid">User id</param>
    public record GetUserQuery(Guid guid) : IRequest<ValueResult<User>>;

}

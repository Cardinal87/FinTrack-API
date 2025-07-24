
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.GetUser
{
    /// <summary>
    /// Represents MediatR command for getting user by id.
    /// Returns <see cref="User"/> if exists
    /// else returns <see langword="null"/>.
    /// </summary>
    /// <param name="guid">User id</param>
    public record GetUserCommand(Guid guid) : IRequest<User?>;

}

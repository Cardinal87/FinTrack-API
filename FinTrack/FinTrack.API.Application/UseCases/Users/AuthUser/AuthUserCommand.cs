using FinTrack.API.Core.Entities;
using FinTrack.API.Application.Common;
using MediatR;
namespace FinTrack.API.Application.UseCases.Users.AuthUser
{
    /// <summary>
    /// Represents MediatR command for checking if the user exists
    /// and are the credentials correct.
    /// Returns <see cref="ValueResult{T}"/> with <see cref="User"/> if it exists and credentials are correct
    /// else returns <see langword="null"/>.
    /// </summary>
    /// <param name="login">User's login</param>
    /// <param name="password">User's password</param>
    public record AuthUserCommand(string login,
                                   string password) : IRequest<ValueResult<User>>;
}

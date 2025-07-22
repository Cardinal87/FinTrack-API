using MediatR;
namespace FinTrack.API.Application.UseCases.Users.CheckUser
{
    /// <summary>
    /// Represents MediatR command for checking if the user exists
    /// and are the credentials correct.
    /// Returns <see langword="true"/> if the user exists and credentials are correct
    /// else returns <see langword="false"/>.
    /// </summary>
    /// <param name="login">User's login</param>
    /// <param name="password">User's password</param>
    public record AuthUserCommand(string login,
                                   string password) : IRequest<bool>;
}

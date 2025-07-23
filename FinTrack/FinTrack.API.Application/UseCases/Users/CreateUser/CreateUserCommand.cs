using MediatR;

namespace FinTrack.API.Application.UseCases.Users.CreateUser
{

    /// <summary>
    /// Represents MediatR command for creating new user.
    /// Returns id of the created user
    /// </summary>
    /// <param name="phone">phone number inc E.164 format</param>
    /// <param name="email">valid email address</param>
    /// <param name="name">full name (1-100 characters)</param>
    /// <param name="password">valid password string</param>
    public record CreateUserCommand(string phone,
                                    string email,
                                    string name,
                                    string password) : IRequest<Guid>;
}

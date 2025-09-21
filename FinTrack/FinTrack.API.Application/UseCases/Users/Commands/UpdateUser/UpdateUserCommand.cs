

using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Commands.UpdateUser
{
    /// <summary>
    /// Represents MediatR command for updating user
    /// </summary>
    /// <param name="name">new name (optional)</param>
    /// <param name="email">new email (optional)</param>
    /// <param name="phone">new phone (optional)</param>
    /// <param name="userId">Id of the user that will be updated</param>
    public record UpdateUserCommand(string? name,
                                    string? email,
                                    string? phone,
                                    Guid userId) : IRequest<Result>;
}

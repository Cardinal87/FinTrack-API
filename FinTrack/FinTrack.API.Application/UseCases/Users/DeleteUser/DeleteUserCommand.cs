using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.DeleteUser
{
    /// <summary>
    /// Represents MediatR command for deleting existing user.
    /// Returns <see cref="Result"/> with operation status
    /// </summary>
    /// <param name="id">id of the existing user to delete</param>
    public record DeleteUserCommand(Guid id) : IRequest<Result>;
   
}
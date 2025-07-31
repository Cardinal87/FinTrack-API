
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.DeleteUser
{
    /// <summary>
    /// Handles user deleting
    /// </summary>
    /// <remarks>
    /// Steps:
    /// <para>1. Delete user with given id</para>
    /// <para>2. Persist data via repository</para>
    /// 
    /// <para>
    /// Exceptions:
    /// <para>- <see cref="KeyNotFoundException"/>: User with given id does not exist</para>
    /// </para>
    /// </remarks>
    internal class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
    {

        private IUserRepository _userRepository;

        public DeleteUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async public Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.DeleteAsync(request.id);
                await _userRepository.SaveChangesAsync();
                return Result.Ok(OperationStatusMessages.NoContent);
            }
            catch(KeyNotFoundException)
            {
                return Result.Fail(OperationStatusMessages.NotFound);
            }
        }
    }
}


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
    internal class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
    {

        private IUserRepository _userRepository;

        public DeleteUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async public Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.DeleteAsync(request.id);
            await _userRepository.SaveChangesAsync();
        }
    }
}

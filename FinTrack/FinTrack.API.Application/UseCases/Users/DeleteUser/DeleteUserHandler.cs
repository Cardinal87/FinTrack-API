
using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.DeleteUser
{
    
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
            catch(EntityNotFoundException)
            {
                return Result.Fail(OperationStatusMessages.NotFound);
            }
        }
    }
}

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Commands.DeleteUser
{
    
    internal class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result>
    {

        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        async public Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.DeleteAsync(request.id);
                await _unitOfWork.SaveChangesAsync();
                return Result.Ok(OperationStatusMessages.NoContent);
            }
            catch(EntityNotFoundException)
            {
                return Result.Fail(OperationStatusMessages.NotFound, "user is not found");
            }
        }
    }
}

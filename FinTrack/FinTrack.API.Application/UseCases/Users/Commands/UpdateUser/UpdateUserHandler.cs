

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FinTrack.API.Application.UseCases.Users.Commands.UpdateUser
{
    internal class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserHandler> _logger;

        public UpdateUserHandler(IUserRepository userRepository, ILogger<UpdateUserHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        async public Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.userId);
                if (user == null)
                {
                    return Result.Fail(OperationStatusMessages.NotFound);
                }
                if (request.name != null)
                {
                    user.Name = request.name;
                }
                if (request.email != null)
                {
                    user.Email = request.email;
                }
                if (request.phone != null)
                {
                    user.Phone = request.phone;
                }
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
                return Result.Ok(OperationStatusMessages.NoContent);
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex, "Data validation for update failed");
                return Result.Fail(OperationStatusMessages.BadRequest);
            }
            catch(UniqueConstraintViolationException ex)
            {
                _logger.LogWarning(ex, $"Property {ex.Property} violates unique constraint");
                return Result.Fail(OperationStatusMessages.BadRequest);
            }
        }
    }
}

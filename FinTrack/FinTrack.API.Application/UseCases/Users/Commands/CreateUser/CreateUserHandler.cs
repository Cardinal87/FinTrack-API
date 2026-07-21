using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace FinTrack.API.Application.UseCases.Users.Commands.CreateUser
{
    internal class CreateUserHandler : IRequestHandler<CreateUserCommand, ValueResult<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<CreateUserHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        
        public CreateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,  ILogger<CreateUserHandler> logger, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        
        async public Task<ValueResult<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var hash = _passwordHasher.GetHash(request.password);

                var user = new User(request.email,
                                    request.phone,
                                    request.name,
                                    hash);
                user.AssignRole(UserRoles.User);

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return ValueResult<Guid>.Ok(user.Id, OperationStatusMessages.Created);
            }
            catch (UniqueConstraintViolationException ex)
            {
                _logger.LogWarning(ex, $"Property {ex.Property} violates unique constraint");
                return ValueResult<Guid>.Fail(OperationStatusMessages.BadRequest, "username, email or phone is already registered");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Validation error during creating new user");
                return ValueResult<Guid>.Fail(OperationStatusMessages.BadRequest, "provided data for creating user is invalid");
            }
        }
    }
}

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
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
        
        public CreateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,  ILogger<CreateUserHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
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

                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync();
                return ValueResult<Guid>.Ok(user.Id, OperationStatusMessages.Created);
            }
            catch(UniqueConstraintViolationException ex)
            {
                _logger.LogWarning(ex, $"Property {ex.Property} violates unique constraint");
                return ValueResult<Guid>.Fail(OperationStatusMessages.BadRequest);
            }
        }
    }
}

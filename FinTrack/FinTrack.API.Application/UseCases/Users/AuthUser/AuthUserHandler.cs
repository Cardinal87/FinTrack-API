

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.AuthUser
{
    internal class AuthUserHandler : IRequestHandler<AuthUserCommand, ValueResult<User>>
    {
        private IUserRepository _userRepository;
        private IPasswordHasher _passwordHasher;
        
        public AuthUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        async public Task<ValueResult<User>> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.login);
            if (user != null)
            {
                var isValidCredentials = _passwordHasher.VerifyPassword(user.PasswordHash, request.password);
                if (isValidCredentials)
                {
                    return ValueResult<User>.Ok(user, OperationStatusMessages.Ok);
                }
                return ValueResult<User>.Fail(OperationStatusMessages.Unauthorized);
            }
            return ValueResult<User>.Fail(OperationStatusMessages.NotFound);
        }
    }
}

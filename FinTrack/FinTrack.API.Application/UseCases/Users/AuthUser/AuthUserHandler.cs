

using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.AuthUser
{
    internal class AuthUserHandler : IRequestHandler<AuthUserCommand, User?>
    {
        private IUserRepository _userRepository;
        private IPasswordHasher _passwordHasher;
        
        public AuthUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        async public Task<User?> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.login);
            if (user != null)
            {
                var isValidCredentials = _passwordHasher.VerifyPassword(user.PasswordHash, request.password);
                return isValidCredentials? user :  null;
            }
            return null;
        }
    }
}



using FinTrack.API.Application.UseCases.Users.CheckUser;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.AuthUser
{
    internal class AuthUserHandler : IRequestHandler<AuthUserCommand, bool>
    {
        private IUserRepository _userRepository;
        private IPasswordHasher _passwordHasher;
        
        public AuthUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        async public Task<bool> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.login);
            if (user == null)
            {
                return false;
            }
            return _passwordHasher.VerifyPassword(user.PasswordHash, request.password);
        }
    }
}

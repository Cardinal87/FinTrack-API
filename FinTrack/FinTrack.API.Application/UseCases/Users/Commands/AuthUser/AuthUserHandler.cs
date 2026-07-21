using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Commands.AuthUser
{
    internal class AuthUserHandler : IRequestHandler<AuthUserCommand, ValueResult<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthUserHandler(IUserRepository userRepository,
                               IPasswordHasher passwordHasher,
                               IJwtTokenService jwtTokenService,
                               IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _jwtTokenService = jwtTokenService;
        }

        async public Task<ValueResult<AuthResponse>> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.login);
            if (user != null)
            {
                var isValidCredentials = _passwordHasher.VerifyPassword(user.PasswordHash, request.password);
                if (isValidCredentials)
                {
                    var accessToken = await _jwtTokenService.GenerateTokenAsync(user);
                    var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);
                    return ValueResult<AuthResponse>.Ok(new AuthResponse(accessToken, refreshToken), OperationStatusMessages.Ok);
                }
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "login or password is incorrect");
            }
            return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "login or password is incorrect");
        }
    }
}

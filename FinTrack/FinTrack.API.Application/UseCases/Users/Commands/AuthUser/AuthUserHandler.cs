using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.Messages;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FinTrack.API.Application.UseCases.Users.Commands.AuthUser
{
    internal class AuthUserHandler : IRequestHandler<AuthUserCommand, ValueResult<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITotpService _totpService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<AuthUserHandler> _logger;

        public AuthUserHandler(IUserRepository userRepository,
                               IPasswordHasher passwordHasher,
                               ITotpService totpService,
                               IMessagePublisher messagePublisher,
                               ILogger<AuthUserHandler> logger,
                               IJwtTokenService jwtTokenService,
                               IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _totpService = totpService;
            _messagePublisher = messagePublisher;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _refreshTokenService = refreshTokenService;
        }

        async public Task<ValueResult<AuthResponse>> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.login);
            if (user == null)
            {
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Login or password is incorrect");
            }
            var isValidCredentials = _passwordHasher.VerifyPassword(user.PasswordHash, request.password);
            if (!isValidCredentials)
            {
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Login or password is incorrect");
            }


            if (user.TotpSecret != null)
            {
                if (!user.IsEmailVerified)
                {
                    return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Forbidden, "Email is not verified. Please verify your email first");
                }

                var challenge = await _jwtTokenService.GenerateTokenAsync(user, challenge: true);

                var code = _totpService.ComputeCode(user.TotpSecret);
                var message = new LoginVerificationCodeMessage(user.Id,
                                                               user.Email,
                                                               request.userAgent,
                                                               request.ip,
                                                               code);

                await _messagePublisher.PublishAsync("fintrack.verification.login", message, cancellationToken);
                _logger.LogDebug("Login verification message for user with id {id} was successfully published", user.Id);

                var response = new AuthResponse(challenge.Token, "", challenge.ExpiresIn);
                return ValueResult<AuthResponse>.Ok(response, OperationStatusMessages.Accepted);
            }
            else
            {
                var access = await _jwtTokenService.GenerateTokenAsync(user);
                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);

                var response = new AuthResponse(access.Token, refreshToken, access.ExpiresIn);
                return ValueResult<AuthResponse>.Ok(response, OperationStatusMessages.Ok);
            }

        }
    }
}

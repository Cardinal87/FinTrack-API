
using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.Messages;
using FinTrack.API.Application.UseCases.Users.Commands.AuthUser;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FinTrack.API.Application.UseCases.Identity.Commands.ResendLoginVerificarionCode
{
    internal class ResendLoginVerificationCodeHandler : IRequestHandler<ResendLoginVerificationCodeCommand, ValueResult<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITotpService _totpService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<ResendLoginVerificationCodeHandler> _logger;
        private readonly IChallengeTokenTracker _challengeTokenTracker;

        public ResendLoginVerificationCodeHandler(IUserRepository userRepository,
                                                  IJwtTokenService jwtTokenService,
                                                  ITotpService totpService,
                                                  IMessagePublisher messagePublisher,
                                                  ILogger<ResendLoginVerificationCodeHandler> logger,
                                                  IChallengeTokenTracker challengeTokenTracker)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _totpService = totpService;
            _messagePublisher = messagePublisher;
            _logger = logger;
            _challengeTokenTracker = challengeTokenTracker;
        }

        public async Task<ValueResult<AuthResponse>> Handle(ResendLoginVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.userId);
            if (user == null || user.TotpSecret == null)
            {
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Invalid verification session");
            }

            var challenge = await _jwtTokenService.GenerateTokenAsync(user, challenge: true);

            var code = _totpService.ComputeCode(user.TotpSecret);
            var message = new LoginVerificationCodeMessage(user.Id,
                                                           user.Email,
                                                           request.userAgent,
                                                           request.ip,
                                                           code);

            var success = await _challengeTokenTracker.TryMarkAsUsedAsync(request.jti, cancellationToken);
            if (!success)
            {
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Invalid verification session");
            }

            await _messagePublisher.PublishAsync("fintrack.verification.login", message, cancellationToken);
            _logger.LogDebug("Login verification message for user with id {id} was successfully published", user.Id);

            var response = new AuthResponse(challenge.Token, "", challenge.ExpiresIn);
            return ValueResult<AuthResponse>.Ok(response, OperationStatusMessages.Accepted);
        }
    }
}

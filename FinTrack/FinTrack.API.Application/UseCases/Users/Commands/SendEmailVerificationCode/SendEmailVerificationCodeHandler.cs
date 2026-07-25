
using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.Messages;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FinTrack.API.Application.UseCases.Users.Commands.SendEmailVerificationCode
{
    public class SendEmailVerificationCodeHandler : IRequestHandler<SendEmailVerificationCodeCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITotpService _totpService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<SendEmailVerificationCodeHandler> _logger;

        public SendEmailVerificationCodeHandler(IUserRepository userRepository,
                                                ITotpService totpService,
                                                IMessagePublisher messagePublisher,
                                                ILogger<SendEmailVerificationCodeHandler> logger)
        {
            _userRepository = userRepository;
            _totpService = totpService;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<Result> Handle(SendEmailVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.userId);

            if (user == null)
            {
                _logger.LogDebug("User with id {id} was not found", request.userId);
                return Result.Fail(OperationStatusMessages.NotFound, 
                    $"User with id {request.userId} was not found");
            }

            if (user.IsEmailVerified)
            {
                _logger.LogDebug("Email of user {id} is already verified", request.userId);
                return Result.Fail(OperationStatusMessages.Conflict, "Email is already verified");
            }

            if (user.TotpSecret == null)
            {
                _logger.LogDebug("TOTP secret is null for user {UserId}", request.userId);
                return Result.Fail(OperationStatusMessages.Conflict, "2FA is not enabled for this account");
            }

            var code = _totpService.ComputeCode(user.TotpSecret);
            var message = new EmailVerificationCodeMessage(request.userId, user.Email, code);
            await _messagePublisher.PublishAsync("fintrack.verification.email", message, cancellationToken);
            _logger.LogDebug("Email verification message for user {id} was successfully published", request.userId);
            return Result.Ok(OperationStatusMessages.Accepted);
        }
    }
}

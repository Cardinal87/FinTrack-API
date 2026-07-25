

using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Application.UseCases.Users.Commands.SendEmailVerificationCode;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FinTrack.API.Application.UseCases.Users.Commands.CheckEmailVerficationCode
{
    internal class CheckEmailVerificationCodeHandler : IRequestHandler<CheckEmailVerificationCodeCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITotpService _totpService;
        private readonly ILogger<CheckEmailVerificationCodeHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CheckEmailVerificationCodeHandler(IUserRepository userRepository,
                                                 ITotpService totpService,
                                                 ILogger<CheckEmailVerificationCodeHandler> logger,
                                                 IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _totpService = totpService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CheckEmailVerificationCodeCommand request, CancellationToken cancellationToken)
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


            var success = _totpService.VerifyCode(user.TotpSecret, request.code);
            if (!success)
            {
                _logger.LogDebug("Porvided totp code is invalid. User id - {id}", request.userId);
                return Result.Fail(OperationStatusMessages.BadRequest, "Verification code is invalid or expired");
            }
            user.VerifyEmail();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Email verified successfully. User id - {id}", request.userId);
            return Result.Ok(OperationStatusMessages.Ok);
            
        }
    }
}


using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Identity.Commands.CheckLoginVerificationCode
{
    internal class CheckLoginVerificationCodeHandler : IRequestHandler<CheckLoginVerificationCodeCommand, ValueResult<AuthResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITotpService _totpService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public CheckLoginVerificationCodeHandler(IUserRepository userRepository,
                                                 ITotpService totpService,
                                                 IJwtTokenService jwtTokenService,
                                                 IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _totpService = totpService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ValueResult<AuthResponse>> Handle(CheckLoginVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.userId);
            if (user == null || user.TotpSecret == null)
            {
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Invalid verification session");
            }

            var success = _totpService.VerifyCode(user.TotpSecret, request.code);
            if (success)
            {
                var tokenResult = await _jwtTokenService.GenerateTokenAsync(user);
                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);

                var result = new AuthResponse(tokenResult.Token, refreshToken, tokenResult.ExpiresIn);

                return ValueResult<AuthResponse>.Ok(result, OperationStatusMessages.Ok);
            }
            return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Verification code is invalid or expired");
        }
    }
}

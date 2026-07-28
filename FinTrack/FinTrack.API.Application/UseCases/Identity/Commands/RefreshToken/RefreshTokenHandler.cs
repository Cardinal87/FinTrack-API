

using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace FinTrack.API.Application.UseCases.Identity.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ValueResult<AuthResponse>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(IJwtTokenService jwtTokenService, 
            IRefreshTokenService refreshTokenService, 
            IUserRepository userRepository, 
            ILogger<RefreshTokenHandler> logger)
        {
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ValueResult<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _refreshTokenService.RotateRefreshTokenAsync(request.refreshToken, cancellationToken);

            if (!result.IsSuccess)
            {
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Provided refresh token is not valid");
            }

            var user = await _userRepository.GetByIdAsync(result.UserId);

            if (user == null) 
            {
                _logger.LogWarning("Token rotation was requested for the deleted user with id {userId}", result.UserId);
                return ValueResult<AuthResponse>.Fail(OperationStatusMessages.Unauthorized, "Provided refresh token is not valid");
            }

            var accessToken = await _jwtTokenService.GenerateTokenAsync(user);

            return ValueResult<AuthResponse>.Ok(new AuthResponse(accessToken.Token, result.RefreshToken!, accessToken.ExpiresIn), OperationStatusMessages.Ok);
        }
    }
}

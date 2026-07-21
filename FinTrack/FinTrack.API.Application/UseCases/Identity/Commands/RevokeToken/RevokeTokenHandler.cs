

using FinTrack.API.Application.Interfaces;
using MediatR;

namespace FinTrack.API.Application.UseCases.Identity.Commands.RevokeToken
{
    internal class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand>
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RevokeTokenHandler(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(request.refreshToken, cancellationToken);
        }
    }
}

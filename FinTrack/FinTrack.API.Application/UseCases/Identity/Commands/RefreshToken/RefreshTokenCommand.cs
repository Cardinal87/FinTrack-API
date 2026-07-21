using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Identity.Commands.RefreshToken
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<ValueResult<AuthResponse>>;
}

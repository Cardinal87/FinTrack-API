

using MediatR;

namespace FinTrack.API.Application.UseCases.Identity.Commands.RevokeToken
{
    public record RevokeTokenCommand(string refreshToken) : IRequest;
}

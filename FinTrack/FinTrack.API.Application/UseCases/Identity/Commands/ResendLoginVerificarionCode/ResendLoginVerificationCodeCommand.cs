

using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Identity.Commands.ResendLoginVerificarionCode
{
    public record ResendLoginVerificationCodeCommand(Guid userId, Guid jti, string userAgent, string ip) : IRequest<ValueResult<AuthResponse>>;
}

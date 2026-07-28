

using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Identity.Commands.CheckLoginVerificationCode
{
    public record CheckLoginVerificationCodeCommand(Guid userId, Guid jti, string code) : IRequest<ValueResult<AuthResponse>>;
}

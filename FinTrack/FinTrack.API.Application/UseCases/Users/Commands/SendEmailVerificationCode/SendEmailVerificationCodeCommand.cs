
using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Commands.SendEmailVerificationCode
{
    public record SendEmailVerificationCodeCommand(Guid userId) : IRequest<Result>;
}



using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Commands.CheckEmailVerficationCode
{
    public record CheckEmailVerificationCodeCommand(Guid userId, string code) : IRequest<Result>;
}

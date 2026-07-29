

namespace FinTrack.API.Application.Messages
{
    public readonly record struct EmailVerificationCodeMessage(Guid UserId, string Email, string TotpCode);
    public readonly record struct LoginVerificationCodeMessage(Guid UserId, string Email, string UserAgent, string Ip, string TotpCode);
}

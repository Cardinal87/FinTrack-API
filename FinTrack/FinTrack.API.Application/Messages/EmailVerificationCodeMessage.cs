namespace FinTrack.API.Application.Messages
{
    public record EmailVerificationCodeMessage(Guid userId, string email, string totpCode);
}

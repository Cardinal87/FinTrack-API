
namespace FinTrack.API.Application.Messages
{
    public record LoginVerificationCodeMessage(Guid userId, string email, string userAgent, string ip, string totpCode);
}

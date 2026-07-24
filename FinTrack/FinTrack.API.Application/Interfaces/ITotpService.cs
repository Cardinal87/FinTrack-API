
namespace FinTrack.API.Application.Interfaces
{
    public interface ITotpService
    {
        string GenerateSecret();                          
        string ComputeCode(string base32Secret);           
        bool VerifyCode(string base32Secret, string code);
    }
}

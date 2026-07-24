

using FinTrack.API.Application.Interfaces;
using OtpNet;
using SimpleBase;
using System.Diagnostics;
using System.Security.Cryptography;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class TotpService : ITotpService
    {
        private const int stepInSeconds = 30;
        private const int codeLength = 6;



        public string ComputeCode(string base32Secret)
        {
            var byteSecret = Base32.Rfc4648.Decode(base32Secret);
            var totp = new Totp(byteSecret,
                                step: stepInSeconds,
                                mode: OtpHashMode.Sha256,
                                totpSize: codeLength);

            return totp.ComputeTotp();
        }

        public string GenerateSecret()
        {
            var bytes = new byte[20];
            RandomNumberGenerator.Fill(bytes);
            return Base32.Rfc4648.Encode(bytes);
        }

        public bool VerifyCode(string base32Secret, string code)
        {
            var byteSecret = Base32.Rfc4648.Decode(base32Secret);
            var totp = new Totp(byteSecret,
                                step: stepInSeconds,
                                mode: OtpHashMode.Sha256,
                                totpSize: codeLength);

            var window = new VerificationWindow(previous: 2, future: 2);
            var result = totp.VerifyTotp(code, out long _, window);
            return result;
        }
    }
}


using FinTrack.API.Core.Interfaces;
using System.Security.Cryptography;

namespace FinTrack.API.Infrastructure.Services
{
    class PBKDF2PasswordHasher : IPasswordHasher
    {
        private const int saltSize = 16;
        private const int keySize = 32;
        private const int iterCount = 50000;
        private const char delimiter = '.';
        private static readonly HashAlgorithmName name = HashAlgorithmName.SHA256;
        public string GetHash(string password)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(saltSize);
            
            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password,
                                                      saltBytes,
                                                      iterCount,
                                                      name,
                                                      keySize);

            var hash = Convert.ToBase64String(hashBytes);
            var salt = Convert.ToBase64String(saltBytes);
            var hashString = $"{name.Name}{delimiter}{iterCount}{delimiter}{salt}{delimiter}{hash}";
            return hashString;
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            string[] segmetns = hashedPassword.Split();
            byte[] salt = Convert.FromBase64String(segmetns[2]);
            byte[] originHash = Convert.FromBase64String(segmetns[3]);
            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(password,
                                                      salt,
                                                      iterCount,
                                                      name,
                                                      keySize);

            

            return CryptographicOperations.FixedTimeEquals(originHash, hashBytes);
        }


        
    }
}

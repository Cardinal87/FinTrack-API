
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace FinTrack.API.Infrastructure.Identity
{
    public class JwtKeyService
    {
        private readonly object _lock = new object();
        private byte[] _key;

        public JwtKeyService()
        {
            GenerateKey();
        }


        [MemberNotNull(nameof(_key))]
        public void GenerateKey()
        {
            lock (_lock)
            {
                var key = new byte[64];
                var rng = RandomNumberGenerator.Create();
                rng.GetBytes(key);
                _key = key;
            }
        }

        public byte[] GetKey()
        {
            lock (_lock)
            {
                return _key;
            }
        } 

    }
}

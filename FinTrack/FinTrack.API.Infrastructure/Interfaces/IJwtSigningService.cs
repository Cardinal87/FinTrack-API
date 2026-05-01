
namespace FinTrack.API.Infrastructure.Interfaces
{
    public interface IJwtSigningService
    {
        /// <summary>
        /// Method using for signing jwt tokens
        /// </summary>
        /// <param name="rawToken">Raw token in header.payload format encoded in base64url</param>
        /// <returns>Signed jwt token in base64url encoding</returns>
        Task<string> SignTokenAsync(string rawToken);


        /// <summary>
        /// Method using to verify provided jwt token
        /// </summary>
        /// <param name="token">Jwt token in base64 encoding</param>
        /// <returns><see langword="true"/> if sign valid or <see langword="false"/> if not</returns>
        Task<bool> VerifyTokenAsync(string token);

    }
}

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;

namespace FinTrack.API.Application.Interfaces
{
    /// <summary>
    /// Represents service for generating JWT tokens
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates JWT token based on user data
        /// </summary>
        /// <param name="user">User domain model</param>
        /// <param name="challenge">Defines whether it be auth challenge or access token (Default: false)</param>
        /// <returns><see cref="TokenGenerationResult"/> with token string and expiration time in seconds</returns>
        public Task<TokenGenerationResult> GenerateTokenAsync(User user, bool challenge = false);
    }
}

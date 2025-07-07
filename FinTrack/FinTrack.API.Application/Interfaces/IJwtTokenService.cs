
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
        /// <returns>JWT token in base64 format</returns>
        public string GenerateToken(User user, IEnumerable<string> roles);
    }
}

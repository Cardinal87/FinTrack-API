
namespace FinTrack.API.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Method for generating new refresh token
        /// </summary>
        /// <param name="userId">ID of the user to whom the token was issued</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>
        /// Returns generated refresh token as <see cref="string"/>
        /// </returns>
        Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default);

        /// <summary>
        /// Method for refresh token rotation 
        /// </summary>
        /// <param name="refreshToken">token for rotation</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>
        /// Returns new refresh token <see cref="string"/> and operation status as <see cref="bool"></see>.
        /// If false, returned token <see cref="string"/> is <see langword="null"/>
        /// </returns>
        Task<(string?, bool)> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

        /// <summary>
        /// Method for revoking provided refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token that will be revoked</param>
        /// <param name="ct">Cancallation token</param>
        /// <returns></returns>
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}

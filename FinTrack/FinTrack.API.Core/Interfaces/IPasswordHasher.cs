

namespace FinTrack.API.Core.Interfaces
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Creates a password hash
        /// </summary>
        /// <param name="password">clear password</param>
        /// <returns>
        /// Formatted string containing hash parameters.
        /// Fotmat: {Hash algorithm}.{Iteration count}.{Salt in Base64}.{Hash in Base64}
        /// </returns>
        public string GetHash(string password);


        /// <summary>
        /// Checks if a password matches the hash
        /// </summary>
        /// <param name="hashedPassword">result of the <see cref="GetHash(string)"/> method</param>
        /// <param name="password">password that would be checked</param>
        /// <returns>
        /// <see langword="true"/> if the password matches the hash.
        /// <see langword="false"/> if password does not match the hash
        /// </returns>
        public bool VerifyPassword(string hashedPassword, string password);
    }
}

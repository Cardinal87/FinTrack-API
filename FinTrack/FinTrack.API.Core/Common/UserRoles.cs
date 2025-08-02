

namespace FinTrack.API.Core.Common
{
    /// <summary>
    /// Static class with user role constants
    /// </summary>
    public static class UserRoles
    {
        public const string User = "User";
        public const string Admin = "Admin";

        /// <summary>
        /// Read-only collection with all possible user roles
        /// </summary>
        public static IReadOnlyCollection<string> AllRoles = [
            User,
            Admin
        ];
    }
}

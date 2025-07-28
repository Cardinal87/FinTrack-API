

namespace FinTrack.API.Core.Common
{
    public static class UserRoles
    {
        public const string User = "User";
        public const string Admin = "Admin";

        public static IReadOnlyCollection<string> AllRoles = [
            User,
            Admin
        ];
    }
}

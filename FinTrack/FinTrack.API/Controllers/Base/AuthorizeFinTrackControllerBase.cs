using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack.API.Controllers.Base
{
    [Authorize]
    public class AuthorizeFinTrackControllerBase : FinTrackContollerBase
    {
        protected Guid UserId => GetCurrentUserGuid();
        protected IReadOnlyCollection<string> UserRoles => GetCurrentUserRoles().AsReadOnly();

        private Guid GetCurrentUserGuid()
        {
            var claim = User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (claim == null) throw new InvalidOperationException("Id claim was not found");
            return Guid.Parse(claim.Value);
        }

        private List<string> GetCurrentUserRoles()
        {
            return User.Claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();
        }
    }
}

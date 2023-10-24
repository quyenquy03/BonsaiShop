using System.Security.Claims;
using System.Security.Principal;

namespace BonsaiShop.Extension
{
    public static class IdentityExtension
    {
        public static string GetAccountId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AccountId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetRoleId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RoleId");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UserName");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetAvatar(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Avatar");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string GetSpecificClaim (this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claims = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == claimType);
            return (claims != null) ? claims.Value : string.Empty;
        }
    }
}

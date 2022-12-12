using System.Security.Claims;
using System.Security.Principal;

using Microsoft.Identity.Web;

namespace ModilistPortal.Infrastructure.Shared.Exntensions
{
    public static class PrincipleExtensions
    {
        public static Guid GetUserId(this IPrincipal user)
        {
            if (!(user.Identity is ClaimsIdentity claimsIdentity))
            {
                return Guid.Empty;
            }

            if (!claimsIdentity.Claims.Any(x => x.Type == ClaimConstants.ObjectId))
            {
                return Guid.Empty;
            }

            return Guid.Parse(((ClaimsIdentity)user.Identity).Claims.FirstOrDefault(x => x.Type == ClaimConstants.ObjectId).Value);
        }
    }
}

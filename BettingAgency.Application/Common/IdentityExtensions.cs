using System.Security.Claims;

namespace BettingAgency.Application.Common;

public static class IdentityExtensions
{
    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        if (principal == null)
            return null;

        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }
}
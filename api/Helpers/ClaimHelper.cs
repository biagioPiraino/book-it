using System.Security.Claims;

namespace BookingSystem.Api.Helpers;

public static class ClaimHelper
{
    public static string GetUserIdentifier(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return userId;
    }
}
using System.Security.Claims;

namespace PhytoIntellect.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdStr, out int userId))
            return userId;

        throw new UnauthorizedAccessException("User ID not found in token.");
    }
}
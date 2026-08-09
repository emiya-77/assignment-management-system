using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(
            ClaimTypes.NameIdentifier
        )?.Value;

        if (!int.TryParse(userId, out var id))
        {
            throw new UnauthorizedAccessException(
                "User ID claim is missing or invalid."
            );
        }

        return id;
    }
}
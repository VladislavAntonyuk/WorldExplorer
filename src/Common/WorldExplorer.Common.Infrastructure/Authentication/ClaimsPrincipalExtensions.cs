namespace WorldExplorer.Common.Infrastructure.Authentication;

using System.Security.Claims;
using Application.Exceptions;
using Microsoft.Identity.Web;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.GetObjectId();

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new WorldExplorerException("User identifier is unavailable");
    }
}

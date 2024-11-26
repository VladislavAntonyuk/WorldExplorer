namespace WorldExplorer.Common.Infrastructure.Authentication;

using System.Security.Claims;
using Application.Exceptions;
using Microsoft.Identity.Web;

public static class ClaimsPrincipalExtensions
{
	public static Guid GetUserId(this ClaimsPrincipal? principal)
	{
		var userId = principal?.GetObjectId();

		return Guid.TryParse(userId, out var parsedUserId)
			? parsedUserId
			: throw new WorldExplorerException("User identifier is unavailable");
	}
}
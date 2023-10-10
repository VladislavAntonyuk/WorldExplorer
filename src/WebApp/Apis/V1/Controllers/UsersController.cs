namespace WebApp.Apis.V1.Controllers;

using global::Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

public class UsersController(ICurrentUserService currentUserService, IUserService userService) : ApiAuthControllerBase
{
	[HttpGet("self")]
	public Task<User?> GetCurrentUser(CancellationToken cancellationToken)
	{
		var currentUser = currentUserService.GetCurrentUser();
		return userService.GetUser(currentUser.ProviderId, cancellationToken);
	}

	[HttpDelete("self")]
	public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
	{
		var currentUserId = currentUserService.GetCurrentUser().ProviderId;
		await userService.DeleteUser(currentUserId, cancellationToken);
		return NoContent();
	}
}
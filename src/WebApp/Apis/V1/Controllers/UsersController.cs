namespace WebApp.Apis.V1.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;
using Shared.Models;
using WebApp.Services.User;

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
namespace WebApp.Apis.V1.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.User;
using Shared.Models;

public class UsersController(ICurrentUserService currentUserService, IUserService userService) : ApiAuthControllerBase
{
	[HttpGet("self")]
	public Task<User?> GetCurrentUser(CancellationToken cancellationToken)
	{
		var currentUser = currentUserService.GetCurrentUser();
		return userService.GetUser(currentUser.ProviderId, cancellationToken);
	}

	[HttpPut("self")]
	public async Task<IActionResult> UpdateCurrentUser(User user, CancellationToken cancellationToken)
	{
		var currentUserId = currentUserService.GetCurrentUser().ProviderId;
		if (currentUserId != user.Id)
		{
			return BadRequest();
		}

		await userService.UpdateUser(user, cancellationToken);
		return Ok();
	}

	[HttpDelete("self")]
	public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
	{
		var currentUserId = currentUserService.GetCurrentUser().ProviderId;
		await userService.DeleteUser(currentUserId, cancellationToken);
		return NoContent();
	}
}
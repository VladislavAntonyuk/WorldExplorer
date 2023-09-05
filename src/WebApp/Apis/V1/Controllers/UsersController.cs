namespace WebApp.Apis.V1.Controllers;

using global::Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

public class UsersController : ApiAuthControllerBase
{
	private readonly ICurrentUserService currentUserService;
	private readonly IUserService userService;

	public UsersController(ICurrentUserService currentUserService, IUserService userService)
	{
		this.currentUserService = currentUserService;
		this.userService = userService;
	}

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
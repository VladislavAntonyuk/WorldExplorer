namespace WebApp.Apis.V1.Controllers;

using global::Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

public class UsersController : ApiAuthControllerBase
{
	private readonly ICurrentUserService currentUserService;
	private readonly IGraphClientService graphClientService;

	public UsersController(ICurrentUserService currentUserService, IGraphClientService graphClientService)
	{
		this.currentUserService = currentUserService;
		this.graphClientService = graphClientService;
	}

	[HttpGet("self")]
	public User GetCurrentUser()
	{
		var currentUser = currentUserService.GetCurrentUser();
		return new User
		{
			Email = currentUser.Email,
			Name = currentUser.Name,
			VisitedPlaces = new List<Place>
			{
				new()
				{
					Name = "Place1",
					Location = new Location(38, 45),
					Images = new List<string>
					{
						"https://upload.wikimedia.org/wikipedia/commons/thumb/1/11/Test-Logo.svg/783px-Test-Logo.svg.png"
					}
				},
				new()
				{
					Name = "Place2 Place2",
					Location = new Location(38, 45),
					Images = new List<string>
					{
						"https://upload.wikimedia.org/wikipedia/commons/thumb/1/11/Test-Logo.svg/783px-Test-Logo.svg.png"
					}
				},
				new()
				{
					Name = "Place3 Place3 Place3",
					Location = new Location(38, 45),
					Images = new List<string>
					{
						"https://upload.wikimedia.org/wikipedia/commons/thumb/1/11/Test-Logo.svg/783px-Test-Logo.svg.png"
					}
				}
			}
		};
	}

	[HttpDelete("self")]
	public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
	{
		var currentUserId = currentUserService.GetCurrentUser().ProviderId;
		await graphClientService.DeleteUser(currentUserId, cancellationToken);
		return NoContent();
	}
}
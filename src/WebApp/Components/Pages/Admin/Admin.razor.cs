namespace WebApp.Components.Pages.Admin;

using Microsoft.AspNetCore.Components;
using Services.Place;
using Services.User;
using Shared.Models;
using WebApp.Components;

public partial class Admin : WorldExplorerAuthBaseComponent
{
	private List<Place> places = [];
	private List<User> users = [];

	[Inject]
	public required IPlacesService PlacesService { get; set; }

	[Inject]
	public required IUserService UsersService { get; set; }

	private Task ClearPlaces()
	{
		return PlacesService.ClearPlaces(CancellationToken.None);
	}

	private Task ClearRequests()
	{
		return PlacesService.ClearRequests(CancellationToken.None);
	}

	private async Task GetUsers()
	{
		users = await UsersService.GetUsers(CancellationToken.None);
	}

	private async Task GetPlaces()
	{
		places = await PlacesService.GetPlaces(CancellationToken.None);
	}

	private async Task DeleteUser(string userId)
	{
		await UsersService.DeleteUser(userId, CancellationToken.None);
		await GetUsers();
	}

	private async Task DeletePlace(Guid placeId)
	{
		await PlacesService.Delete(placeId, CancellationToken.None);
		await GetPlaces();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await GetPlaces();
		await GetUsers();
	}
}
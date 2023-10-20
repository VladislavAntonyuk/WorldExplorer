namespace WebApp.Components.Pages.Admin;

using Microsoft.AspNetCore.Components;
using Services;
using Shared.Models;
using WebApp.Services.Place;
using WebApp.Services.User;

public partial class Admin : WorldExplorerAuthBaseComponent
{
	private List<Place> places = new();
	private List<User> users = new();

	[Inject]
	public required IPlacesService PlacesService { get; set; }

	[Inject]
	public required IUserService UsersService { get; set; }

	private Task ClearPlaces()
	{
		return PlacesService.ClearPlaces(CancellationToken.None);
	}

	private async Task GetUsers()
	{
		users = await UsersService.GetUsers(CancellationToken.None);
	}

	private async Task GetPlaces()
	{
		places = await PlacesService.GetPlaces(CancellationToken.None);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await GetPlaces();
		await GetUsers();
	}
}
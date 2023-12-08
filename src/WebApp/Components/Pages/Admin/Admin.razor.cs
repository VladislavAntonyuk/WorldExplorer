namespace WebApp.Components.Pages.Admin;

using Infrastructure.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services.Place;
using Services.User;
using Place = Shared.Models.Place;
using User = Shared.Models.User;

public partial class Admin : WorldExplorerAuthBaseComponent
{
	private List<Place> places = [];
	private List<User> users = [];
	private List<LocationInfoRequest> requests = [];

	[Inject]
	public required IPlacesService PlacesService { get; set; }

	[Inject]
	public required ILocationInfoRequestsService LocationInfoRequestsService { get; set; }

	[Inject]
	public required IUserService UsersService { get; set; }
	[Inject]
	public required IDialogService DialogService { get; set; }

	private async Task ClearPlaces()
	{
		var isConfirmed = await DialogService.ShowMessageBox("Clear places",
															  "Are you sure you want to clear all places?");
		if (isConfirmed == true)
		{
			await PlacesService.ClearPlaces(CancellationToken.None);
			await GetPlaces();
		}
	}

	private async Task ClearRequests()
	{
		var isConfirmed = await DialogService.ShowMessageBox("Clear requests",
															 "Are you sure you want to clear all requests?");
		if (isConfirmed == true)
		{
			await LocationInfoRequestsService.Clear(CancellationToken.None);
			await GetRequests();
		}
	}

	private async Task GetUsers()
	{
		users = await UsersService.GetUsers(CancellationToken.None);
	}

	private async Task GetPlaces()
	{
		places = await PlacesService.GetPlaces(CancellationToken.None);
	}

	private async Task GetRequests()
	{
		requests = await LocationInfoRequestsService.GetRequests(CancellationToken.None);
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

	private async Task DeleteRequest(int requestId)
	{
		await LocationInfoRequestsService.Delete(requestId, CancellationToken.None);
		await GetRequests();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await GetPlaces();
		await GetUsers();
		await GetRequests();
	}
}
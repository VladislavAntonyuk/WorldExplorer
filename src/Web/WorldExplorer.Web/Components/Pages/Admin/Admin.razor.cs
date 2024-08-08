namespace WorldExplorer.Web.Components.Pages.Admin;

using Microsoft.AspNetCore.Components;
using Modules.Places.Application.Places.GetPlace;
using MudBlazor;
using WorldExplorer.Modules.Users.Application.Users.GetUser;
using WorldExplorer.Web.Components;

public partial class Admin(WorldExplorerApiClient apiClient, IDialogService dialogService) : WorldExplorerAuthBaseComponent
{
	private List<PlaceResponse> places = [];
	private List<UserResponse> users = [];
	private List<LocationInfoRequestResponse> requests = [];

	[Inject]
	public required IDialogService DialogService { get; set; }

	private async Task ClearPlaces()
	{
		var isConfirmed = await DialogService.ShowMessageBox("Clear places",
															  "Are you sure you want to clear all places?");
		if (isConfirmed == true)
		{
			await apiClient.ClearPlaces(CancellationToken.None);
			await GetPlaces();
		}
	}

	private async Task ClearRequests()
	{
		var isConfirmed = await DialogService.ShowMessageBox("Clear requests",
															 "Are you sure you want to clear all requests?");
		if (isConfirmed == true)
		{
			await apiClient.ClearLocationInfoRequests(CancellationToken.None);
			await GetRequests();
		}
	}

	private async Task GetUsers()
	{
		users = await apiClient.GetUsers(CancellationToken.None);
	}

	private async Task GetPlaces()
	{
		places = await apiClient.GetPlaces(CancellationToken.None);
	}

	private async Task GetRequests()
	{
		requests = await apiClient.GetLocationInfoRequests(CancellationToken.None);
	}

	private async Task DeleteUser(Guid userId)
	{
		await apiClient.DeleteUser(userId, CancellationToken.None);
		await GetUsers();
	}

	private async Task DeletePlace(Guid placeId)
	{
		await apiClient.DeletePlace(placeId, CancellationToken.None);
		await GetPlaces();
	}

	private async Task DeleteRequest(Guid requestId)
	{
		await apiClient.DeleteLocationInfoRequest(requestId, CancellationToken.None);
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
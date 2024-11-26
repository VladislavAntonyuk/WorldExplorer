namespace WorldExplorer.Web.Components.Pages.Admin;

using Modules.Places.Application.Places.GetPlace;
using Modules.Users.Application.Users.GetUser;
using MudBlazor;
using WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

public partial class Admin(WorldExplorerApiClient apiClient, IDialogService dialogService)
	: WorldExplorerAuthBaseComponent
{
	private MudTable<UserResponse> usersTable;
	private MudTable<PlaceResponse> placesTable;
	private MudTable<LocationInfoRequestResponse> requestsTable;

	private async Task ClearPlaces()
	{
		var isConfirmed = await dialogService.ShowMessageBox("Clear places",
		                                                     "Are you sure you want to clear all places?");
		if (isConfirmed == true)
		{
			await apiClient.ClearPlaces(CancellationToken.None);
			await placesTable.ReloadServerData();
		}
	}

	private async Task ClearRequests()
	{
		var isConfirmed = await dialogService.ShowMessageBox("Clear requests",
		                                                     "Are you sure you want to clear all requests?");
		if (isConfirmed == true)
		{
			await apiClient.ClearLocationInfoRequests(CancellationToken.None);
			await requestsTable.ReloadServerData();
		}
	}

	private async Task<TableData<UserResponse>> GetUsers(TableState tableState, CancellationToken cancellationToken)
	{
		var users = await apiClient.GetUsers(cancellationToken);
		return new TableData<UserResponse>()
		{
			Items = users,
			TotalItems = users.Count
		};
	}

	private async Task<TableData<PlaceResponse>> GetPlaces(TableState tableState, CancellationToken cancellationToken)
	{
		var places = await apiClient.GetPlaces(cancellationToken);
		return new TableData<PlaceResponse>()
		{
			Items = places,
			TotalItems = places.Count
		};
	}

	private async Task<TableData<LocationInfoRequestResponse>> GetRequests(TableState tableState, CancellationToken cancellationToken)
	{
		var requests = await apiClient.GetLocationInfoRequests(CancellationToken.None);
		return new TableData<LocationInfoRequestResponse>()
		{
			Items = requests,
			TotalItems = requests.Count
		};
	}

	private async Task DeleteUser(Guid userId)
	{
		await apiClient.DeleteUser(userId, CancellationToken.None);
		await usersTable.ReloadServerData();
	}

	private async Task DeletePlace(Guid placeId)
	{
		await apiClient.DeletePlace(placeId, CancellationToken.None);
		await placesTable.ReloadServerData();
	}

	private async Task DeleteRequest(int requestId)
	{
		await apiClient.DeleteLocationInfoRequest(requestId, CancellationToken.None);
		await requestsTable.ReloadServerData();
	}
}
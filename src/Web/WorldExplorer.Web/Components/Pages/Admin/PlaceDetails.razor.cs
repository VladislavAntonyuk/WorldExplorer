namespace WorldExplorer.Web.Components.Pages.Admin;

using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Modules.Places.Application.Abstractions;
using Modules.Places.Application.Places.GetPlace;
using MudBlazor;
using Services;

public partial class PlaceDetails(WorldExplorerApiClient apiClient, ISnackbar snackbar) : WorldExplorerAuthBaseComponent
{
	private PlaceRequest? place;

	[Parameter]
	public Guid PlaceId { get; set; }

	public Converter<Location, string> Convert => new()
	{
		GetFunc = s => string.IsNullOrEmpty(s) ? new Location(0, 0) : JsonSerializer.Deserialize<Location>(s),
		SetFunc = s => JsonSerializer.Serialize(s)
	};

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var placeResponse = await apiClient.GetPlaceDetails(PlaceId, CancellationToken.None);
		if (placeResponse is null)
		{
			return;
		}

		place = new PlaceRequest
		{
			Description = placeResponse.Description,
			Location = placeResponse.Location,
			Name = placeResponse.Name,
			Images = placeResponse.Images
		};
	}

	private async Task SavePlace()
	{
		if (place is null)
		{
			return;
		}

		await apiClient.UpdatePlace(PlaceId, place, CancellationToken.None);
		snackbar.Add("Saved", Severity.Success);
	}

	private async Task DeletePlace()
	{
		await apiClient.DeletePlace(PlaceId, CancellationToken.None);
		snackbar.Add("Deleted", Severity.Success);
	}

	private void DeleteImage(string image)
	{
		place?.Images.Remove(image);
	}

	private void Back()
	{
		NavigationManager.NavigateTo("/admin");
	}
}
namespace WorldExplorer.Web.Components.Pages.Admin;

using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Modules.Places.Application.Places.GetPlace;
using Modules.Places.Domain.Places;
using MudBlazor;
using WorldExplorer.Web.Components;

public partial class PlaceDetails(WorldExplorerApiClient apiClient) : WorldExplorerAuthBaseComponent
{
	private PlaceResponse? place;

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
		place = await apiClient.GetPlaceDetails(PlaceId, CancellationToken.None);
	}

	private Task SavePlace()
	{
		return place is null ? Task.CompletedTask : apiClient.UpdatePlace(place, CancellationToken.None);
	}

	private Task DeletePlace()
	{
		return apiClient.DeletePlace(PlaceId, CancellationToken.None);
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
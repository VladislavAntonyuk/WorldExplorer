namespace WebApp.Components.Pages.Admin;

using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services.Place;
using Shared.Models;
using Place = Shared.Models.Place;

public partial class PlaceDetails : WorldExplorerAuthBaseComponent
{
	private Place? place;

	[Inject]
	public required IPlacesService PlacesService { get; set; }

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
		place = await PlacesService.GetPlaceDetails(PlaceId, CancellationToken.None);
	}

	private Task SavePlace()
	{
		return place is null ? Task.CompletedTask : PlacesService.UpdatePlace(place, CancellationToken.None);
	}

	private Task DeletePlace()
	{
		return PlacesService.Delete(PlaceId, CancellationToken.None);
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
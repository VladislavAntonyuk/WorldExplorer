namespace WebApp.Pages.Admin;

using global::Shared.Models;
using Microsoft.AspNetCore.Components;
using Services;

public partial class Admin : WorldExplorerAuthBaseComponent
{
	private List<Place> places = new();

	[Inject]
	public required IPlacesService PlacesService { get; set; }

	private Task ClearPlaces()
	{
		return PlacesService.ClearPlaces(CancellationToken.None);
	}

	private async Task GetPlaces()
	{
		places = await PlacesService.GetPlaces(CancellationToken.None);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await GetPlaces();
	}
}
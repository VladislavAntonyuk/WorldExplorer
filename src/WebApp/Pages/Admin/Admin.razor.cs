namespace WebApp.Pages.Admin;

using global::Shared.Models;
using Microsoft.AspNetCore.Components;
using WebApp.Services;

public partial class Admin : WorldExplorerAuthBaseComponent
{
	[Inject]
	public required IPlacesService PlacesService { get; set; }

	private List<Place> places = new();

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
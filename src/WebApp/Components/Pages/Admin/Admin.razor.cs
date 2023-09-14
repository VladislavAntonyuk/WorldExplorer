namespace WebApp.Components.Pages.Admin;

using Microsoft.AspNetCore.Components;
using Shared.Models;
using WebApp.Services;

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
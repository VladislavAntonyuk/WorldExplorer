namespace WebApp.Pages.Admin;
using Microsoft.AspNetCore.Components;
using WebApp.Services;

public partial class Admin : WorldExplorerAuthBaseComponent
{
	[Inject]
	public required IPlacesService PlacesService { get; set; }

	private Task ClearPlaces()
	{
		return PlacesService.ClearPlaces(CancellationToken.None);
	}
}
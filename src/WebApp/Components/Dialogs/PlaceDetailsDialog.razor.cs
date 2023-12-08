namespace WebApp.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Services.Place;
using Shared.Models;

public partial class PlaceDetailsDialog : BaseDialog
{
	private Place? place;

	[Parameter]
	public required Guid PlaceId { get; set; }

	[Inject]
	public required IPlacesService PlacesService { get; set; }

	[Inject]
	public required ISnackbar Snackbar { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		do
		{
			place = await PlacesService.GetPlaceDetails(PlaceId, CancellationToken.None);
			if (place is null)
			{
				Snackbar.Add($"{MudDialog?.Title} {Translation.NotFound}", Severity.Error);
				MudDialog?.Close();
				break;
			}

			if (string.IsNullOrWhiteSpace(place.Description))
			{
				await Task.Delay(TimeSpan.FromSeconds(10));
			}
		} while (string.IsNullOrWhiteSpace(place?.Description));
	}
}
namespace WorldExplorer.Web.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using Modules.Places.Application.Places.GetPlace;
using MudBlazor;

public partial class PlaceDetailsDialog(WorldExplorerApiClient apiClient, ISnackbar snackbar) : BaseDialog
{
	private PlaceResponse? place;

	[Parameter]
	public required Guid PlaceId { get; set; }
	
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		do
		{
			place = await apiClient.GetPlaceDetails(PlaceId, CancellationToken.None);
			if (place is null)
			{
				snackbar.Add($"{MudDialog?.Title} Translation.NotFound", Severity.Error);
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
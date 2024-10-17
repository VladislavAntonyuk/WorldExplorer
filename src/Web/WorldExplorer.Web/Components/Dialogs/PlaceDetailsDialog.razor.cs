namespace WorldExplorer.Web.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using Modules.Places.Application.Places.GetPlace;
using MudBlazor;
using StrawberryShake;

public partial class PlaceDetailsDialog(
	WorldExplorerApiClient apiClient,
	IWorldExplorerTravellersClient travellersClient,
	ISnackbar snackbar) : BaseDialog
{
	private PlaceResponse? place;
	private IReadOnlyCollection<ReviewResponse> reviews = [];

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
			else
			{
				var reviewResponse = await travellersClient.GetTravellers.ExecuteAsync();
				if (reviewResponse.IsSuccessResult())
				{
					reviews = reviewResponse.Data?.Travellers?.Items?.Select(x => new ReviewResponse
					                        {
						                        Id = x.Id
					                        })
					                        .ToList() ?? [];
				}
			}
		} while (string.IsNullOrWhiteSpace(place?.Description));
	}
}

internal class ReviewResponse
{
	public Guid Id { get; set; }
}
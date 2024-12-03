namespace WorldExplorer.Web.Components.Dialogs;

using Microsoft.AspNetCore.Components;
using Modules.Places.Application.Places.GetPlace;
using MudBlazor;
using StrawberryShake;

public partial class PlaceDetailsDialog(
	WorldExplorerApiClient apiClient,
	IWorldExplorerTravellersClient travellersClient,
	ISnackbar snackbar) : BaseAuthDialog
{
	private PlaceResponse? place;
	private readonly ReviewRequest reviewRequest = new();
	private ICollection<ReviewResponse> reviews = [];
	private int Rating => reviews.Count > 0 ? (int)reviews.Average(x => x.Rating) : 0;

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
				var reviewResponse = await travellersClient.GetVisitsByPlaceId.ExecuteAsync(place.Id);
				if (reviewResponse.IsSuccessResult())
				{
					reviews = reviewResponse.Data?.VisitsByPlaceId?.Items?.Select(x => new ReviewResponse
					{
						Comment = x.Review?.Comment,
						Rating = x.Review?.Rating ?? 0,
						ReviewDate = x.VisitDate,
						Traveller = new TravellerResponse(x.TravellerId, "User")
					})
											.ToList() ?? [];
				}
			}
		} while (string.IsNullOrWhiteSpace(place?.Description));
	}

	private async Task CreateVisit()
	{
		var result = await travellersClient.CreateVisit.ExecuteAsync(PlaceId, Guid.Parse(CurrentUser.ProviderId), reviewRequest.Rating, reviewRequest.Comment);
		if (result.IsSuccessResult())
		{
			snackbar.Add(Translation.AddReviewSuccess, Severity.Success);
			reviews.Add(new ReviewResponse
			{
				Comment = reviewRequest.Comment,
				Rating = reviewRequest.Rating,
				ReviewDate = DateTimeOffset.UtcNow,
				Traveller = new TravellerResponse(Guid.Parse(CurrentUser.ProviderId), "User")
			});
		}
		else
		{
			foreach (var error in result.Errors.Select(x => x.Message))
			{
				snackbar.Add(error, Severity.Error);
			}
		}
	}
}

internal class ReviewRequest
{
	public string? Comment { get; set; }
	public int Rating { get; set; }
}

internal class ReviewResponse
{
	public DateTimeOffset ReviewDate { get; init; }
	public string? Comment { get; init; }
	public double Rating { get; init; }
	public TravellerResponse Traveller { get; init; }
}

internal record TravellerResponse(Guid Id, string DisplayName);
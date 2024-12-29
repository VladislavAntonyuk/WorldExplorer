namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Models;
using Resources.Localization;
using Services;
using Services.API;
using Services.Auth;
using Services.Navigation;
using StrawberryShake;

public sealed partial class PlaceDetailsViewModel(
	ICurrentUserService currentUserService,
	IPlacesApi placesApi,
	IWorldExplorerTravellersClient travellersClient,
	ILauncher launcher,
	IShare share,
	IArService arService,
	IDialogService dialogService,
	INavigationService navigationService) : BasePopupViewModel(navigationService)
{
	private readonly INavigationService navigationService = navigationService;

	[ObservableProperty]
	public partial bool IsLiveViewEnabled { get; private set; }

	[ObservableProperty]
	public partial double Rating { get; set; }

	[ObservableProperty]
	public partial string Comment { get; set; } = string.Empty;

	[ObservableProperty]
	public partial bool IsPlacedLoaded { get; private set; }

	[ObservableProperty]
	public partial Place Place { get; private set; } = Place.Default;

	[ObservableProperty]
	public partial bool IsReviewEnabled { get; private set; }

	private Guid? placeId;

	public override void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		base.ApplyQueryAttributes(query);
		placeId = query["place"] as Guid?;
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		if (placeId is null)
		{
			return;
		}

		do
		{
			var getDetailsResult = await placesApi.GetDetails(placeId.Value, CancellationToken.None);
			if (getDetailsResult.IsSuccessful)
			{
				var placeReviews = await travellersClient.GetVisitsByPlaceId.ExecuteAsync(placeId.Value);
				var reviews = new List<Review>();
				if (placeReviews.IsSuccessResult())
				{
					reviews.AddRange(placeReviews.Data?.VisitsByPlaceId?.Items?.Select(x => new Review
					{
						Comment = x.Review?.Comment,
						Rating = x.Review?.Rating ?? 0,
						ReviewDate = x.VisitDate,
						Traveller = new TravellerResponse(x.Traveller!.Id, x.Traveller.Name)
					}) ?? []);
					var travellerId = currentUserService.GetCurrentUser()?.Id;
					IsReviewEnabled = reviews.All(x => x.Traveller.Id != travellerId);
				}

				Place = new Place
				{
					Id = getDetailsResult.Content.Id,
					Name = getDetailsResult.Content.Name,
					Location = getDetailsResult.Content.Location,
					Description = getDetailsResult.Content.Description,
					Images = getDetailsResult.Content.Images,
					Reviews = reviews
				};
				if (Place.Images.Count > 0 && arService.IsSupported())
				{
					IsLiveViewEnabled = true;
				}
			}
			else
			{
				break;
			}

			if (string.IsNullOrWhiteSpace(Place.Description))
			{
				await Task.Delay(TimeSpan.FromSeconds(10));
			}
		} while (string.IsNullOrWhiteSpace(Place.Description));


		IsPlacedLoaded = true;
		if (Place == Place.Default)
		{
			await dialogService.ToastAsync(Localization.UnableToGetPlaceDetails);
		}
	}

	[RelayCommand]
	private async Task Ar()
	{
		await dialogService.ToastAsync("Opening AR...");
		await ClosePopup();
		await navigationService.NavigateAsync<ArViewModel, ErrorViewModel>(new Dictionary<string, object>
		{
			{
				"images", Place.Images
			}
		});
	}

	[RelayCommand]
	private Task<bool> OpenUrl()
	{
		return launcher.TryOpenAsync(new Uri($"https://google.com/search?q={Place.Name}"));
	}

	[RelayCommand]
	private Task SharePlace()
	{
		return share.RequestAsync(new ShareTextRequest(Localization.SharePlaceText, Place.Name)
		{
			Uri = $"https://google.com/search?q={Place.Name}",
			Subject = Place.Name
		});
	}

	[RelayCommand]
	private async Task BuildRoute()
	{
		var myLocation = await Geolocation.GetLocationAsync();
		if (myLocation is null)
		{
			return;
		}

		if (DeviceInfo.Current.Platform == DevicePlatform.iOS ||
			DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
		{
			// https://developer.apple.com/library/ios/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
			await Launcher.OpenAsync($"http://maps.apple.com/?daddr={myLocation.Latitude},{myLocation.Longitude}&saddr={Place.Location.Latitude},{Place.Location.Longitude}");
		}
		else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
		{
			// opens the 'task chooser' so the user can pick Maps, Chrome or other mapping app
			await Launcher.OpenAsync($"http://maps.google.com/?daddr={myLocation.Latitude},{myLocation.Longitude}&saddr={Place.Location.Latitude},{Place.Location.Longitude}");
		}
		else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
		{
			await Launcher.OpenAsync($"bingmaps:?rtp=adr.{myLocation.Latitude},{myLocation.Longitude}~adr.{Place.Location.Latitude},{Place.Location.Longitude}");
		}
	}

	[RelayCommand]
	private async Task CreateVisit()
	{
		var user = currentUserService.GetCurrentUser();
		if (user is null)
		{
			return;
		}

		if (Rating is < 1 or > 5)
		{
			await dialogService.ToastAsync(Localization.RatingValidation);
			return;
		}

		if (string.IsNullOrWhiteSpace(Comment))
		{
			await dialogService.ToastAsync(Localization.CommentValidation);
			return;
		}

		var result = await travellersClient.CreateVisit.ExecuteAsync(Place.Id, user.Id, (int)Rating, Comment);
		if (result.IsSuccessResult())
		{
			await dialogService.ToastAsync(Localization.AddReviewSuccess);
			IsReviewEnabled = false;
			Place.Reviews.Add(new Review
			{
				Comment = Comment,
				Rating = Rating,
				ReviewDate = DateTime.UtcNow,
				Traveller = new TravellerResponse(user.Id, user.Name)
			});
			OnPropertyChanged(nameof(Place));
		}
		else
		{
			await dialogService.ToastAsync(result.Errors.Select(x => x.Message));
		}
	}
}
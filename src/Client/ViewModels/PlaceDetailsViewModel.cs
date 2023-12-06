namespace Client.ViewModels;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Resources.Localization;
using Services;
using Services.API;
using Services.Navigation;
using Shared.Models;

public sealed partial class PlaceDetailsViewModel(IPlacesApi placesApi,
	ILauncher launcher,
	IShare share,
	IArService arService,
	IDialogService dialogService,
	INavigationService navigationService) : BaseViewModel, IQueryAttributable
{
	private Guid? placeId;

	[ObservableProperty]
	private bool isPlacedLoaded;

	[ObservableProperty]
	private bool isLiveViewEnabled;

	[ObservableProperty]
	private Place place = Place.Default;

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		placeId = query["place"] as Guid?;
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		if (placeId is null)
		{
			return;
		}

		await dialogService.ToastAsync(Localization.LoadingPlaceDetails);
		do
		{
			var getDetailsResult = await placesApi.GetDetails(placeId.Value, CancellationToken.None);
			if (getDetailsResult.IsSuccessStatusCode)
			{
				Place = getDetailsResult.Content;
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
		await navigationService.NavigateAsync<ArViewModel, ErrorViewModel>(new Dictionary<string, object?>
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

		if (DeviceInfo.Current.Platform == DevicePlatform.iOS || DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
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
}
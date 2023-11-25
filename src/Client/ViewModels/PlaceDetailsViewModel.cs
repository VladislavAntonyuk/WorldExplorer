namespace Client.ViewModels;

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

		IsLiveViewEnabled = false;
		await dialogService.ToastAsync(Localization.LoadingPlaceDetails);
		var getDetailsResult = await placesApi.GetDetails(placeId.Value, CancellationToken.None);
		if (getDetailsResult.IsSuccessStatusCode)
		{
			Place = getDetailsResult.Content;
			if (Place.Images.Count > 0 && arService.IsSupported())
			{
				IsLiveViewEnabled = true;
			}
		}

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
	private Task<bool> OpenUrl(string placeName)
	{
		return launcher.TryOpenAsync(new Uri($"https://google.com/search?q={placeName}"));
	}

	[RelayCommand]
	private Task SharePlace(string placeName)
	{
		return share.RequestAsync(new ShareTextRequest(Localization.SharePlaceText, placeName)
		{
			Uri = $"https://google.com/search?q={placeName}",
			Subject = placeName
		});
	}
}
namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extensions;
using Framework;
using Resources.Localization;
using Services;
using Services.Navigation;
using Shared.Models;

public sealed partial class PlaceDetailsViewModel : BaseViewModel, IQueryAttributable, IDisposable
{
	private readonly IArService arService;
	private readonly IDialogService dialogService;
	private readonly HttpClient httpClient;
	private readonly ILauncher launcher;
	private readonly INavigationService navigationService;
	private readonly IPlacesApi placesApi;
	private readonly IShare share;
	private Place? basePlace;

	[ObservableProperty]
	private bool isLiveViewEnabled;

	[ObservableProperty]
	private Place place;

	[ObservableProperty]
	private byte[][] placeImages = [];

	public PlaceDetailsViewModel(IPlacesApi placesApi,
		ILauncher launcher,
		IShare share,
		IArService arService,
		IDialogService dialogService,
		INavigationService navigationService,
		IHttpClientFactory httpClientFactory)
	{
		Place = Place.Default;
		this.placesApi = placesApi;
		this.launcher = launcher;
		this.share = share;
		this.arService = arService;
		this.dialogService = dialogService;
		this.navigationService = navigationService;
		httpClient = httpClientFactory.CreateClient();
	}

	public void Dispose()
	{
		httpClient.Dispose();
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		basePlace = query["place"] as Place;
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		if (basePlace is null)
		{
			return;
		}

		PlaceImages = [];
		IsLiveViewEnabled = false;
		await dialogService.ToastAsync(Localization.LoadingPlaceDetails);
		var getDetailsResult = await placesApi.GetDetails(basePlace.Name, basePlace.Location, CancellationToken.None);
		if (getDetailsResult.IsSuccessStatusCode)
		{
			Place = getDetailsResult.Content;
			LoadImages().AndForget(true);
		}

		if (Place == Place.Default)
		{
			await dialogService.ToastAsync(Localization.UnableToGetPlaceDetails);
		}
	}

	[RelayCommand]
	private async Task Ar()
	{
		if (PlaceImages.Length > 0)
		{
			await navigationService.NavigateAsync<ArViewModel, ErrorViewModel>(new Dictionary<string, object?>
			{
				{
					"images", PlaceImages
				}
			});
		}
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

	private async Task LoadImages()
	{
		var imageTasks = Place.Images.Take(70)
		                      .Select(x=> httpClient.GetByteArrayAsync(x).AndSafe(Array.Empty<byte>()));
		var images = await Task.WhenAll(imageTasks);

		PlaceImages = images.Where(x => x.Length > 0).ToArray();
		if (PlaceImages.Length > 0 && arService.IsSupported())
		{
			IsLiveViewEnabled = true;
		}
	}
}
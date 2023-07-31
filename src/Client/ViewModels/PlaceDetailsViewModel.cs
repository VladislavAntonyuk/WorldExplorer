namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;
using Shared.Models;

public sealed partial class PlaceDetailsViewModel : BaseViewModel, IQueryAttributable, IDisposable
{
	private readonly HttpClient httpClient;
	private readonly ILauncher launcher;
	private readonly INavigationService navigationService;
	private readonly IPlacesApi placesApi;
	private readonly IShare share;
	private Place? basePlace;

	[ObservableProperty]
	private Place? place;

	public PlaceDetailsViewModel(IPlacesApi placesApi,
		ILauncher launcher,
		IShare share,
		INavigationService navigationService)
	{
		this.placesApi = placesApi;
		this.launcher = launcher;
		this.share = share;
		this.navigationService = navigationService;
		httpClient = new HttpClient();
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

		var getDetailsResult = await placesApi.GetDetails(basePlace.Name, basePlace.Location, CancellationToken.None);
		if (getDetailsResult.IsSuccessStatusCode)
		{
			Place = getDetailsResult.Content;
		}
	}

	[RelayCommand]
	private async Task Ar()
	{
		if (Place is not null && Place.Images.Count > 0)
		{
			var imageTasks = Place.Images.Select(httpClient.GetByteArrayAsync);
			var imagesBytes = await Task.WhenAll(imageTasks);
#if IOS
			UIKit.UIApplication.SharedApplication.KeyWindow?.RootViewController?.DismissViewController(true, async () =>
			{
#endif
			await navigationService.NavigateAsync<ArViewModel, ErrorViewModel>(new Dictionary<string, object?>
			{
				{
					"images", imagesBytes.ToList()
				}
			});
#if IOS
			});
#endif
		}
	}

	[RelayCommand]
	private Task OpenUrl(string placeName)
	{
		return launcher.OpenAsync(new Uri($"https://google.com/search?q={placeName}"));
	}

	[RelayCommand]
	private Task SharePlace(string placeName)
	{
		return share.RequestAsync(new ShareTextRequest
		{
			Uri = $"https://google.com/search?q={placeName}",
			Title = placeName,
			Subject = placeName,
			Text = "I'd like to share this place with you"
		});
	}
}
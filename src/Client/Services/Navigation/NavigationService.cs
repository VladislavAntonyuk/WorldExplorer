namespace Client.Services;

using Enums;
using Framework;
using ViewModels;

internal class NavigationService : INavigationService, IDisposable
{
	private readonly IConnectivity connectivity;
	private ShellNavigationState? currentState;

	public NavigationService(IConnectivity connectivity)
	{
		this.connectivity = connectivity;
		connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
	}

	public void Dispose()
	{
		connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
	}

	public Task NavigateAsync<TViewModel, TErrorViewModel>(IDictionary<string, object?>? parameters = null)
		where TViewModel : BaseViewModel where TErrorViewModel : BaseViewModel
	{
		if (connectivity.NetworkAccess != NetworkAccess.Internet)
		{
			return Shell.Current.GoToAsync(BuildRoot<TErrorViewModel>(), true, new Dictionary<string, object>
			{
				{
					"errorCode", ErrorCode.NoInternet
				}
			});
		}

		return parameters is null
			? Shell.Current.GoToAsync(BuildRoot<TViewModel>(), true)
			: Shell.Current.GoToAsync(BuildRoot<TViewModel>(), true, parameters);
	}

	public Task NavigateBackAsync()
	{
		return Shell.Current.GoToAsync("..", true);
	}

	private async void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
	{
		if (e.NetworkAccess == NetworkAccess.Internet)
		{
			await Shell.Current.GoToAsync(currentState ?? BuildRoot<LoadingViewModel>(), true);
			return;
		}

		currentState = Shell.Current.CurrentState;
		if (e.NetworkAccess != NetworkAccess.Internet)
		{
			await Shell.Current.GoToAsync(BuildRoot<ErrorViewModel>(), true, new Dictionary<string, object>
			{
				{
					"errorCode", ErrorCode.NoInternet
				}
			});
		}
	}

	private static string BuildRoot<TViewModel>()
	{
		var uri = new UriBuilder("", GetPagePathForViewModel(typeof(TViewModel)));
		return uri.Uri.OriginalString;
	}

	private static string GetPagePathForViewModel(Type viewModelType)
	{
		if (!ViewModelLocator.Mappings.ContainsKey(viewModelType))
		{
			throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
		}

		return ViewModelLocator.Mappings[viewModelType];
	}
}
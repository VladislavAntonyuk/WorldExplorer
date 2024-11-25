namespace Client.Services.Navigation;

using Framework;
using Models.Enums;
using ViewModels;

internal sealed class NavigationService : INavigationService, IDisposable
{
	private readonly IConnectivity connectivity;
	private NavigationState? currentState;

	public NavigationService(IConnectivity connectivity)
	{
		this.connectivity = connectivity;
		connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
	}

	public void Dispose()
	{
		connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
	}

	public Task NavigateAsync<TViewModel, TErrorViewModel>()
		where TViewModel : BaseViewModel where TErrorViewModel : BaseViewModel
	{
		return NavigateAsync<TViewModel, TErrorViewModel>(new Dictionary<string, object>());
	}

	public async Task NavigateAsync<TViewModel, TErrorViewModel>(IDictionary<string, object> parameters)
		where TViewModel : BaseViewModel where TErrorViewModel : BaseViewModel
	{
		if (connectivity.NetworkAccess != NetworkAccess.Internet)
		{
			await Shell.Current.GoToAsync(BuildRoot<TErrorViewModel>(), true, new Dictionary<string, object>
			{
				{
					"errorCode", ErrorCode.NoInternet
				}
			});
			return;
		}

		var state = BuildRoot<TViewModel>();
		await Shell.Current.GoToAsync(state, true, parameters);

		currentState = new NavigationState(state, parameters);
	}

	public async Task NavigateBackAsync()
	{
		await Shell.Current.GoToAsync("..", true);
		currentState = new NavigationState(Shell.Current.CurrentState.Location, new Dictionary<string, object>());
	}

	private async void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
	{
		if (e.NetworkAccess == NetworkAccess.Internet)
		{
			if (currentState is null)
			{
				await Shell.Current.GoToAsync(BuildRoot<LoadingViewModel>(), true);
			}
			else
			{
				await Shell.Current.GoToAsync(currentState.State, true, currentState.Parameters);
			}
		}
		else
		{
			await Shell.Current.GoToAsync(BuildRoot<ErrorViewModel>(), true, new Dictionary<string, object>
			{
				{
					"errorCode", ErrorCode.NoInternet
				}
			});
		}
	}

	private static Uri BuildRoot<TViewModel>()
	{
		var viewModelType = typeof(TViewModel);
		if (!ViewModelLocator.Mappings.TryGetValue(viewModelType, out var value))
		{
			throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
		}

		return new Uri(value, UriKind.RelativeOrAbsolute);
	}
}
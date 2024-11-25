namespace Client.Services.Navigation;

using Framework;

public interface INavigationService
{
	Task NavigateAsync<TViewModel, TErrorViewModel>()
		where TViewModel : BaseViewModel where TErrorViewModel : BaseViewModel;

	Task NavigateAsync<TViewModel, TErrorViewModel>(IDictionary<string, object> parameters)
		where TViewModel : BaseViewModel where TErrorViewModel : BaseViewModel;

	Task NavigateBackAsync();
}
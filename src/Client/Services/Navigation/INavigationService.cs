namespace Client.Services;

using Framework;

public interface INavigationService
{
	Task NavigateAsync<TViewModel, TErrorViewModel>(IDictionary<string, object?>? parameters = null)
		where TViewModel : BaseViewModel where TErrorViewModel : BaseViewModel;

	Task NavigateBackAsync();
}
namespace Client.ViewModels;

using Framework;
using Services;

public class LoadingViewModel : BaseViewModel
{
	private readonly INavigationService navigation;

	public LoadingViewModel(INavigationService navigation)
	{
		this.navigation = navigation;
	}

	public override async Task InitializeAsync()
	{
		await Task.Delay(100);
		await navigation.NavigateAsync<LoginViewModel, ErrorViewModel>();
	}
}
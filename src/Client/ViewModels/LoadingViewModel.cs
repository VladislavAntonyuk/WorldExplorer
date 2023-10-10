namespace Client.ViewModels;

using Framework;
using Services;

public class LoadingViewModel(INavigationService navigation) : BaseViewModel
{
	public override async Task InitializeAsync()
	{
		await Task.Delay(100);
		await navigation.NavigateAsync<LoginViewModel, ErrorViewModel>();
	}
}
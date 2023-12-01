namespace Client.ViewModels;

using Framework;
using Services.API;
using Services.Navigation;

public class LoadingViewModel(INavigationService navigation, IUsersApi usersApi) : BaseViewModel
{
	public override async Task InitializeAsync()
	{
		var user = await usersApi.GetCurrentUser(CancellationToken.None);
		if (user.IsSuccessStatusCode)
		{
			await navigation.NavigateAsync<ExplorerViewModel, ErrorViewModel>();
		}
		else
		{
			await navigation.NavigateAsync<LoginViewModel, ErrorViewModel>();
		}
	}
}
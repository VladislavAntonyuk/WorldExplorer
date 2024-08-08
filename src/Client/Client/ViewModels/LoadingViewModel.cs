namespace Client.ViewModels;

using CommunityToolkit.Mvvm.Messaging;
using Framework;
using Services.API;
using Services.Auth;
using Services.Navigation;

public class LoadingViewModel(INavigationService navigation, IUsersApi usersApi) : BaseViewModel
{
	public override async Task InitializeAsync()
	{
		var user = await usersApi.GetCurrentUser(CancellationToken.None);
		if (user.IsSuccessStatusCode)
		{
			WeakReferenceMessenger.Default.Send(new UserAuthenticatedEvent(user.Content));
			await navigation.NavigateAsync<ExplorerViewModel, ErrorViewModel>();
		}
		else
		{
			await navigation.NavigateAsync<LoginViewModel, ErrorViewModel>();
		}
	}
}
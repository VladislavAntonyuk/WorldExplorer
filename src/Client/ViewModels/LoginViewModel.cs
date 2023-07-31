namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;
using Services.Auth;
using Views;

public partial class LoginViewModel : BaseViewModel
{
	private readonly IAuthService authService;
	private readonly IDialogService dialogService;
	private readonly INavigationService navigation;

	public LoginViewModel(INavigationService navigation, IAuthService authService, IDialogService dialogService)
	{
		Items = new ObservableCollection<CarouselModel>
		{
			new("language", "Multi language", "Use Draw & GO on your own language!"),
			new("adjust", "Dark theme", "Break edges! Use Draw & GO on Windows, Linux and Android devices!")
		};
		this.navigation = navigation;
		this.authService = authService;
		this.dialogService = dialogService;
	}

	public ObservableCollection<CarouselModel> Items { get; }

	[RelayCommand(AllowConcurrentExecutions = false, IncludeCancelCommand = true)]
	private async Task Login(CancellationToken cancellationToken)
	{
		var token = await authService.SignInInteractively(cancellationToken);
		if (token.IsSuccessful)
		{
			await navigation.NavigateAsync<ExplorerViewModel, ErrorViewModel>();
		}
		else
		{
			await dialogService.ToastAsync(token.Errors.Select(x => x.Description), CancellationToken.None);
		}
	}
}
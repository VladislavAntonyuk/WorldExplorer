namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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
	private Timer? timer;

	public LoginViewModel(INavigationService navigation, IAuthService authService, IDialogService dialogService)
	{
		Items = new ObservableCollection<CarouselModel>
		{
			new(Resources.Localization.Localization.PromoTitle1, Resources.Localization.Localization.PromoText1),
			new(Resources.Localization.Localization.PromoTitle2, Resources.Localization.Localization.PromoText2),
			new(Resources.Localization.Localization.PromoTitle3, Resources.Localization.Localization.PromoText3),
			new(Resources.Localization.Localization.PromoTitle4, Resources.Localization.Localization.PromoText4),
			new(Resources.Localization.Localization.PromoTitle5, Resources.Localization.Localization.PromoText5),
		};
		this.navigation = navigation;
		this.authService = authService;
		this.dialogService = dialogService;
	}

	public override Task InitializeAsync()
	{
		timer = new Timer(_ =>
		{
			if (Position == Items.Count - 1)
			{
				Position = 0;
			}
			else
			{
				Position++;
			}
		}, null, 0, 3000);
		return base.InitializeAsync();
	}

	public override Task UnInitializeAsync()
	{
		timer?.Dispose();
		return base.UnInitializeAsync();
	}

	public ObservableCollection<CarouselModel> Items { get; }

	[ObservableProperty]
	private int position;

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
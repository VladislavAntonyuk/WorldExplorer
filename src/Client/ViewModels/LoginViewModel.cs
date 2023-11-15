namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Models;
using Resources.Localization;
using Services;
using Services.Auth;
using Services.Navigation;

public partial class LoginViewModel(INavigationService navigation,
	IAuthService authService,
	IDialogService dialogService) : BaseViewModel
{
	[ObservableProperty]
	private int position;

	private Timer? timer;

	public ObservableCollection<CarouselModel> Items { get; } = new()
	{
		new(Localization.PromoTitle1, Localization.PromoText1),
		new(Localization.PromoTitle2, Localization.PromoText2),
		new(Localization.PromoTitle3, Localization.PromoText3),
		new(Localization.PromoTitle4, Localization.PromoText4),
		new(Localization.PromoTitle5, Localization.PromoText5)
	};

	public override Task InitializeAsync()
	{
		timer = new Timer(_ =>
		{
			if (Position >= Items.Count - 1)
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
namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Models;
using Resources.Localization;
using Services;
using Services.Auth;

public partial class ProfileViewModel : BaseViewModel
{
	private readonly IAuthService authService;
	private readonly IDialogService dialogService;
	private readonly INavigationService navigationService;
	private readonly IUsersApi usersApi;

	[ObservableProperty]
	private UserDetails? user;

	public ProfileViewModel(IAuthService authService,
		INavigationService navigationService,
		IDialogService dialogService,
		IUsersApi usersApi)
	{
		Title = Localization.Profile;
		this.authService = authService;
		this.navigationService = navigationService;
		this.dialogService = dialogService;
		this.usersApi = usersApi;
	}

	[RelayCommand]
	private async Task Delete(CancellationToken cancellationToken)
	{
		var confirmationResult =
			await dialogService.ConfirmAsync(Localization.DeleteProfile, Localization.DeleteProfileConfirmationText, Localization.Yes, Localization.No);
		if (!confirmationResult)
		{
			return;
		}

		var deleteResult = await usersApi.Delete(cancellationToken);
		if (deleteResult.IsSuccessStatusCode)
		{
			await Logout(cancellationToken);
		}
		else
		{
			await dialogService.ToastAsync(deleteResult.Error.Message, CancellationToken.None);
		}
	}

	[RelayCommand]
	private async Task Logout(CancellationToken cancellationToken)
	{
		await authService.LogoutAsync(cancellationToken);
		await navigationService.NavigateAsync<LoginViewModel, ErrorViewModel>();
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		var getUserResult = await usersApi.GetCurrentUser(CancellationToken.None);
		if (getUserResult.IsSuccessStatusCode)
		{
			User = new UserDetails
			{
				Email = getUserResult.Content.Email,
				Name = getUserResult.Content.Name,
				VisitedPlaces = getUserResult.Content.VisitedPlaces,
				Activities = new List<UserActivity>
				{
					new()
					{
						Date = DateTime.Today,
						Steps = 50
					},
					new()
					{
						Date = new DateTime(2023, 07, 27),
						Steps = 150
					}
				}
			};
		}
		else
		{
			await dialogService.ToastAsync(getUserResult.Error.Message);
		}
	}
}
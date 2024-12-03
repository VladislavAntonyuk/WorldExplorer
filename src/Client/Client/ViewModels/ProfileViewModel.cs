namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Framework;
using Models;
using Resources.Localization;
using Services;
using Services.API;
using Services.Auth;
using Services.Navigation;
using StrawberryShake;

public partial class ProfileViewModel : BaseViewModel
{
	private readonly IAuthService authService;
	private readonly IDialogService dialogService;
	private readonly INavigationService navigationService;
	private readonly IWorldExplorerTravellersClient travellersClient;
	private readonly IUsersApi usersApi;

	[ObservableProperty]
	public partial User? User { get; set; }

	public ProfileViewModel(IAuthService authService,
		INavigationService navigationService,
		IDialogService dialogService,
		IUsersApi usersApi,
		IWorldExplorerTravellersClient travellersClient)
	{
		Title = Localization.Profile;
		this.authService = authService;
		this.navigationService = navigationService;
		this.dialogService = dialogService;
		this.usersApi = usersApi;
		this.travellersClient = travellersClient;
	}

	[RelayCommand]
	private async Task Delete(CancellationToken cancellationToken)
	{
		var confirmationResult = await dialogService.ConfirmAsync(Localization.DeleteProfile,
		                                                          Localization.DeleteProfileConfirmationText,
		                                                          Localization.Yes, Localization.No);
		if (!confirmationResult)
		{
			return;
		}

		var deleteResult = await usersApi.Delete(cancellationToken);
		if (deleteResult.IsSuccessful)
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
		WeakReferenceMessenger.Default.Send(new UserAuthenticatedEvent(null));
		await navigationService.NavigateAsync<LoginViewModel, ErrorViewModel>();
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		var getUserResult = await usersApi.GetCurrentUser(CancellationToken.None);
		if (getUserResult.IsSuccessful)
		{
			User = getUserResult.Content;
			var traveller = await travellersClient.GetTravellerById.ExecuteAsync(User.Id);
			if (traveller.IsSuccessResult())
			{
				User.Activities.AddRange(traveller.Data?.ById?.Visits.GroupBy(x => x.VisitDate)
				                                  .Select(x => new UserActivity
				                                  {
					                                  Date = x.Key.LocalDateTime,
					                                  ReviewedPlacesCount = x.Count()
				                                  }) ?? []);
			}
		}
		else
		{
			await dialogService.ToastAsync(getUserResult.Error.Message);
		}
	}

	[RelayCommand]
	private Task SaveChanges(CancellationToken cancellationToken)
	{
		WeakReferenceMessenger.Default.Send(new UserAuthenticatedEvent(User));
		return User is null
			? Task.CompletedTask
			: usersApi.UpdateCurrentUser(new IUsersApi.UpdateUserRequest
			{
				TrackUserLocation = User.Settings.TrackUserLocation
			}, cancellationToken);
	}
}
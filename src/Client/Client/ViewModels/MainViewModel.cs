namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Framework;
using Services.Auth;
using Services.Navigation;
using Shared.Models;

public static class Constants
{
	public const string ProductName = "World Explorer";
}

public partial class MainViewModel : BaseViewModel, IRecipient<UserAuthenticatedEvent>
{
	private readonly INavigationService navigationService;

	public MainViewModel(INavigationService navigationService)
	{
		this.navigationService = navigationService;
		WeakReferenceMessenger.Default.Register(this);
		Title = Constants.ProductName;
	}

	[ObservableProperty]
	private User? user;
	
	public void Receive(UserAuthenticatedEvent message)
	{
		User = message.User;
	}

	[RelayCommand]
	private Task Profile()
	{
		return navigationService.NavigateAsync<ProfileViewModel, ErrorViewModel>();
	}

	[RelayCommand]
	private Task OpenCamera()
	{
		return navigationService.NavigateAsync<CameraViewModel, ErrorViewModel>();
	}
}
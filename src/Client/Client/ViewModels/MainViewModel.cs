namespace Client.ViewModels;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Framework;
using Services.Auth;
using Services.Navigation;
using Shared.Models;
using Views;

public static class Constants
{
	public const string ProductName = "World Explorer";
}

public partial class MainViewModel : BaseViewModel, IRecipient<UserAuthenticatedEvent>
{
	private readonly ILauncher launcher;
	private readonly INavigationService navigationService;

	[ObservableProperty]
	private User? user;

	public MainViewModel(INavigationService navigationService, ILauncher launcher)
	{
		this.navigationService = navigationService;
		this.launcher = launcher;
		WeakReferenceMessenger.Default.Register(this);
		Title = Constants.ProductName;
	}

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

	[RelayCommand]
	private Task Help()
	{
		return launcher.TryOpenAsync("https://world-explorer.azurewebsites.net/about");
	}

	[RelayCommand]
	private void About()
	{
		Application.Current?.OpenWindow(new Window(new AboutPage(new AboutViewModel())));
	}

	[RelayCommand]
	private async Task Search(string? text)
	{
		await Toast.Make("Work in progress").Show();
	}
}
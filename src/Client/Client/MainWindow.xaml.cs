namespace Client;

using AlohaKit.Controls;
using Client.Services.Auth;
using CommunityToolkit.Mvvm.Messaging;
using Shared.Models;
using ViewModels;

public partial class MainWindow : Window, IRecipient<UserAuthenticatedEvent>
{
	private readonly MainViewModel mainViewModel;

	public MainWindow(MainViewModel mainViewModel)
	{
		InitializeComponent();
		BindingContext = this.mainViewModel = mainViewModel;
		WeakReferenceMessenger.Default.Register(this);
		SetTitleBar(null);
	}

	private void OnProfileTapped(object? sender, TappedEventArgs e)
	{
		mainViewModel.ProfileCommand.Execute(null);
	}

	public void Receive(UserAuthenticatedEvent message)
	{
		SetTitleBar(message.User);
	}

	private void SetTitleBar(User? user)
	{
		Avatar.Name = user?.Name;
		TrailingContent.IsVisible = SearchBar.IsVisible = DeviceInfo.Platform == DevicePlatform.WinUI && user is not null;
	}
}
namespace Client;

using CommunityToolkit.Mvvm.Messaging;
using Services.Auth;
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

	public void Receive(UserAuthenticatedEvent message)
	{
		SetTitleBar(message.User);
	}

	private void OnProfileTapped(object? sender, TappedEventArgs e)
	{
		mainViewModel.ProfileCommand.Execute(null);
	}

	private void SetTitleBar(User? user)
	{
		Avatar.Name = user?.Name;
		Avatar.IsVisible = SearchBar.IsVisible = user is not null;
	}

	private void HelpClicked(object? sender, EventArgs e)
	{
		mainViewModel.HelpCommand.Execute(null);
	}

	private void AboutClicked(object? sender, EventArgs e)
	{
		mainViewModel.AboutCommand.Execute(null);
	}

	private void SearchBar_OnSearchButtonPressed(object? sender, EventArgs e)
	{
		mainViewModel.SearchCommand.Execute(SearchBar.Text);
	}
}
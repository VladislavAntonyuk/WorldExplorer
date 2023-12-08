namespace Client;

using ViewModels;

public partial class App : Application
{
	public App(ShellViewModel viewModel)
	{
		InitializeComponent();

		MainPage = new AppShell(viewModel);
	}
}
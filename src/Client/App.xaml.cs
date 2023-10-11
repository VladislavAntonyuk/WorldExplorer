namespace Client;

using ViewModels;

public partial class App : Application
{
	public App(ShellViewModel viewModel)
	{
		InitializeComponent();

		MainPage = new AppShell(viewModel);
	}

	protected override async void OnStart()
	{
		base.OnStart();
		await Services.DeviceInstallationService.RegisterDevice("drawgo", "p7fcbXEbiLKwdv7uX/XpFCRSmP5AEaxuBLmSOluzXhE=");
	}
}
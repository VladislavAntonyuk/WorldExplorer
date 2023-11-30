namespace Client;

using Shared;
using ViewModels;

public partial class App : Application
{
	private readonly KeysSettings keysSettings;

	public App(ShellViewModel viewModel, KeysSettings keysSettings)
	{
		this.keysSettings = keysSettings;
		InitializeComponent();

		MainPage = new AppShell(viewModel);
	}

	protected override async void OnStart()
	{
		base.OnStart();
		await Services.DeviceInstallationService.RegisterDevice("world-explorer", keysSettings.NotificationsHub);
	}
}
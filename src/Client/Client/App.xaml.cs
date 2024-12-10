namespace Client;

using Microsoft.Extensions.Options;
using ViewModels;

public partial class App : Application
{
	private readonly MainViewModel mainViewModel;
	private readonly ILauncher launcher;
	private readonly IOptions<UrlsSettings> urlOptions;

	public App(MainViewModel mainViewModel, ILauncher launcher, IOptions<UrlsSettings> urlOptions)
	{
		this.mainViewModel = mainViewModel;
		this.launcher = launcher;
		this.urlOptions = urlOptions;
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new MainWindow(mainViewModel);
	}

	protected override async void OnStart()
	{
		base.OnStart();
		// Run AppService if it is stopped
		await launcher.TryOpenAsync(urlOptions.Value.Api);
	}
}
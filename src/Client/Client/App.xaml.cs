namespace Client;

using ViewModels;

public partial class App : Application
{
	private readonly MainViewModel mainViewModel;

	public App(MainViewModel mainViewModel)
	{
		this.mainViewModel = mainViewModel;
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new MainWindow(mainViewModel);
	}
}
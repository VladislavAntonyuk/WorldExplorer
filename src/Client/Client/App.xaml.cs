namespace Client;

using ViewModels;

public partial class App : Application
{
	private readonly ShellViewModel viewModel;

	public App(ShellViewModel viewModel)
	{
		this.viewModel = viewModel;
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell(viewModel));
	}
}
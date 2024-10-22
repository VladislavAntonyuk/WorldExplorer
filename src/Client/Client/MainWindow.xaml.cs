namespace Client;

using ViewModels;

public partial class MainWindow : Window
{
	public MainWindow(ShellViewModel viewModel)
	{
		InitializeComponent();
		//MainShell.BindingContext = viewModel;
	}
}
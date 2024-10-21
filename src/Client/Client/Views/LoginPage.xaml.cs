namespace Client.Views;

using Framework;
using ViewModels;

public partial class LoginPage : BaseContentPage<LoginViewModel>
{
	public LoginPage(LoginViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
		return false;
	}
}
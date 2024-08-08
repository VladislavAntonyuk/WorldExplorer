namespace Client.Views;

using Framework;
using ViewModels;

public partial class ErrorPage : BaseContentPage<ErrorViewModel>
{
	public ErrorPage(ErrorViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}
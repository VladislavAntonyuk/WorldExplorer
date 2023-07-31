namespace Client.Views;

using Framework;
using ViewModels;

public partial class LoadingPage : BaseContentPage<LoadingViewModel>
{
	public LoadingPage(LoadingViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
		return false;
	}
}
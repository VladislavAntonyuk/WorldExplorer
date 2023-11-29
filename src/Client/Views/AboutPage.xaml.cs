namespace Client.Views;

using Framework;
using ViewModels;

public partial class AboutPage : BaseContentPage<AboutViewModel>
{
	public AboutPage(AboutViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
namespace Client.Views;

using Framework;
using ViewModels;

public partial class CameraPage : BaseContentPage<CameraViewModel>
{
	public CameraPage(CameraViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
namespace Client.Views;

using Framework;
using ViewModels;

public partial class ArPage : BaseContentPage<ArViewModel>
{
	public ArPage(ArViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		arView.Handler?.DisconnectHandler();
	}
}
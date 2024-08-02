namespace Client.Views;

using CommunityToolkit.Maui.Core.Primitives;
using Framework;
using ViewModels;

public partial class CameraPage : BaseContentPage<CameraViewModel>
{
	public CameraPage(CameraViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var camera = await CameraView.GetAvailableCameras(CancellationToken.None);
		CameraView.SelectedCamera = camera.FirstOrDefault(x => x.Position != CameraPosition.Front);
		await CameraView.StartCameraPreview(CancellationToken.None);
	}

	protected override void OnDisappearing()
	{
		CameraView.StopCameraPreview();
		base.OnDisappearing();
	}
}
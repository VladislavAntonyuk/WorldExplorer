namespace Client.ViewModels;

using Camera.MAUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;
using Services.Navigation;

public partial class CameraViewModel(INavigationService navigationService,
	IDispatcher dispatcher,
	IDialogService dialogService,
	IDeviceDisplay deviceDisplay) : BaseViewModel
{
	[ObservableProperty]
	private bool isCameraLoaded;

	public override async Task InitializeAsync()
	{
		deviceDisplay.KeepScreenOn = true;
		var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
		if (cameraPermissionStatus == PermissionStatus.Granted)
		{
			CameraView.Current.CamerasLoaded += CameraView_CamerasLoaded;
			await base.InitializeAsync();
		}
		else
		{
			await Close();
		}
	}

	public override Task UnInitializeAsync()
	{
		deviceDisplay.KeepScreenOn = false;
		CameraView.Current.CamerasLoaded -= CameraView_CamerasLoaded;
		return base.UnInitializeAsync();
	}

	private async void CameraView_CamerasLoaded(object? sender, EventArgs e)
	{
		if (CameraView.Current.NumCamerasDetected > 0)
		{
			CameraView.Current.Camera = CameraView.Current.Cameras[0];
			await dispatcher.DispatchAsync(async () =>
			{
				var result = await CameraView.Current.StartCameraAsync();
				if (result is CameraResult.Success)
				{
					IsCameraLoaded = true;
				}
				else
				{
					await dialogService.ToastAsync($"Camera error: {result.ToString()}");
					await Close();
				}
			});
		}
	}

	[RelayCommand]
	private async Task Photo()
	{
		var stream = await CameraView.Current.TakePhotoAsync();
		if (stream != null)
		{
			await SaveImage(stream);
		}
	}

	[RelayCommand]
	private Task Close()
	{
		return navigationService.NavigateBackAsync();
	}

	private async Task SaveImage(Stream stream)
	{
		var savePermissionStatus = OperatingSystem.IsAndroidVersionAtLeast(33) ? PermissionStatus.Granted : await Permissions.RequestAsync<Permissions.StorageWrite>();
		if (savePermissionStatus == PermissionStatus.Granted)
		{
			await stream.SaveAsImage(async error =>
			{
				await dialogService.ToastAsync(error);
			});
		}
	}
}
namespace Client.ViewModels;

using Camera.MAUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;

public partial class CameraViewModel : BaseViewModel
{
	private readonly IDialogService dialogService;
	private readonly IDeviceDisplay deviceDisplay;
	private readonly IDispatcher dispatcher;
	private readonly INavigationService navigationService;

	[ObservableProperty]
	private bool isCameraLoaded;

	public CameraViewModel(INavigationService navigationService, IDispatcher dispatcher, IDialogService dialogService, IDeviceDisplay deviceDisplay)
	{
		this.navigationService = navigationService;
		this.dispatcher = dispatcher;
		this.dialogService = dialogService;
		this.deviceDisplay = deviceDisplay;
	}

	public override async Task InitializeAsync()
	{
		deviceDisplay.KeepScreenOn = true;
		var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
		if (cameraPermissionStatus == PermissionStatus.Granted)
		{
			CameraView.Current.CamerasLoaded += CameraView_CamerasLoaded;
		}

		await base.InitializeAsync();
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
			CameraView.Current.Camera = CameraView.Current.Cameras.First();
			await dispatcher.DispatchAsync(async () =>
			{
				if (await CameraView.Current.StartCameraAsync() == CameraResult.Success)
				{
					IsCameraLoaded = true;
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
		var savePermissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
		if (savePermissionStatus != PermissionStatus.Granted)
		{
			return;
		}
#if ANDROID || IOS
		await stream.SaveAsImage(async error =>
		{
			await dialogService.ToastAsync(error);
		});
#endif
	}
}
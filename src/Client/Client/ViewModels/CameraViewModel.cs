namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;
using Services.Navigation;

public partial class CameraViewModel(INavigationService navigationService,
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
			IsCameraLoaded = true;
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
		return base.UnInitializeAsync();
	}


	[RelayCommand]
	private async Task Photo(Stream? stream)
	{
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
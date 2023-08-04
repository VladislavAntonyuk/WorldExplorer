namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;

public partial class ArViewModel : BaseViewModel, IQueryAttributable
{
	private readonly INavigationService navigationService;
	private readonly IDeviceDisplay deviceDisplay;

	public ArViewModel(INavigationService navigationService, IDeviceDisplay deviceDisplay)
	{
		this.navigationService = navigationService;
		this.deviceDisplay = deviceDisplay;
		Images = new();
	}

	public override Task InitializeAsync()
	{
		deviceDisplay.KeepScreenOn = true;
		return base.InitializeAsync();
	}

	public override Task UnInitializeAsync()
	{
		deviceDisplay.KeepScreenOn = false;
		return base.UnInitializeAsync();
	}

	public ObservableCollection<byte[]> Images { get; private set; }

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("images", out var imagesObject) && imagesObject is ObservableCollection<byte[]> images)
		{
			Images = images;
		}
	}

	[RelayCommand]
	private Task Close()
	{
		return navigationService.NavigateBackAsync();
	}
}
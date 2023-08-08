namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;

public partial class ArViewModel : BaseViewModel, IQueryAttributable
{
	private readonly IDeviceDisplay deviceDisplay;
	private readonly INavigationService navigationService;

	public ArViewModel(INavigationService navigationService, IDeviceDisplay deviceDisplay)
	{
		this.navigationService = navigationService;
		this.deviceDisplay = deviceDisplay;
		Images = new ObservableCollection<byte[]>();
	}

	public ObservableCollection<byte[]> Images { get; }

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("images", out var imagesObject) && imagesObject is byte[][] images)
		{
			Images.Clear();
			foreach (var image in images)
			{
				Images.Add(image);
			}
		}
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

	[RelayCommand]
	private Task Close()
	{
		return navigationService.NavigateBackAsync();
	}
}
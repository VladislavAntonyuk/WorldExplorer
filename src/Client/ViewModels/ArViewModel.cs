namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services.Navigation;

public partial class ArViewModel
	(INavigationService navigationService, IDeviceDisplay deviceDisplay) : BaseViewModel, IQueryAttributable
{
	public ObservableCollection<byte[]> Images { get; } = [];

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
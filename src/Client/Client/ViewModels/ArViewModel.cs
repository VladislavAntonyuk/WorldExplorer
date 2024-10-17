namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services.Navigation;

public partial class ArViewModel
	(INavigationService navigationService, IDeviceDisplay deviceDisplay) : BaseViewModel, IQueryAttributable
{
	public ObservableCollection<string> Images { get; } = [];

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.TryGetValue("images", out var imagesObject) && imagesObject is List<string> images)
		{
			Images.Clear();
			foreach (var image in images)
			{
				Images.Add(image);
			}
		}
	}

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();
		deviceDisplay.KeepScreenOn = true;
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
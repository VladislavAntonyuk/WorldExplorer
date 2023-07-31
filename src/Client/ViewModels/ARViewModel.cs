namespace Client.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;

public partial class ArViewModel : BaseViewModel, IQueryAttributable
{
	private readonly INavigationService navigationService;

	public ArViewModel(INavigationService navigationService)
	{
		this.navigationService = navigationService;
		Images = new();
	}

	public ObservableCollection<byte[]> Images { get; }

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey("images"))
		{
			var images = query["images"] as List<byte[]>;
			if (images is null)
			{
				return;
			}

			foreach (var image in images)
			{
				Images.Add(image);
			}
		}
	}

	[RelayCommand]
	private Task Close()
	{
		return navigationService.NavigateBackAsync();
	}
}
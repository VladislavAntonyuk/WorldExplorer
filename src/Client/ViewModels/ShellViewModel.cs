namespace Client.ViewModels;

using CommunityToolkit.Mvvm.Input;
using Framework;
using Services;

public partial class ShellViewModel : BaseViewModel
{
	private readonly INavigationService navigationService;

	public ShellViewModel(INavigationService navigationService)
	{
		this.navigationService = navigationService;
	}

	[RelayCommand]
	private Task OpenCamera()
	{
		return navigationService.NavigateAsync<CameraViewModel, ErrorViewModel>();
	}
}
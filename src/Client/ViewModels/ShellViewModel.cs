namespace Client.ViewModels;

using CommunityToolkit.Mvvm.Input;
using Framework;
using Services.Navigation;

public partial class ShellViewModel(INavigationService navigationService) : BaseViewModel
{
	[RelayCommand]
	private Task OpenCamera()
	{
		return navigationService.NavigateAsync<CameraViewModel, ErrorViewModel>();
	}
}
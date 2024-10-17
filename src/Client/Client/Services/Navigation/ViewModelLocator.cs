namespace Client.Services.Navigation;

using ViewModels;
using Views;

internal static class ViewModelLocator
{
	internal static readonly Dictionary<Type, string> Mappings = new()
	{
		{
			typeof(LoadingViewModel), $"//{nameof(LoadingPage)}"
		},
		{
			typeof(LoginViewModel), $"//{nameof(LoginPage)}"
		},
		{
			typeof(ErrorViewModel), $"//{nameof(ErrorPage)}"
		},
		{
			typeof(ProfileViewModel), $"//home/{nameof(ProfilePage)}"
		},
		{
			typeof(ExplorerViewModel), $"//home/{nameof(ExplorerPage)}"
		},
		{
			typeof(ArViewModel), $"//home/{nameof(ArPage)}"
		},
		{
			typeof(CameraViewModel), $"//home/{nameof(CameraPage)}"
		}
	};
}
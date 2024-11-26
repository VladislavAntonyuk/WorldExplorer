namespace Client.ViewModels;

using Framework;

public class AboutViewModel : BaseViewModel
{
	public static string Version => VersionTracking.CurrentVersion;
}
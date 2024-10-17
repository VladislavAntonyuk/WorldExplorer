namespace Client.ViewModels;

using Framework;

public class AboutViewModel : BaseViewModel
{
	public string Version => VersionTracking.CurrentVersion;
}
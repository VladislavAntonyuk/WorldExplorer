namespace Client;

using Android.App;
using Android.Content.PM;
using Android.OS;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
		  LaunchMode = LaunchMode.SingleTop,
		  ConfigurationChanges = ConfigChanges.ScreenSize |
								 ConfigChanges.Orientation |
								 ConfigChanges.UiMode |
								 ConfigChanges.ScreenLayout |
								 ConfigChanges.SmallestScreenSize |
								 ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override async void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		await DeviceInstallationService.RegisterDevice("drawgo", "p7fcbXEbiLKwdv7uX/XpFCRSmP5AEaxuBLmSOluzXhE=");
	}
}
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.WinUI;

using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using CommunityToolkit.Maui.Alerts;
using Microsoft.UI.Xaml;
using Windows.Networking.PushNotifications;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

/// <summary>
///     Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	///     Initializes the singleton application object.  This is the first line of authored code
	///     executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		InitializeComponent();
	}

	protected override MauiApp CreateMauiApp()
	{
		return MauiProgram.CreateMauiApp();
	}

	private async Task RegisterDevice(string notificationHub, string key)
	{
		var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
		channel.PushNotificationReceived += Channel_PushNotificationReceived;
		var deviceInstallation = new
		{
			InstallationId = new EasClientDeviceInformation().Id,
			Platform = "wns",
			PushChannel = channel.Uri
		};
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("x-ms-version", "2015-01");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
																 CreateToken($"https://{notificationHub}.servicebus.windows.net",
																			 "DefaultListenSharedAccessSignature",
																			 key));
		await httpClient.PutAsJsonAsync($"https://{notificationHub}.servicebus.windows.net/{notificationHub}/installations/{deviceInstallation.InstallationId}?api-version=2015-01", deviceInstallation);
	}

	private void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
	{
		Toast.Make(args.RawNotification.Content).Show();
	}

	protected override async void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);
		await RegisterDevice("drawgo", "p7fcbXEbiLKwdv7uX/XpFCRSmP5AEaxuBLmSOluzXhE=");
	}

	private static string CreateToken(string resourceUri, string keyName, string key)
	{
		var sinceEpoch = DateTime.UtcNow - DateTime.UnixEpoch;
		var week = 60 * 60 * 24 * 7;
		var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);
		var stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
		var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
		var sasToken = string.Format(CultureInfo.InvariantCulture,
									 "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
									 HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry,
									 keyName);
		return sasToken;
	}
}
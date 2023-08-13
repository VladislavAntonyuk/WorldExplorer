namespace Client;

using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.Provider;
using Firebase.Messaging;
using global::Android.App;

public static class DeviceInstallationService
{
	private static bool NotificationsSupported
		=> GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;

	private static string? GetDeviceId()
		=> Settings.Secure.GetString(Application.Context.ContentResolver, Settings.Secure.AndroidId);

	public static async Task RegisterDevice(string notificationHub, string key)
	{
		if (!NotificationsSupported)
		{
			return;
		}

		var firebaseToken = await FirebaseMessaging.Instance.GetToken();
		var deviceInstallation = new
		{
			InstallationId = GetDeviceId(),
			Platform = "gcm",
			PushChannel = firebaseToken.ToString()
		};
		using var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("x-ms-version", "2015-01");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization",
														   CreateToken($"https://{notificationHub}.servicebus.windows.net",
																	   "DefaultListenSharedAccessSignature",
																	   key));
		await httpClient.PutAsJsonAsync($"https://{notificationHub}.servicebus.windows.net/{notificationHub}/installations/{deviceInstallation.InstallationId}?api-version=2015-01", deviceInstallation);
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
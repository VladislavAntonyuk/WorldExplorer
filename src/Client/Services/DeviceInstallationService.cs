namespace Client.Services;

using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public record DeviceInstallation(string? InstallationId, string Platform, string PushChannel);

public static partial class DeviceInstallationService
{
	public static async Task RegisterDevice(string notificationHub, string key)
	{
		if (Connectivity.NetworkAccess != NetworkAccess.Internet)
		{
			return;
		}

		var deviceInstallation = await GetDeviceInstallation();
		if (deviceInstallation == null)
		{
			return;
		}

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
		const int week = 60 * 60 * 24 * 7;
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
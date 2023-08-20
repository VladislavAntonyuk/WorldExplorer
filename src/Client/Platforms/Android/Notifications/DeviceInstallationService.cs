namespace Client;

using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.Provider;
using Firebase.Messaging;
using global::Android.App;

public static partial class DeviceInstallationService
{
	private static bool NotificationsSupported
		=> GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;

	private static string? GetDeviceId()
		=> Settings.Secure.GetString(Application.Context.ContentResolver, Settings.Secure.AndroidId);

	
	private static async Task<DeviceInstallation?> GetDeviceInstallation()
	{
		if (!NotificationsSupported)
		{
			return null;
		}

		var firebaseToken = await FirebaseMessaging.Instance.GetToken();
		return new DeviceInstallation(GetDeviceId(), "gcm", firebaseToken.ToString());
	}
}
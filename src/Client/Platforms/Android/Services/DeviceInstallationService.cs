namespace Client.Services;

using Android.App;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.Provider;
using Firebase.Messaging;

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

		try
		{
			var firebaseToken = await FirebaseMessaging.Instance.GetToken();
			return new DeviceInstallation(GetDeviceId(), "gcm", firebaseToken.ToString());
		}
		catch
		{
			return null;
		}
	}
}
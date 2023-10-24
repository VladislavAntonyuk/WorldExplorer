namespace Client.Services;

using CommunityToolkit.Maui.Alerts;
using Windows.Networking.PushNotifications;
using Windows.Security.ExchangeActiveSyncProvisioning;

public static partial class DeviceInstallationService
{
	private static async Task<DeviceInstallation?> GetDeviceInstallation()
	{
		var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
		channel.PushNotificationReceived += Channel_PushNotificationReceived;
		return new DeviceInstallation(new EasClientDeviceInformation().Id.ToString(), "wns", channel.Uri);
	}

	private static void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
	{
		Toast.Make(args.RawNotification.Content).Show();
	}
}
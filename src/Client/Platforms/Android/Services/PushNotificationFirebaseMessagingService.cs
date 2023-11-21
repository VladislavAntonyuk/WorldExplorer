namespace Client.Services;

using Android;
using Android.App;
using Android.Content.PM;
using AndroidX.Core.Content;
using Firebase.Messaging;
using Constants = Shared.Constants;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
	int messageId;
	public override void OnMessageReceived(RemoteMessage message)
	{
		base.OnMessageReceived(message);

		MainThread.InvokeOnMainThreadAsync(() =>
		{
			if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
			{
				return;
			}

			var pushNotification = message.GetNotification();

			var manager = (NotificationManager?)Application.Context.GetSystemService(NotificationService);
			if (OperatingSystem.IsAndroidVersionAtLeast(26))
			{
				var channel = new NotificationChannel(pushNotification.ChannelId, Constants.ProductName, NotificationImportance.Max);
				manager?.CreateNotificationChannel(channel);
			}

			var notification = new Notification.Builder(Application.Context, pushNotification.ChannelId)
										.SetContentTitle(pushNotification.Title)
										.SetContentText(pushNotification.Body)
										.SetSmallIcon(Resource.Mipmap.SymDefAppIcon)
										.Build();

			manager?.Notify(messageId++, notification);
		});
	}
}
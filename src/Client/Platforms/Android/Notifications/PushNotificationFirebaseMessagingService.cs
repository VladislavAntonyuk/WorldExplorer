namespace Client;

using AndroidX.Core.App;
using CommunityToolkit.Maui.Alerts;
using Firebase.Messaging;
using global::Android.App;
using global::Android.Content;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
{
	public override void OnMessageReceived(RemoteMessage p0)
	{
		base.OnMessageReceived(p0);

		MainThread.InvokeOnMainThreadAsync(() => Toast.Make(p0.GetNotification().Body).Show());
	}
}
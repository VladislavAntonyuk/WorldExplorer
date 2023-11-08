namespace Client;

using Foundation;
using Microsoft.Identity.Client;
using UIKit;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp()
	{
		return MauiProgram.CreateMauiApp();
	}
	
	public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
	{
		AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
		return base.OpenUrl(application, url, options);
	}
}
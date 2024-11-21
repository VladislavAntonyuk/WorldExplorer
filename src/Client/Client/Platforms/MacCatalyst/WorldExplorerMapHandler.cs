using PlatformMap = WebKit.WKWebView;

namespace Client.Controls.WorldExplorerMap;

using Foundation;
using WebKit;

public partial class WorldExplorerMapHandler
{
	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		platformView.Configuration.UserContentController.AddScriptMessageHandler(new WebViewScriptMessageHandler(this), "worldExplorerMap");
		VirtualView.Pins.CollectionChanged += Pins_CollectionChanged;
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		VirtualView.Pins.CollectionChanged -= Pins_CollectionChanged;
		CallJsMethod(platformView, "destroyMap()");
		platformView.Configuration.UserContentController.RemoveScriptMessageHandler("worldExplorerMap");

		base.DisconnectHandler(platformView);
	}

	static void CallJsMethod(PlatformMap platformWebView, string script)
	{
		platformWebView.EvaluateJavaScript(script, (result, error) => { });
	}

	private sealed class WebViewScriptMessageHandler(WorldExplorerMapHandler handler)
		: NSObject, IWKScriptMessageHandler
	{
		public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
		{
			handler.WebViewWebMessageReceived(((NSString)message.Body).ToString());
		}
	}
}
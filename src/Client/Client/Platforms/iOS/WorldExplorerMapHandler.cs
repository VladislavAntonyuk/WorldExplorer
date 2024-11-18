using PlatformMap = WebKit.WKWebView;

namespace Client.Controls.WorldExplorerMap;

using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WebKit;

public partial class WorldExplorerMapHandler
{
	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		platformView.Configuration.UserContentController.AddScriptMessageHandler(new WebViewScriptMessageHandler(WebViewWebMessageReceived), "worldExplorerMap");
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

	private sealed class WebViewScriptMessageHandler(Action<string> messageReceivedAction)
		: NSObject, IWKScriptMessageHandler
	{
		public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
		{
			messageReceivedAction(((NSString)message.Body).ToString());
		}
	}
}
using PlatformMap = WebKit.WKWebView;

namespace Client.Controls.WorldExplorerMap;

using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WebKit;

public partial class WorldExplorerMapHandler
{
	protected override PlatformMap CreatePlatformView()
	{
		var config = new WKWebViewConfiguration();
		config.UserContentController.AddScriptMessageHandler(new WebViewScriptMessageHandler(WebViewWebMessageReceived), "worldExplorerMap");

		var webView = new MauiWKWebView(CGRect.Empty, new WebViewHandler(), config);
		return webView;
	}

	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		var mapPage = GetWebPage();
		platformView.LoadHtmlString(new NSString(mapPage), null);
		WorldExplorerMapPropertyMapper.UpdateProperties(this, VirtualView);
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		CallJsMethod(platformView, "destroyMap()");

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
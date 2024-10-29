using PlatformMap = Microsoft.Maui.Platform.MauiWebView;

namespace Client.Controls.WorldExplorerMap;

using System.Text.Json;
using Android.Webkit;
using Java.Interop;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public partial class WorldExplorerMapHandler
{
	private WebViewJavaScriptInterface? javaScriptInterface;

	protected override PlatformMap CreatePlatformView()
	{
		var webViewHandler = new WebViewHandler();
		var webView = new MauiWebView(webViewHandler, Context);
		return webView;
	}

	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		javaScriptInterface = new WebViewJavaScriptInterface(this);
		VirtualView.Pins.CollectionChanged += Pins_CollectionChanged;
		var mapPage = GetWebPage();
		platformView.AddJavascriptInterface(javaScriptInterface, "worldExplorerMap");
		((IWebViewDelegate)platformView).LoadHtml(mapPage, null);
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		VirtualView.Pins.CollectionChanged -= Pins_CollectionChanged;
		CallJsMethod(platformView, "destroyMap()");
		platformView.RemoveJavascriptInterface("worldExplorerMap");
		base.DisconnectHandler(platformView);
	}

	static void CallJsMethod(PlatformMap platformWebView, string script)
	{
		platformWebView.EvaluateJavaScript(new EvaluateJavaScriptAsyncRequest(script));
	}

	private sealed class WebViewJavaScriptInterface(WorldExplorerMapHandler handler) : Java.Lang.Object
	{
		[JavascriptInterface]
		[Export("sendMessage")]
		public void SendMessage(string message)
		{
			handler.WebViewWebMessageReceived(message);
		}
	}
}
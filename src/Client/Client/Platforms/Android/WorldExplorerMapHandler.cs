using PlatformMap = Android.Webkit.WebView;

namespace Client.Controls.WorldExplorerMap;

using System.Text.Json;
using Android.Webkit;
using Java.Interop;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public partial class WorldExplorerMapHandler
{
	private WebViewJavaScriptInterface? javaScriptInterface;

	//protected override PlatformMap CreatePlatformView()
	//{
	//	WebView.SetWebContentsDebuggingEnabled(true);
	//	var webView = new MauiWebView(this, Context);

	//	webView.Settings.JavaScriptEnabled = true;
	//	webView.Settings.DomStorageEnabled = true;
	//	webView.Settings.AllowContentAccess = true;
	//	webView.Settings.BlockNetworkImage = false;
	//	webView.Settings.BlockNetworkLoads = false;
	//	webView.Settings.DatabaseEnabled = true;
	//	webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
	//	webView.Settings.LoadsImagesAutomatically = true;
	//	webView.Settings.SafeBrowsingEnabled = false;
	//	webView.Settings.SetGeolocationEnabled(true);
	//	webView.Settings.AllowFileAccess = true;
	//	webView.Settings.AllowFileAccessFromFileURLs = true;
	//	webView.Settings.AllowUniversalAccessFromFileURLs = true;

	//	webView.SetWebChromeClient(new WebChromeClient());
	//	webView.SetWebViewClient(new WebViewClient());

	//	return webView;
	//}

	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		javaScriptInterface = new WebViewJavaScriptInterface(this);
		VirtualView.Pins.CollectionChanged += Pins_CollectionChanged;
		platformView.AddJavascriptInterface(javaScriptInterface, "worldExplorerMap");
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
		[Export("postMessage")]
		public void PostMessage(string message)
		{
			handler.WebViewWebMessageReceived(message);
		}
	}
}
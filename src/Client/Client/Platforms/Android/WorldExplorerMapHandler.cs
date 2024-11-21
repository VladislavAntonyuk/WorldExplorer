using PlatformMap = Android.Webkit.WebView;

namespace Client.Controls.WorldExplorerMap;

using System.Text.Json;
using Android.Views;
using Android.Webkit;
using Java.Interop;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public partial class WorldExplorerMapHandler
{
	private WebViewJavaScriptInterface? javaScriptInterface;
	// This name matches the name of the API used in HybridWebView.js and must remain in sync
	private const string HybridWebViewHostJsName = "hybridWebViewHost";

	protected override PlatformMap CreatePlatformView()
	{
		var platformView = new WebView(Context)
		{
			LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
		};

		platformView.Settings.DomStorageEnabled = true;
		platformView.Settings.SetSupportMultipleWindows(true);
		
		platformView.Settings.JavaScriptEnabled = true;

		javaScriptInterface = new WebViewJavaScriptInterface(this);
		platformView.AddJavascriptInterface(javaScriptInterface, HybridWebViewHostJsName);

		return platformView;
	}

	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);

		var webViewClient = new MapClient();
		PlatformView.SetWebViewClient(webViewClient);

		platformView.LoadUrl(new Uri(AppOriginUri, "/").ToString());
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		if (OperatingSystem.IsAndroidVersionAtLeast(26))
		{
			//if (platformView.WebViewClient is MauiHybridWebViewClient webViewClient)
			//{
			//	webViewClient.Disconnect();
			//}
			//if (platformView.WebChromeClient is MauiWebChromeClient webChromeClient)
			//{
			//	webChromeClient.Disconnect();
			//}
		}

		platformView.SetWebViewClient(null!);
		//platformView.SetWebChromeClient(null);

		platformView.StopLoading();


		base.DisconnectHandler(platformView);
	}

	internal static void EvaluateJavaScript(IHybridWebViewHandler handler, IHybridWebView hybridWebView, EvaluateJavaScriptAsyncRequest request)
	{
		handler.PlatformView.EvaluateJavaScript(request);
	}

	public static void MapSendRawMessage(IHybridWebViewHandler handler, IHybridWebView hybridWebView, object? arg)
	{
		if (arg is not HybridWebViewRawMessage hybridWebViewRawMessage || handler.PlatformView is not IHybridPlatformWebView hybridPlatformWebView)
		{
			return;
		}

		hybridPlatformWebView.SendRawMessage(hybridWebViewRawMessage.Message ?? "");
	}





























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

	//protected override void ConnectHandler(PlatformMap platformView)
	//{
	//	base.ConnectHandler(platformView);
	//	javaScriptInterface = new WebViewJavaScriptInterface(this);
	//	VirtualView.Pins.CollectionChanged += Pins_CollectionChanged;
	//	platformView.AddJavascriptInterface(javaScriptInterface, "worldExplorerMap");
	//}

	//protected override void DisconnectHandler(PlatformMap platformView)
	//{
	//	VirtualView.Pins.CollectionChanged -= Pins_CollectionChanged;
	//	CallJsMethod(platformView, "destroyMap()");
	//	platformView.RemoveJavascriptInterface("worldExplorerMap");
	//	base.DisconnectHandler(platformView);
	//}

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

public class MapClient : WebViewClient
{
	public override WebResourceResponse? ShouldInterceptRequest(WebView? view, IWebResourceRequest? request)
	{
		var stream = new MemoryStream();
		return new WebResourceResponse("text/html", "UTF-8", 200, "OK", null, stream);

		return base.ShouldInterceptRequest(view, request);
	}
}
using PlatformMap = Microsoft.Maui.Platform.MauiWebView;

namespace WorldExplorer.Client.Map.WorldExplorerMap;

using System.Text.Json;
using Android.Webkit;
using Java.Interop;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public partial class WorldExplorerMapHandler
{
	private WebViewJavaScriptInterface _javaScriptInterface;
	WebViewHandler webViewHandler;

	protected override PlatformMap CreatePlatformView()
	{
		_javaScriptInterface = new WebViewJavaScriptInterface(this);
		webViewHandler = new WebViewHandler();
		var webView = new MauiWebView(webViewHandler, Context);
		webView.SetWebViewClient(new MapWebViewClient(this, webViewHandler));
		return webView;
	}

	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		var mapPage = GetWebPage();
		platformView.AddJavascriptInterface(_javaScriptInterface, "worldExplorerMap");
		((IWebViewDelegate)platformView).LoadHtml(mapPage, null);
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		CallJsMethod(platformView, "destroyMap()");
		platformView.RemoveJavascriptInterface("worldExplorerMap");
		base.DisconnectHandler(platformView);
	}

	static void CallJsMethod(PlatformMap platformWebView, string script)
	{
		platformWebView.EvaluateJavaScript(new EvaluateJavaScriptAsyncRequest(script));
	}

	private sealed class MapWebViewClient(WorldExplorerMapHandler mapHandler, WebViewHandler handler) : WebViewClient
	{
		public override void OnLoadResource(WebView? view, string? url)
		{
			base.OnLoadResource(view, url);
		}

		public override WebResourceResponse? ShouldInterceptRequest(WebView? view, IWebResourceRequest? request)
		{
			return base.ShouldInterceptRequest(view, request);
		}

		public override bool ShouldOverrideUrlLoading(WebView? view, IWebResourceRequest? request)
		{
			return base.ShouldOverrideUrlLoading(view, request);
		}

		public override void OnPageFinished(WebView? view, string? url)
		{
			base.OnPageFinished(view, url);
			WorldExplorerMapPropertyMapper.UpdateProperties(mapHandler, mapHandler.VirtualView);
		}
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
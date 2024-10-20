using PlatformMap = Microsoft.Maui.Platform.MauiWebView;

namespace WorldExplorer.Client.Map.WorldExplorerMap;

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

public partial class WorldExplorerMapHandler
{
	protected override PlatformMap CreatePlatformView()
	{
		var webView = new MauiWebView(new WebViewHandler())
		{
			IsRightTapEnabled = false , CanGoBack = false, CanGoForward = false, AllowDrop = false,
			CanDrag = false, IsDoubleTapEnabled = false, IsHoldingEnabled = false
		};
		return webView;
	}

	protected override void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		var mapPage = GetWebPage();
		platformView.NavigationStarting += OnNavigationStarting;
		platformView.WebMessageReceived += WebViewWebMessageReceived;
		platformView.LoadHtml(mapPage, null);
	}

	private static void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
	{
		args.Cancel = args.IsUserInitiated;
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		CallJsMethod(platformView, "destroyMap()");
		platformView.WebMessageReceived -= WebViewWebMessageReceived;
		platformView.NavigationStarting -= OnNavigationStarting;
		base.DisconnectHandler(platformView);
	}

	static void CallJsMethod(PlatformMap platformWebView, string script)
	{
		if (platformWebView.CoreWebView2 != null)
		{
			platformWebView.DispatcherQueue.TryEnqueue(async () => await platformWebView.CoreWebView2.ExecuteScriptAsync(script));
		}
	}

	void WebViewWebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
	{
		WebViewWebMessageReceived(args.WebMessageAsJson);
	}
}
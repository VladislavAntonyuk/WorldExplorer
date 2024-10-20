using PlatformMap = Microsoft.Maui.Platform.MauiWebView;

namespace WorldExplorer.Client.Map.WorldExplorerMap;

using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Storage.Streams;

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
		platformView.NavigationCompleted += WebViewNavigationCompleted;
		platformView.WebMessageReceived += WebViewWebMessageReceived;
		platformView.LoadHtml(mapPage, null);
	}

	private void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
	{
		args.Cancel = args.IsUserInitiated;
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		CallJsMethod(platformView, "destroyMap()");
		platformView.NavigationCompleted -= WebViewNavigationCompleted;
		platformView.WebMessageReceived -= WebViewWebMessageReceived;
		platformView.NavigationStarting -= OnNavigationStarting;
		base.DisconnectHandler(platformView);
	}

	void WebViewNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
	{
		WorldExplorerMapPropertyMapper.UpdateProperties(this, VirtualView);
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
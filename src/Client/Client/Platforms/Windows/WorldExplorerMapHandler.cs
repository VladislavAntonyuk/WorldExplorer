//using PlatformMap = Microsoft.Maui.Platform.MauiWebView;
using PlatformMap = Microsoft.UI.Xaml.Controls.WebView2;

namespace Client.Controls.WorldExplorerMap;

using System.Buffers.Text;
using Windows.ApplicationModel;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

public partial class WorldExplorerMapHandler
{
	const string LocalHostName = "appdir";
	const string LocalScheme = $"https://{LocalHostName}/";
	const string BaseInsertionScript = @"
			var head = document.getElementsByTagName('head')[0];
			var bases = head.getElementsByTagName('base');
			if(bases.length == 0) {
				head.innerHTML = 'baseTag' + head.innerHTML;
			}";
	static string ApplicationPath => Package.Current != null
		? Package.Current.InstalledLocation.Path
		: AppContext.BaseDirectory;
	static string GetBaseTagInsertionScript(string baseUrl)
	{
		var baseTag = $"<base href=\"{baseUrl}\"></base>";
		return $"<script>{BaseInsertionScript.Replace("baseTag", baseTag, StringComparison.Ordinal)}</script>";
	}

	protected override PlatformMap CreatePlatformView()
	{
		var webView = new PlatformMap()
		{
			IsRightTapEnabled = false,
			CanGoBack = false,
			CanGoForward = false,
			AllowDrop = false,
			CanDrag = false,
			IsDoubleTapEnabled = false,
			IsHoldingEnabled = false
		};
		return webView;
	}

	protected override async void ConnectHandler(PlatformMap platformView)
	{
		base.ConnectHandler(platformView);
		var mapPage = GetWebPage();
		platformView.NavigationStarting += OnNavigationStarting;
		platformView.WebMessageReceived += WebViewWebMessageReceived;
		VirtualView.Pins.CollectionChanged += Pins_CollectionChanged;
		var script = GetBaseTagInsertionScript(LocalScheme);
		var htmlWithScript = $"{script}\n{mapPage}";
		await platformView.EnsureCoreWebView2Async();
		platformView.CoreWebView2.SetVirtualHostNameToFolderMapping(
			LocalHostName,
			ApplicationPath,
			CoreWebView2HostResourceAccessKind.Allow);

		platformView.NavigateToString(htmlWithScript);
	}

	private static void OnNavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
	{
		args.Cancel = args.IsUserInitiated;
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		VirtualView.Pins.CollectionChanged -= Pins_CollectionChanged;
		CallJsMethod(platformView, "destroyMap()");
		platformView.WebMessageReceived -= WebViewWebMessageReceived;
		platformView.NavigationStarting -= OnNavigationStarting;
		base.DisconnectHandler(platformView);
	}

	static async void CallJsMethod(PlatformMap platformWebView, string script)
	{
		if (platformWebView.CoreWebView2 != null)
		{
			await platformWebView.CoreWebView2.ExecuteScriptAsync(script);
		}
	}

	void WebViewWebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
	{
		WebViewWebMessageReceived(args.WebMessageAsJson);
	}
}
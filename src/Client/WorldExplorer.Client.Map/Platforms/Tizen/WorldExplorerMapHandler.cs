#if ANDROID
using PlatformMap = Android.Views.View;
#elif IOS || MACCATALYST
using PlatformMap = UIKit.UIView;
#elif WINDOWS
using PlatformMap = Microsoft.UI.Xaml.FrameworkElement;
#endif

namespace WorldExplorer.Client.Map.WorldExplorerMap;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

public partial class WorldExplorerMapHandler
{

	protected override PlatformMap CreatePlatformView()
	{
		using var stream = FileSystem.OpenAppPackageFileAsync("map.html").GetAwaiter().GetResult();
		using var reader = new StreamReader(stream);
		var mapPage = reader.ReadToEnd();
#if ANDROID
		var webView = new MauiWebView(new WebViewHandler(), Context);
#else
		var webView = new MauiWebView(new WebViewHandler());
#endif
		webView.NavigationCompleted += WebViewNavigationCompleted;
		webView.WebMessageReceived += WebViewWebMessageReceived;
		webView.LoadHtml(mapPage, null);
		return webView;
	}

	protected override void DisconnectHandler(PlatformMap platformView)
	{
		if (PlatformView is MauiWebView mauiWebView)
		{
			CallJsMethod(platformView, "destroyMap()");
			mauiWebView.NavigationCompleted -= WebViewNavigationCompleted;
			mauiWebView.WebMessageReceived -= WebViewWebMessageReceived;
		}

		base.DisconnectHandler(platformView);
	}

	void WebViewNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
	{
		CallJsMethod(sender, "initMap()");
		Mapper.UpdateProperties(this, VirtualView);
	}

	static void CallJsMethod(PlatformMap platformWebView, string script)
	{
		if (platformWebView is WebView2 webView2 && webView2.CoreWebView2 != null)
		{
			platformWebView.DispatcherQueue.TryEnqueue(async () => await webView2.ExecuteScriptAsync(script));
		}
	}

	void WebViewWebMessageReceived(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
	{
		// For some reason the web message is empty
		if (string.IsNullOrEmpty(args.WebMessageAsJson))
		{
			return;
		}

		var eventMessage = JsonSerializer.Deserialize<EventMessage>(args.WebMessageAsJson, jsonSerializerOptions);

		// The web message (or it's ID) could not be deserialized to something we recognize
		if (eventMessage is null || !Enum.TryParse<EventIdentifier>(eventMessage.Id, true, out var eventId))
		{
			return;
		}

		var payloadAsString = eventMessage.Payload?.ToString();

		// The web message does not have a payload
		if (string.IsNullOrWhiteSpace(payloadAsString))
		{
			return;
		}

		switch (eventId)
		{
			case EventIdentifier.MarkerClicked:
				var clickedPinWebView = JsonSerializer.Deserialize<WorldExplorerPin>(payloadAsString, jsonSerializerOptions);
				var clickedPinWebViewId = clickedPinWebView?.PlaceId.ToString();

				if (!string.IsNullOrEmpty(clickedPinWebViewId))
				{
					var clickedPin = VirtualView.Pins.FirstOrDefault(p => p.PlaceId.ToString().Equals(clickedPinWebViewId));
					((IWorldExplorerPin?)clickedPin)?.OnMarkerClicked();
				}

				break;
		}
	}
}
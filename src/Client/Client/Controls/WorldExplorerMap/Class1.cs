using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#if ANDROID
using PlatformMap = Android.Views.View;
#elif IOS || MACCATALYST
using PlatformMap = UIKit.UIView;
#elif WINDOWS
using PlatformMap = Microsoft.UI.Xaml.FrameworkElement;
using Windows.Devices.Geolocation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
#endif

namespace Client.Controls.WorldExplorerMap;

using Microsoft.Maui.Handlers;

using Microsoft.Maui.Platform;

using System.Text.Json;

public partial class WorldExplorerMapHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
	: ViewHandler<IWorldExplorerMap, PlatformMap>(mapper ?? Mapper, commandMapper ?? CommandMapper)
{
	readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public static IPropertyMapper<IWorldExplorerMap, WorldExplorerMapHandler> Mapper = new PropertyMapper<IWorldExplorerMap, WorldExplorerMapHandler>(ViewMapper)
	{
		[nameof(IWorldExplorerMap.IsShowingUser)] = MapIsShowingUser,
		[nameof(IWorldExplorerMap.Pins)] = MapPins
	};


	public static CommandMapper<IWorldExplorerMap, WorldExplorerMapHandler> CommandMapper = new(ViewCommandMapper)
	{
		//[nameof(IWorldExplorerMap.MoveToRegion)] = MapMoveToRegion
	};

	public WorldExplorerMapHandler() : this(Mapper, CommandMapper)
	{

	}

	/// <inheritdoc/>
	[RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
	[RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
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

	/// <inheritdoc />
	[RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
	[RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
	protected override void DisconnectHandler(PlatformMap platformView)
	{
		if (PlatformView is MauiWebView mauiWebView)
		{
			mauiWebView.NavigationCompleted -= WebViewNavigationCompleted;
			mauiWebView.WebMessageReceived -= WebViewWebMessageReceived;
		}

		base.DisconnectHandler(platformView);
	}

	/// <summary>
	/// Maps IsShowingUser
	/// </summary>
	public static async void MapIsShowingUser(WorldExplorerMapHandler handler, IWorldExplorerMap map)
	{
		if (map.IsShowingUser)
		{
			var location = await GetCurrentLocation();
			if (location != null)
			{
				CallJSMethod(handler.PlatformView, $"addLocationPin({location.Latitude.ToString(CultureInfo.InvariantCulture)},{location.Longitude.ToString(CultureInfo.InvariantCulture)});");
			}
		}
		else
		{
			CallJSMethod(handler.PlatformView, "removeLocationPin();");
		}
	}

	/// <summary>
	/// Map Pins
	/// </summary>
	public static void MapPins(WorldExplorerMapHandler handler, IWorldExplorerMap map)
	{
		CallJSMethod(handler.PlatformView, "removeAllPins();");

		foreach (var pin in map.Pins)
		{
			CallJSMethod(handler.PlatformView, $"addPin({pin.Location.Latitude.ToString(CultureInfo.InvariantCulture)}," +
				$"{pin.Location.Longitude.ToString(CultureInfo.InvariantCulture)},'{pin.Label}', '{pin.Address}', '{pin.Id}');");
		}
	}

	/// <summary>
	/// Maps MoveToRegion
	/// </summary>
	public static new void MapMoveToRegion(WorldExplorerMapHandler handler, IMap map, object? arg)
	{
		var newRegion = arg;
		if (newRegion == null)
		{
			return;
		}

		//handler.regionToGo = newRegion;

		//CallJSMethod(handler.PlatformView, $"setRegion({newRegion.Center.Latitude.ToString(CultureInfo.InvariantCulture)},{newRegion.Center.Longitude.ToString(CultureInfo.InvariantCulture)});");
	}

	static void CallJSMethod(PlatformMap platformWebView, string script)
	{
		if (platformWebView is WebView2 webView2 && webView2.CoreWebView2 != null)
		{
			platformWebView.DispatcherQueue.TryEnqueue(async () => await webView2.ExecuteScriptAsync(script));
		}
	}
	
	static async Task<Location?> GetCurrentLocation()
	{
		var geoLocator = new Geolocator();
		var position = await geoLocator.GetGeopositionAsync();
		return new Location(position.Coordinate.Latitude, position.Coordinate.Longitude);
	}

	void WebViewNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
	{
		// Update initial properties when our page is loaded
		Mapper.UpdateProperties(this, VirtualView);

		//if (regionToGo != null)
		//{
		//	MapMoveToRegion(this, VirtualView, regionToGo);
		//}
	}

	[RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
	[RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
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
			case EventIdentifier.BoundsChanged:
				var mapRect = JsonSerializer.Deserialize<Bounds>(payloadAsString, jsonSerializerOptions);
				if (mapRect?.Center is not null)
				{
					//VirtualView.VisibleRegion = new MapSpan(new Location(mapRect.Center.Latitude, mapRect.Center.Longitude),
					//	mapRect.Height, mapRect.Width);
				}
				break;
			case EventIdentifier.MapClicked:
				var clickedLocation = JsonSerializer.Deserialize<Location>(payloadAsString,
					jsonSerializerOptions);
				if (clickedLocation is not null)
				{
					//	VirtualView.Clicked(clickedLocation);
				}
				break;

			case EventIdentifier.InfoWindowClicked:
				var clickedInfoWindowWebView = JsonSerializer.Deserialize<InfoWindow>(payloadAsString,
					jsonSerializerOptions);
				var clickedInfoWindowWebViewId = clickedInfoWindowWebView?.InfoWindowMarkerId;

				if (!string.IsNullOrEmpty(clickedInfoWindowWebViewId))
				{
					//var clickedPin = VirtualView.Pins.SingleOrDefault(p => (p as Pin)?.Id.ToString().Equals(clickedInfoWindowWebViewId) ?? false);

					//var hideInfoWindow = clickedPin?.SendInfoWindowClick();
					//if (hideInfoWindow is not false)
					//{
					//	CallJSMethod(PlatformView, "hideInfoWindow();");
					//}
				}
				break;

			case EventIdentifier.PinClicked:
				//var clickedPinWebView = JsonSerializer.Deserialize<Pin>(payloadAsString, jsonSerializerOptions);
				//var clickedPinWebViewId = clickedPinWebView?.MarkerId?.ToString();

				//if (!string.IsNullOrEmpty(clickedPinWebViewId))
				//{
				//	var clickedPin = VirtualView.Pins.SingleOrDefault(p => (p as Pin)?.Id.ToString().Equals(clickedPinWebViewId) ?? false);

				//	var hideInfoWindow = clickedPin?.SendMarkerClick();
				//	if (hideInfoWindow is not false)
				//	{
				//		CallJSMethod(PlatformView, "hideInfoWindow();");
				//	}
				//}
				break;
		}
	}
}
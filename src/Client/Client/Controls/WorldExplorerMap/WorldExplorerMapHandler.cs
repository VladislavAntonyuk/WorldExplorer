#if ANDROID
using PlatformMap = Android.Webkit.WebView;
#elif IOS || MACCATALYST
using PlatformMap = Microsoft.Maui.Platform.MauiWKWebView;
#elif WINDOWS
using PlatformMap = Microsoft.Maui.Platform.MauiWebView;
#endif

namespace Client.Controls.WorldExplorerMap;

using System.Collections.Specialized;
using System.Globalization;
using System.Text.Json;
using Microsoft.Maui.Handlers;
#if __IOS__ || MACCATALYST
using PlatformView = WebKit.WKWebView;
#elif ANDROID
using PlatformView = Android.Webkit.WebView;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.Controls.WebView2;
#elif TIZEN
using PlatformView = Microsoft.Maui.Platform.MauiWebView;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID && !TIZEN)
using PlatformView = System.Object;
#endif

	public partial interface IMapWebViewHandler : IWebViewHandler
	{
		new IWorldExplorerMap VirtualView { get; }
		new PlatformView PlatformView { get; }
	}

public partial class WorldExplorerMapHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
	: WebViewHandler(mapper ?? WorldExplorerMapPropertyMapper, commandMapper ?? WebViewHandler.CommandMapper), IMapWebViewHandler
{
	public new IWorldExplorerMap VirtualView => (IWorldExplorerMap)base.VirtualView;
	
	PlatformView IMapWebViewHandler.PlatformView => PlatformView;

	readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public static readonly IPropertyMapper<IWorldExplorerMap, WorldExplorerMapHandler> WorldExplorerMapPropertyMapper = new PropertyMapper<IWorldExplorerMap, WorldExplorerMapHandler>(WebViewHandler.Mapper)
	{
		[nameof(IWorldExplorerMap.UserLocation)] = MapUserLocation,
		[nameof(IWorldExplorerMap.Pins)] = MapPins
	};

	public WorldExplorerMapHandler() : this(WorldExplorerMapPropertyMapper, WebViewHandler.CommandMapper)
	{

	}

	public static void MapUserLocation(WorldExplorerMapHandler handler, IWorldExplorerMap map)
	{
		CallJsMethod(handler.PlatformView,
					 map.UserLocation is not null
						 ? $"addLocationPin({map.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{map.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture)});"
						 : "removeLocationPin();");
	}

	public static void MapPins(WorldExplorerMapHandler handler, IWorldExplorerMap map)
	{
		CallJsMethod(handler.PlatformView, "removeAllPins();");

		foreach (var pins in map.Pins.Chunk(50))
		{
			var pinsArray = JsonSerializer.Serialize(pins.Select(p => new
			{
				p.Location.Latitude,
				p.Location.Longitude,
				p.Label,
				p.Image,
				p.PlaceId
			}));

			var script = $"addMarkers({pinsArray});";
			CallJsMethod(handler.PlatformView, script);
		}
	}

	static string GetWebPage()
	{
		using var stream = FileSystem.OpenAppPackageFileAsync("wwwroot/map.html").GetAwaiter().GetResult();
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}


	void WebViewWebMessageReceived(string webMessageAsJson)
	{
		if (string.IsNullOrEmpty(webMessageAsJson))
		{
			return;
		}

		var eventMessage = JsonSerializer.Deserialize<EventMessage>(webMessageAsJson, jsonSerializerOptions);

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
			case EventIdentifier.MapInitialized:
				VirtualView.OnMapReady();
				break;
			case EventIdentifier.MarkerClicked:
				var clickedPinWebView = JsonSerializer.Deserialize<Payload>(payloadAsString, jsonSerializerOptions);
				var clickedPinWebViewId = clickedPinWebView?.PlaceId;
				var clickedPin = VirtualView.Pins.FirstOrDefault(p => p.PlaceId == clickedPinWebViewId);
				clickedPin?.MarkerClicked?.Execute(clickedPin);

				break;
		}
	}

	private sealed record Payload(Guid PlaceId);

	private void Pins_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		WorldExplorerMapPropertyMapper.UpdateProperty(this, VirtualView, nameof(IWorldExplorerMap.Pins));
	}
}
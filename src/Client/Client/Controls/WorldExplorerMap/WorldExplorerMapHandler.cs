#if ANDROID
using PlatformMap = Microsoft.Maui.Platform.MauiWebView;
#elif IOS || MACCATALYST
using PlatformMap = WebKit.WKWebView;
#elif WINDOWS
using PlatformMap = Microsoft.Maui.Platform.MauiWebView;
#endif

namespace Client.Controls.WorldExplorerMap;

using System.Globalization;
using System.Text.Json;
using Microsoft.Maui.Handlers;

public partial class WorldExplorerMapHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
	: ViewHandler<IWorldExplorerMap, PlatformMap>(mapper ?? WorldExplorerMapPropertyMapper, commandMapper ?? ViewCommandMapper)
{
	readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public static readonly IPropertyMapper<IWorldExplorerMap, WorldExplorerMapHandler> WorldExplorerMapPropertyMapper = new PropertyMapper<IWorldExplorerMap, WorldExplorerMapHandler>(ViewMapper)
	{
		[nameof(IWorldExplorerMap.UserLocation)] = MapUserLocation,
		[nameof(IWorldExplorerMap.Pins)] = MapPins
	};

	public WorldExplorerMapHandler() : this(WorldExplorerMapPropertyMapper, ViewCommandMapper)
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

		foreach (var pin in map.Pins)
		{
			CallJsMethod(handler.PlatformView, $"addMarker('{pin.Location.Latitude.ToString(CultureInfo.InvariantCulture)}','{pin.Location.Longitude.ToString(CultureInfo.InvariantCulture)}','{pin.Label}', '{pin.Image}', '{pin.PlaceId}');");
		}
	}

	static string GetWebPage()
	{
		using var stream = FileSystem.OpenAppPackageFileAsync("wwwroot/map.html").GetAwaiter().GetResult();
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}


	void WebViewWebMessageReceived(string WebMessageAsJson)
	{
		// For some reason the web message is empty
		if (string.IsNullOrEmpty(WebMessageAsJson))
		{
			return;
		}

		var eventMessage = JsonSerializer.Deserialize<EventMessage>(WebMessageAsJson, jsonSerializerOptions);

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
				WorldExplorerMapPropertyMapper.UpdateProperties(this, VirtualView);
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
}
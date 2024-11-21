namespace Client.Controls.WorldExplorerMap;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.Json;
using System.Windows.Input;

public class WorldExplorerMap : HybridWebView
{
	private sealed record Payload(Guid PlaceId);

	public WorldExplorerMap()
	{
		RawMessageReceived += OnRawMessageReceived;
	}

	private async void PinsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		await EvaluateJavaScriptAsync("removeAllPins();");

		foreach (var pins in Pins.Chunk(50))
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
			await EvaluateJavaScriptAsync(script);
		}
	}

	readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	private void OnRawMessageReceived(object? sender, HybridWebViewRawMessageReceivedEventArgs e)
	{
		if (string.IsNullOrEmpty(e.Message))
		{
			return;
		}

		var eventMessage = JsonSerializer.Deserialize<EventMessage>(e.Message, jsonSerializerOptions);

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
				MapReadyCommand?.Execute(null);
				break;
			case EventIdentifier.MarkerClicked:
				var clickedPinWebView = JsonSerializer.Deserialize<Payload>(payloadAsString, jsonSerializerOptions);
				var clickedPinWebViewId = clickedPinWebView?.PlaceId;
				var clickedPin = Pins.FirstOrDefault(p => p.PlaceId == clickedPinWebViewId);
				clickedPin?.MarkerClicked?.Execute(clickedPin);
				break;
		}
	}

	public static readonly BindableProperty MapReadyCommandProperty = BindableProperty.Create(nameof(MapReadyCommand), typeof(ICommand), typeof(WorldExplorerMap));
	public static readonly BindableProperty UserLocationProperty = BindableProperty.Create(nameof(UserLocation), typeof(Location), typeof(WorldExplorerMap), propertyChanged: UserLocationChanged);
	public static readonly BindableProperty PinsProperty = BindableProperty.Create(nameof(Pins), typeof(ObservableCollection<WorldExplorerPin>), typeof(WorldExplorerMap), propertyChanged: PinsChanged);

	private static void PinsChanged(BindableObject bindable, object oldvalue, object newvalue)
	{
		var control = (WorldExplorerMap)bindable;
		control.Pins.CollectionChanged += control.PinsOnCollectionChanged;
	}

	private static void UserLocationChanged(BindableObject bindable, object oldvalue, object newvalue)
	{
		var control = (WorldExplorerMap)bindable;
		control.EvaluateJavaScriptAsync(
		             control.UserLocation is not null
			             ? $"addLocationPin({control.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{control.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture)});"
			             : "removeLocationPin();");
	}

	public ObservableCollection<WorldExplorerPin> Pins
	{
		get => (ObservableCollection<WorldExplorerPin>)GetValue(PinsProperty);
		set => SetValue(PinsProperty, value);
	}

	public Location? UserLocation
	{
		get => (Location?)GetValue(UserLocationProperty);
		set => SetValue(UserLocationProperty, value);
	}

	public ICommand? MapReadyCommand
	{
		get => (ICommand?)GetValue(MapReadyCommandProperty);
		set => SetValue(MapReadyCommandProperty, value);
	}
}
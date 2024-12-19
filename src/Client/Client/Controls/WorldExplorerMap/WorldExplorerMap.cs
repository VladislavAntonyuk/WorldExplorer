namespace Client.Controls.WorldExplorerMap;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Windows.Input;

public sealed class WorldExplorerMap : HybridWebView, IDisposable
{
	public static readonly BindableProperty MapReadyCommandProperty =
		BindableProperty.Create(nameof(MapReadyCommand), typeof(ICommand), typeof(WorldExplorerMap));

	public static readonly BindableProperty UserLocationProperty = BindableProperty.Create(
		nameof(UserLocation), typeof(Location), typeof(WorldExplorerMap), propertyChanged: UserLocationChanged);

	public static readonly BindableProperty PinsProperty = BindableProperty.Create(
		nameof(Pins), typeof(ObservableCollection<WorldExplorerPin>), typeof(WorldExplorerMap),
		propertyChanged: PinsChanged);

	private readonly JsonSerializerOptions jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public WorldExplorerMap()
	{
		RawMessageReceived += OnRawMessageReceived;
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

	public void Dispose()
	{
		RawMessageReceived -= OnRawMessageReceived;
	}

	private async void PinsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		await EvaluateJavaScriptAsync("removeAllMarkers();");

		foreach (var pin in Pins)
		{
			var script = $"addMarker('{pin.Location.Latitude}','{pin.Location.Longitude}','{pin.Label}','{pin.Image}','{pin.PlaceId}');";
			await EvaluateJavaScriptAsync(script);
		}

		Debug.Assert(await EvaluateJavaScriptAsync("markers.length") == Pins.Count.ToString());
	}

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

	private static void PinsChanged(BindableObject bindable, object? oldvalue, object? newvalue)
	{
		var control = (WorldExplorerMap)bindable;
		var oldPins = (ObservableCollection<WorldExplorerPin>?)oldvalue;
		var newPins = (ObservableCollection<WorldExplorerPin>?)newvalue;

		if (oldPins is not null)
		{
			oldPins.CollectionChanged -= control.PinsOnCollectionChanged;
		}

		if (newPins is not null)
		{
			newPins.CollectionChanged += control.PinsOnCollectionChanged;
		}
	}

	private static void UserLocationChanged(BindableObject bindable, object oldvalue, object newvalue)
	{
		var control = (WorldExplorerMap)bindable;
		control.EvaluateJavaScriptAsync(control.UserLocation is not null
											? $"addUserLocationMarker({control.UserLocation.Latitude.ToString(CultureInfo.InvariantCulture)},{control.UserLocation.Longitude.ToString(CultureInfo.InvariantCulture)});"
											: "removeUserLocationMarker();");
	}

	private sealed record Payload(Guid PlaceId);
}
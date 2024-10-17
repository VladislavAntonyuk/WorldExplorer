namespace WebAppTests;

using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using NetTopologySuite.Geometries;
using WorldExplorer.Common.Infrastructure.Serialization;
using WorldExplorer.Modules.Places.Application.Abstractions;
using Xunit.Abstractions;
using Location = WorldExplorer.Modules.Places.Application.Abstractions.Location;

public abstract class BaseAiProviderTests(ITestOutputHelper testOutputHelper)
{
	[Fact]
	public async Task GetNearbyPlacesShouldReturnTenPlaces()
	{
		var aiService = await GetAiService();
		var places = await aiService.GetNearByPlaces(new Location(48.455833330000026, 35.06388889000002));
		places.Count.Should().Be(10);
		testOutputHelper.WriteLine(JsonSerializer.Serialize(places, new JsonSerializerOptions(SerializerSettings.Instance)
		{
			Converters = { new PointJsonConverter() }
		}));
	}

	[Fact]
	public async Task GetPlaceDetailsShouldReturnDetailedDescription()
	{
		var aiService = await GetAiService();
		var placeDescription = await aiService.GetPlaceDescription("Dnipro Circus", new Location(48.455833330000026, 35.06388889000002));
		placeDescription?.Length.Should().BeGreaterThan(100);
		testOutputHelper.WriteLine(placeDescription);
	}

	public abstract Task<IAiService> GetAiService();
}

public class PointJsonConverter : JsonConverter<Point>
{
	public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
	public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteNumber("latitude", value.Y);
		writer.WriteNumber("longitude", value.X);
		writer.WriteEndObject();
	}
}
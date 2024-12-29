namespace WebAppTests;

using System.Text.Json;
using FluentAssertions;
using WorldExplorer.Common.Infrastructure.Serialization;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Infrastructure;
using Xunit.Abstractions;
using Location = WorldExplorer.Modules.Places.Application.Abstractions.Location;

public abstract class BaseAiProviderTests(ITestOutputHelper testOutputHelper)
{
	[Fact]
	public async Task GetNearbyPlacesShouldReturnTenPlaces()
	{
		SerializerSettings.ConfigureJsonSerializerOptionsInstance([new PointJsonConverter()]);
		var aiService = await GetAiService();
		var places = await aiService.GetNearByPlaces(new Location(48.455833330000026, 35.06388889000002), CancellationToken.None);
		places.Count.Should().Be(10);
		testOutputHelper.WriteLine(JsonSerializer.Serialize(places, SerializerSettings.Instance));
	}

	[Fact]
	public async Task GetPlaceDetailsShouldReturnDetailedDescription()
	{
		var aiService = await GetAiService();
		var placeDescription = await aiService.GetPlaceDescription("Dnipro Circus", new Location(48.455833330000026, 35.06388889000002), CancellationToken.None);
		placeDescription?.Length.Should().BeGreaterThan(100);
		testOutputHelper.WriteLine(placeDescription);
	}

	public abstract Task<IAiService> GetAiService();
}
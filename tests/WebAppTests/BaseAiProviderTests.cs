namespace WebAppTests;

using System.Text.Json;
using Shouldly;
using WorldExplorer.Common.Infrastructure.Serialization;
using WorldExplorer.Modules.Places.Application.Abstractions;
using WorldExplorer.Modules.Places.Infrastructure;
using Location = WorldExplorer.Modules.Places.Application.Abstractions.Location;

public abstract class BaseAiProviderTests(ITestOutputHelper testOutputHelper)
{
	[Fact]
	public async Task GetNearbyPlacesShouldReturnTenPlaces()
	{
		SerializerSettings.ConfigureJsonSerializerOptionsInstance([new PointJsonConverter()]);
		var aiService = await GetAiService();
		var places = await aiService.GetNearByPlaces(new Location(48.455833330000026, 35.06388889000002), TestContext.Current.CancellationToken);
		testOutputHelper.WriteLine(JsonSerializer.Serialize(places, SerializerSettings.Instance));
		places.Count.ShouldBeGreaterThanOrEqualTo(10);
	}

	[Fact]
	public async Task GetPlaceDetailsShouldReturnDetailedDescription()
	{
		var aiService = await GetAiService();
		var placeDescription = await aiService.GetPlaceDescription("Dnipro Circus", new Location(48.455833330000026, 35.06388889000002), TestContext.Current.CancellationToken);
		placeDescription.ShouldNotBeNullOrEmpty();
		placeDescription.Length.ShouldBeGreaterThan(100);
		testOutputHelper.WriteLine(placeDescription);
	}

	public abstract Task<IAiService> GetAiService();
}
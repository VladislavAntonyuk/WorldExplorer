namespace WebAppTests;

using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shared.Models;
using WebApp.Services.AI;
using Xunit.Abstractions;

public class AiServiceTests(ITestOutputHelper testOutputHelper)
{
	[Fact]
	public async Task GetNearbyPlacesShouldReturnTenPlaces()
	{
		var aiService = GetAiService();
		var places = await aiService.GetNearByPlaces(new Location(48.455833330000026, 35.06388889000002));
		places.Count.Should().Be(10);
		testOutputHelper.WriteLine(JsonSerializer.Serialize(places));
	}

	[Fact]
	public async Task GetPlaceDetailsShouldReturnDetailedDescription()
	{
		var aiService = GetAiService();
		var placeDescription = await aiService.GetPlaceDescription("Dnipro Circus", new Location(48.455833330000026, 35.06388889000002));
		placeDescription?.Length.Should().BeGreaterThan(100);
		testOutputHelper.WriteLine(placeDescription);
	}

	private AiService GetAiService()
	{
		return new(Options.Create(new AiSettings
		{
			ApiKey = "API-key",
			Provider = "OpenAI"
		}), NullLogger<AiService>.Instance);
	}
}
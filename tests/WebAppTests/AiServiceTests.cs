namespace WebAppTests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shared.Models;
using WebApp.Services.AI;

public class AiServiceTests
{
	private readonly AiService aiService = new(Options.Create(new AiSettings
	{
		ApiKey = "API_KEY",
		Model = "gpt-4-1106-preview",
		Provider = "OpenAI"
	}), NullLogger<AiService>.Instance);

	[Fact]
	public async Task GetNearbyPlacesShouldReturnTenPlaces()
	{
		var places = await aiService.GetNearByPlaces(new Location(48.455833330000026, 35.06388889000002));
		places.Count.Should().Be(10);
	}

	[Fact]
	public async Task GetPlaceDetailsShouldReturnDetailedDescription()
	{
		var placeDescription = await aiService.GetPlaceDescription("Dnipro Circus", new Location(48.455833330000026, 35.06388889000002));
		placeDescription?.Length.Should().BeGreaterThan(100);
	}
}
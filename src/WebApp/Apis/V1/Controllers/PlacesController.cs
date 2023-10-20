namespace WebApp.Apis.V1.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;
using Shared.Models;
using WebApp.Services.Place;

public class PlacesController(IPlacesService placesService) : ApiAuthControllerBase
{
	[HttpGet("recommendations")]
	[ProducesResponseType(typeof(List<Place>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public Task<List<Place>> GetRecommendations([FromQuery] Location location, CancellationToken cancellationToken)
	{
		return placesService.GetNearByPlaces(location, cancellationToken);
	}

	[HttpGet("details")]
	[ProducesResponseType(typeof(Place), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public Task<Place?> GetDetails([FromQuery] string name,
		[FromQuery] Location location,
		CancellationToken cancellationToken)
	{
		return placesService.GetPlaceDetails(name, location, cancellationToken);
	}
}
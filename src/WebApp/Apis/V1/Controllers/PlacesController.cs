namespace WebApp.Apis.V1.Controllers;

using global::Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

public class PlacesController : ApiAuthControllerBase
{
	private readonly IPlacesService placesService;

	public PlacesController(IPlacesService placesService)
	{
		this.placesService = placesService;
	}

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
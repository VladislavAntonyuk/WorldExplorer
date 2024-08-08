namespace WebApp.Apis.V1.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.Place;
using Shared.Models;

public class PlacesController(IPlacesService placesService) : ApiAuthControllerBase
{
	[HttpGet("recommendations")]
	[ProducesResponseType<List<Place>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public Task<OperationResult<List<Place>>> GetRecommendations([FromQuery] Location location, CancellationToken cancellationToken)
	{
		return placesService.GetNearByPlaces(location, cancellationToken);
	}

	[HttpGet("{id:guid}")]
	[ProducesResponseType<Place>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public Task<Place?> GetDetails(Guid id, CancellationToken cancellationToken)
	{
		return placesService.GetPlaceDetails(id, cancellationToken);
	}
}
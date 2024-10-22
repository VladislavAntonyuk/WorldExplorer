namespace WorldExplorer.Modules.Places.Presentation.Places;

using Application.Abstractions;
using Application.Places.GetNearByPlaces;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class GetRecommendations : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("places/recommendations", async ([AsParameters] Request location, ISender sender) =>
		   {
			   var result = await sender.Send(new GetNearByPlacesQuery(new Location(location.Latitude, location.Longitude)));

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization()
		   .WithTags(Tags.Places);
	}

	public record Request
	{
		public double Latitude { get; init; }
		public double Longitude { get; init; }
	}
}
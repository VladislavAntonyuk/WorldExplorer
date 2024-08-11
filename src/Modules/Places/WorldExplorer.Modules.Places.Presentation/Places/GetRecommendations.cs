namespace WorldExplorer.Modules.Places.Presentation.Places;

using System.Security.Claims;
using Application.Places.GetPlace;
using Common.Domain;
using Common.Infrastructure.Authentication;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.GetUser;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Places.Domain.Places;

internal sealed class GetRecommendations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
	{
        app.MapGet("places/recommendations", async (ClaimsPrincipal claims, [AsParameters] Request location, ISender sender) =>
        {
			Result<List<PlaceResponse>> result = await sender.Send(new GetNearByPlacesQuery(location.Longitude, location.Latitude));

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

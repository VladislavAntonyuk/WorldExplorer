namespace WorldExplorer.Modules.Places.Presentation.Places;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Places.Domain.Places;

internal sealed class GetRecommendations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
	{
        app.MapGet("places/recommendations", async (ClaimsPrincipal claims, [AsParameters] Location location, ISender sender) =>
        {
	  //      return placesService.GetNearByPlaces(location, cancellationToken);
			//Result<UserResponse> result = await sender.Send(new GetUserQuery(claims.GetUserId()));

   //         return result.Match(Results.Ok, ApiResults.Problem);

            return Results.Ok(new List<Place>());
        })
        //.RequireAuthorization()
        .WithTags(Tags.Places);
	}
}

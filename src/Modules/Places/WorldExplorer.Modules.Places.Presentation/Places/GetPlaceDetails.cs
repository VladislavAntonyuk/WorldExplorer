namespace WorldExplorer.Modules.Places.Presentation.Places;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorldExplorer.Common.Presentation.Endpoints;

internal sealed class GetPlaceDetails : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("{id:guid}", async (ClaimsPrincipal claims, Guid id, ISender sender) =>
		   {
			   //return placesService.GetPlaceDetails(id, cancellationToken);
			   //Result<UserResponse> result = await sender.Send(new GetUserQuery(claims.GetUserId()));

			  // return result.Match(Results.NoContent, ApiResults.Problem);

			  return Results.NotFound();
		   })
		   //.RequireAuthorization(Permissions.GetUser)
		   .WithTags(Tags.Places);
	}
}
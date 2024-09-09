namespace WorldExplorer.Modules.Places.Presentation.Places;

using Application.Places.GetPlace;
using Application.Places.GetPlaces;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class UpdatePlaces : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut("places/{id:guid}", async (Guid id, PlaceRequest placeRequest, ISender sender) =>
		   {
			   var result = await sender.Send(new UpdatePlaceCommand(id, placeRequest));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Places);

		app.MapDelete("places/{id:guid}", async (Guid id, ISender sender) =>
		   {
			   var result = await sender.Send(new DeletePlaceCommand(id));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Places);
	}
}
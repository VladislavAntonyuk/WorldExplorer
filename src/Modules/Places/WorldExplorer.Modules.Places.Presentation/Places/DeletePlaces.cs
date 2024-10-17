namespace WorldExplorer.Modules.Places.Presentation.Places;

using Application.Places.GetPlaces;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class DeletePlaces : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete("places/{id:guid}", async (Guid id, ISender sender) =>
		   {
			   var result = await sender.Send(new DeletePlaceCommand(id));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Places);

		app.MapDelete("places", async (ISender sender) =>
		   {
			   var result = await sender.Send(new DeletePlaceCommand(Guid.Empty));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Places);
	}
}
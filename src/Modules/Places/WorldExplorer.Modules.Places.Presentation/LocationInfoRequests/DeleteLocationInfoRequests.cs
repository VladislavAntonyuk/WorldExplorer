namespace WorldExplorer.Modules.Places.Presentation.LocationInfoRequests;

using Application.LocationInfoRequests.DeleteLocationInfoRequest;
using Application.LocationInfoRequests.DeleteLocationInfoRequests;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class DeleteLocationInfoRequests : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete("locationInfoRequests/{id:int}", async (int id, ISender sender) =>
		   {
			   var result = await sender.Send(new DeleteLocationInfoRequestCommand(id));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(PolicyConstants.AdministratorPolicy)
		   .WithTags(Tags.LocationInfoRequests);

		app.MapDelete("locationInfoRequests", async (ISender sender) =>
		   {
			   var result = await sender.Send(new DeleteLocationInfoRequestsCommand());

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(PolicyConstants.AdministratorPolicy)
		   .WithTags(Tags.LocationInfoRequests);
	}
}
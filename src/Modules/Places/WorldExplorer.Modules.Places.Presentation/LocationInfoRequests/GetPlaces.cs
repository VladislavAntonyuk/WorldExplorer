namespace WorldExplorer.Modules.Places.Presentation.LocationInfoRequests;

using Application.LocationInfoRequests.GetLocationInfoRequests;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class GetLocationInfoRequests : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("locationInfoRequests", async (ISender sender) =>
		   {
			   var result = await sender.Send(new GetLocationInfoRequestsQuery());

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization(PolicyConstants.AdministratorPolicy)
		   .WithTags(Tags.LocationInfoRequests);
	}
}
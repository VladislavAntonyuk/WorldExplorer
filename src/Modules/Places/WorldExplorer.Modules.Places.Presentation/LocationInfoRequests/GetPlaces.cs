namespace WorldExplorer.Modules.Places.Presentation.LocationInfoRequests;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorldExplorer.Common.Infrastructure.Authorization;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Common.Presentation.Results;
using WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequests;

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
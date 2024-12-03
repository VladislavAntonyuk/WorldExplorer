namespace WorldExplorer.Modules.Places.Presentation.LocationInfoRequests;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorldExplorer.Common.Infrastructure.Authorization;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Common.Presentation.Results;
using WorldExplorer.Modules.Places.Application.LocationInfoRequests.GetLocationInfoRequest;

internal sealed class GetLocationInfoRequest : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("locationInfoRequests/{id:int}", async (int id, ISender sender) =>
		   {
			   var result = await sender.Send(new GetLocationInfoRequestQuery(id));

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization(PolicyConstants.AdministratorPolicy)
		   .WithTags(Tags.LocationInfoRequests);
	}
}
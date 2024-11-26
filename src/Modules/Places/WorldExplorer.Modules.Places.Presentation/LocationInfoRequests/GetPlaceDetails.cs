namespace WorldExplorer.Modules.Places.Presentation.Places;

using Application.LocationInfoRequests.GetLocationInfoRequest;
using Application.Places.GetPlace;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
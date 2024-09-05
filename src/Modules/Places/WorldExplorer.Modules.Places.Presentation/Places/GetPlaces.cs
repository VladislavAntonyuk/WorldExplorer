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

internal sealed class GetPlaces : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("places", async (ISender sender) =>
		   {
			   var result = await sender.Send(new GetPlacesQuery());

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Places);
	}
}
namespace WorldExplorer.Modules.Places.Presentation.Places;

using Application.Places.GetPlace;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class GetPlaceDetails : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("places/{id:guid}", async (Guid id, ISender sender) =>
		   {
			   var result = await sender.Send(new GetPlaceDetailsQuery(id));

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization()
		   .WithTags(Tags.Places);
	}
}
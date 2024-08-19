namespace WorldExplorer.Modules.Places.Presentation.Places;

using System.Security.Claims;
using Common.Domain;
using Common.Infrastructure.Authentication;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.GetUser;
using WorldExplorer.Common.Presentation.Endpoints;

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
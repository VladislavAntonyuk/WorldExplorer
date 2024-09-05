namespace WorldExplorer.Modules.Users.Presentation.Users;

using System.Security.Claims;
using Application.Users.GetUser;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class GetUserProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("users/profile", async (ClaimsPrincipal claims, ISender sender) =>
		   {
			   var result = await sender.Send(new GetUserQuery(claims.GetUserId()));

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization()
		   .WithTags(Tags.Users);

		app.MapGet("users/{id:guid}", async (Guid id, ISender sender) =>
		   {
			   var result = await sender.Send(new GetUserQuery(id));

			   return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Users);
	}
}
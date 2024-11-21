namespace WorldExplorer.Modules.Users.Presentation.Users;

using System.Security.Claims;
using Application.Users.DeleteUser;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class DeleteUserProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete("users/profile", async (ClaimsPrincipal claims, ISender sender) =>
		   {
			   var result = await sender.Send(new DeleteUserCommand(claims.GetUserId()));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization()
		   .WithTags(Tags.Users);

		app.MapDelete("users/{id:guid}", async (Guid id, ISender sender) =>
		   {
			   var result = await sender.Send(new DeleteUserCommand(id));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization(PolicyConstants.AdministratorPolicy)
		   .WithTags(Tags.Users);
	}
}
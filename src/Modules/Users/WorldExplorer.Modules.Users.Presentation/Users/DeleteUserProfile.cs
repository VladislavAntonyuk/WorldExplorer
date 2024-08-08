using System.Security.Claims;
using WorldExplorer.Common.Domain;
using WorldExplorer.Common.Infrastructure.Authentication;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Common.Presentation.Results;
using WorldExplorer.Modules.Users.Application.Users.GetUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading;

namespace WorldExplorer.Modules.Users.Presentation.Users;

internal sealed class DeleteUserProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete("users/profile", async (ClaimsPrincipal claims, ISender sender) =>
		   {
			   var currentUserId = claims.GetUserId();
			   // var result = await userService.DeleteUser(currentUserId);
			   Result<UserResponse> result = await sender.Send(new GetUserQuery(claims.GetUserId()));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization()
		   .WithTags(Tags.Users);
	}
}
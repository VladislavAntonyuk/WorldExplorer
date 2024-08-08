using System.Security.Claims;
using WorldExplorer.Common.Domain;
using WorldExplorer.Common.Infrastructure.Authentication;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Common.Presentation.Results;
using WorldExplorer.Modules.Users.Application.Users.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorldExplorer.Modules.Users.Presentation.Users;

internal sealed class UpdateUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile", async (Request request,  ClaimsPrincipal claims, ISender sender) =>
        {

	        //var currentUserId = currentUserService.GetCurrentUser().ProviderId;
	        //if (currentUserId != user.Id)
	        //{
		       // return BadRequest();
	        //}

	        //await userService.UpdateUser(user, cancellationToken);
	        //return Ok();


			Result result = await sender.Send(new UpdateUserCommand(
                claims.GetUserId(),
                request.FirstName,
                request.LastName));

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.ModifyUser)
        .WithTags(Tags.Users);
    }

    internal sealed class Request
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}

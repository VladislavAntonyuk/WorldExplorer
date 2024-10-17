namespace WorldExplorer.Modules.Users.Presentation.Users;

using System.Security.Claims;
using Application.Users.UpdateUser;
using Common.Infrastructure.Authentication;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class UpdateUserProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut("users/profile", async (ClaimsPrincipal claims, Request settings, ISender sender) =>
		   {
			   var result = await sender.Send(new UpdateUserCommand(claims.GetUserId(), settings.TrackUserLocation));

			   return result.Match(Results.NoContent, ApiResults.Problem);
		   })
		   .RequireAuthorization()
		   .WithTags(Tags.Users);
	}

	internal sealed class Request
	{
		public bool TrackUserLocation { get; set; }
	}
}
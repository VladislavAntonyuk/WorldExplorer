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

using Domain.Users;

internal sealed class UpdateUserProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut("users/profile", async (ClaimsPrincipal claims, Request settings, ISender sender) =>
		   {
			   var result = await sender.Send(new UpdateUserCommand(
				                                  claims.GetUserId(),
				                                  settings.TrackUserLocation));

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
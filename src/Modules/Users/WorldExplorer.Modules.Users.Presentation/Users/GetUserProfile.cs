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

namespace WorldExplorer.Modules.Users.Presentation.Users;

internal sealed class GetUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/profile", async (ClaimsPrincipal claims, ISender sender) =>
        {
            Result<UserResponse> result = await sender.Send(new GetUserQuery(claims.GetUserId()));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
	}
}

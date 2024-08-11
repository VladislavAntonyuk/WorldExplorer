namespace WorldExplorer.Modules.Users.Presentation.Users;

using Application.Users.GetUser;
using Common.Infrastructure.Authorization;
using Common.Presentation.Endpoints;
using Common.Presentation.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

internal sealed class GetUsers : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("users", async (ISender sender) =>
		   {
			   var result = await sender.Send(new GetUsersQuery());

			   return result.Match(Results.Ok<List<UserResponse>>, ApiResults.Problem);
		   })
		   .RequireAuthorization(Constants.AdministratorPolicy)
		   .WithTags(Tags.Users);
	}
}
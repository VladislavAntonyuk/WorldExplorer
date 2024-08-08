using WorldExplorer.Common.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorldExplorer.Modules.Users.Presentation.Users;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

internal sealed class RegisterUser : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("users/register", async (Request? request, ISender sender) =>
		   {
			   // If input data is null, show block page
			   if (request is null)
			   {
				   return Results.BadRequest(new ResponseContent("ShowBlockPage", "There was a problem with your request."));
			   }

			   // TODO Require Auth
			   // var clientIds = configuration.GetRequiredSection("ClientIds").Get<string[]>();
			   // if (clientIds is null || !clientIds.Contains(requestConnector.ClientId))
			   // {
				  //  logger.LogWarning("HTTP clientId is not authorized.");
				  //  return Unauthorized();
			   // }

			   //if (string.IsNullOrWhiteSpace(requestConnector.ObjectId))
			   //{
			   //	return BadRequest(new ResponseContent("ShowBlockPage", "ObjectId is mandatory."));
			   //}

			   // var user = await graphClientService.GetUser(requestConnector.ObjectId, cancellationToken);
			   // if (user is null)
			   // {
				  //  logger.LogWarning("User {UserId} not found.", requestConnector.ObjectId);
				  //  return BadRequest(new ResponseContent("ShowBlockPage", "User not found."));
			   // }
			   //
			   // await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);

			   //var existedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == requestConnector.ObjectId,
			   //															cancellationToken);
			   //if (existedUser is null)
			   //{
			   //	await dbContext.Users.AddAsync(new User { Id = requestConnector.ObjectId }, cancellationToken);
			   //	await dbContext.SaveChangesAsync(cancellationToken);
			   //}

			   var result = new ResponseContent
			   {
				  // Groups = string.Join(',', user.Groups.Select(x => x.DisplayName)),
				 //  Language = user.Language.ToString()
			   };

			   return Results.Ok(result);




			   // Result<Guid> result = await sender.Send(new RegisterUserCommand(
				  //                                          request.Email, request.Password, request.FirstName,
				  //                                          request.LastName));
			   //
			   // return result.Match(Results.Ok, ApiResults.Problem);
		   })
		   // TODO Require Basic Auth
		   .AllowAnonymous()
		   .WithTags(Tags.Users);
	}

	// private bool IsAuthorized(HttpRequest req)
	// {
	// 	var username = configuration.GetValue<string>("AzureAdB2CClaimsBasicAuthUsername");
	// 	var password = configuration.GetValue<string>("AzureAdB2CClaimsBasicAuthPassword");
	//
	// 	// Check if the HTTP Authorization header exist
	// 	if (!req.Headers.ContainsKey("Authorization"))
	// 	{
	// 		logger.LogWarning("Missing HTTP basic authentication header.");
	// 		return false;
	// 	}
	//
	// 	// Read the authorization header
	// 	var auth = req.Headers["Authorization"].ToString();
	//
	// 	// Ensure the type of the authorization header id `Basic`
	// 	if (!auth.StartsWith("Basic "))
	// 	{
	// 		logger.LogWarning("HTTP basic authentication header must start with 'Basic '.");
	// 		return false;
	// 	}
	//
	// 	// Get the the HTTP basic authorization credentials
	// 	var cred = Encoding.UTF8.GetString(Convert.FromBase64String(auth[6..])).Split(':');
	//
	// 	// Evaluate the credentials and return the result
	// 	return cred[0] == username && cred[1] == password;
	// }

	internal sealed class Request
	{
		[JsonPropertyName("client_id")]
		[Required]
		public required string ClientId { get; init; }

		[JsonPropertyName("objectId")]
		[Required]
		public required Guid ObjectId { get; init; }
	}

	internal class ResponseContent
	{
		private const string ApiVersion = "1.0.0";

		public ResponseContent() : this("Continue")
		{
		}

		public ResponseContent(string action, string? userMessage = null)
		{
			Version = ApiVersion;
			Action = action;
			UserMessage = userMessage;
			if (action == "ValidationError")
			{
				Status = "400";
			}
		}

		[JsonPropertyName("version")]
		public string Version { get; }

		[JsonPropertyName("action")]
		public string Action { get; set; }

		[JsonPropertyName("userMessage")]
		public string? UserMessage { get; set; }

		[JsonPropertyName("status")]
		public string? Status { get; set; }

		[JsonPropertyName("extension_Groups")]
		public string Groups { get; set; } = string.Empty;

		[JsonPropertyName("extension_Language")]
		public string Language { get; set; } = string.Empty;
	}
}
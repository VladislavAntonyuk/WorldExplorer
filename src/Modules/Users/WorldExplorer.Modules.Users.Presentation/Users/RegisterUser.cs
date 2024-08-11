using WorldExplorer.Common.Presentation.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorldExplorer.Modules.Users.Presentation.Users;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Users.RegisterUser;
using Common.Presentation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

			   var result = await sender.Send(new RegisterUserCommand(request.ObjectId));
			   if (result.IsSuccess)
			   {
				   return Results.Ok(new ResponseContent
				   {
					   Language = result.Value.Language.ToString(),
					   Groups = result.Value.Groups
				   });
			   }

			   return Results.BadRequest(new ResponseContent("ShowBlockPage", result.Error.Description));
		   })
		   .RequireAuthorization("BasicAuthenticationPolicy")
		   .WithTags(Tags.Users);
	}

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
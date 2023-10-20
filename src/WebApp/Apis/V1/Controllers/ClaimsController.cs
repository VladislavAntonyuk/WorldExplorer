namespace WebApp.Apis.V1.Controllers;

using System.Text;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using WebApp.Services.User;

public class ClaimsController(IGraphClientService graphClientService,
	ILogger<ClaimsController> logger,
	IConfiguration configuration,
	IDbContextFactory<WorldExplorerDbContext> factory) : ApiControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Post([FromBody] RequestConnector? requestConnector,
		CancellationToken cancellationToken)
	{
		// Check HTTP basic authorization
		if (!IsAuthorized(Request))
		{
			logger.LogWarning("HTTP basic authentication validation failed.");
			return Unauthorized();
		}

		// If input data is null, show block page
		if (requestConnector is null)
		{
			return BadRequest(new ResponseContent("ShowBlockPage", "There was a problem with your request."));
		}

		var clientIds = configuration.GetRequiredSection("ClientIds").Get<string[]>();
		if (clientIds is null || !clientIds.Contains(requestConnector.ClientId))
		{
			logger.LogWarning("HTTP clientId is not authorized.");
			return Unauthorized();
		}

		if (string.IsNullOrWhiteSpace(requestConnector.ObjectId))
		{
			return BadRequest(new ResponseContent("ShowBlockPage", "ObjectId is mandatory."));
		}

		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);

		var existedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == requestConnector.ObjectId,
																	cancellationToken);
		if (existedUser is null)
		{
			await dbContext.Users.AddAsync(new User
				                               { Id = requestConnector.ObjectId }, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		var user = await graphClientService.GetUser(requestConnector.ObjectId, cancellationToken);
		var result = new ResponseContent
		{
			Groups = string.Join(',', user.Groups.Select(x => x.DisplayName)),
			Language = user.Language.ToString()
		};

		return Ok(result);
	}

	private bool IsAuthorized(HttpRequest req)
	{
		var username = configuration.GetValue<string>("AzureAdB2CClaimsBasicAuthUsername");
		var password = configuration.GetValue<string>("AzureAdB2CClaimsBasicAuthPassword");

		// Check if the HTTP Authorization header exist
		if (!req.Headers.ContainsKey("Authorization"))
		{
			logger.LogWarning("Missing HTTP basic authentication header.");
			return false;
		}

		// Read the authorization header
		var auth = req.Headers["Authorization"].ToString();

		// Ensure the type of the authorization header id `Basic`
		if (!auth.StartsWith("Basic "))
		{
			logger.LogWarning("HTTP basic authentication header must start with 'Basic '.");
			return false;
		}

		// Get the the HTTP basic authorization credentials
		var cred = Encoding.UTF8.GetString(Convert.FromBase64String(auth[6..])).Split(':');

		// Evaluate the credentials and return the result
		return cred[0] == username && cred[1] == password;
	}
}
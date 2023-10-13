namespace WebApp.Services;

using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta;
using Shared.Models;

public class AzureAdB2CGraphClientConfiguration
{
	public const string ConfigurationName = "AzureAdB2CGraphClient";
	public string? ClientId { get; set; }

	public string? ClientSecret { get; set; }
	public string? TenantId { get; set; }
}

public class UserService
	(GraphServiceClient graphClient, IDbContextFactory<WorldExplorerDbContext> factory) : IUserService
{
	public async Task<User?> GetUser(string providerId, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(providerId))
		{
			return null;
		}

		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);

		var dbUser = await dbContext.Users
									.Include(user => user.Visits)
									.FirstOrDefaultAsync(u => u.Id == providerId, cancellationToken);
		if (dbUser is null)
		{
			return null;
		}

		var profile = await graphClient.Users[providerId].GetAsync(cancellationToken: cancellationToken);
		if (profile is null)
		{
			return null;
		}

		return new User
		{
			Id = dbUser.Id,
			Visits = dbUser.Visits.Select(ToDto).ToList(),
			Name = profile.DisplayName ?? string.Empty,
			Email = profile.OtherMails?.FirstOrDefault() ?? string.Empty,
			Activities = new List<UserActivity>
			{
				new()
				{
					Date = DateTime.Today,
					Steps = 50
				},
				new()
				{
					Date = DateTime.Today.AddDays(-1),
					Steps = 150
				}
			}
		};
	}

	public async Task DeleteUser(string providerId, CancellationToken cancellationToken)
	{
		await using var dbContext = await factory.CreateDbContextAsync(cancellationToken);
		await dbContext.Users.Where(x => x.Id == providerId).ExecuteDeleteAsync(cancellationToken);
		await graphClient.Users[providerId].DeleteAsync(cancellationToken: cancellationToken);
	}

	private static Visit ToDto(Infrastructure.Entities.Visit arg)
	{
		return new Visit
		{
			Id = arg.Id,
			Place = new Place
			{
				Id = arg.PlaceId,
				Name = string.Empty,
				Location = new Location(0, 0)
			},
			VisitDate = arg.VisitDate
		};
	}
}